using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AI.BehaviourTree;

namespace Assets.AI.BehaviourTree
{
    public class AI_DefaultStrategy : MonoBehaviour
    {
        [SerializeField] protected GameObject _visualRootPrefab;
        [SerializeField] protected Transform _visualParent;
        protected Node _behaviorTree;

        protected EnemyStateExecutor _executor;
        protected AIMethods _methods;

        void Start()
        {
        }

        void Update()
        {
            if(gameObject.activeSelf && !_executor.IsDying)
                _behaviorTree?.Evaluate();
        }

        public virtual void BuildBehaviorTree(EnemyStateExecutor executor, AIMethods methods)
        {
            _executor = executor;
            _methods = methods;
            IBehaviourSubTree[] strategies = GetComponents<IBehaviourSubTree>();

            _behaviorTree = new PrioritySelector("Root");
            var hitSequence = new Sequence("HitSequence", 100);
            hitSequence.AddChild(new Leaf("CheckHitCondition",
                new Condition(() => executor.IsHit && executor.Interruptable)));
            hitSequence.AddChild(new Leaf("ProcessHit", new Strategy_AI_Hit(executor, methods)));
            _behaviorTree.AddChild(hitSequence);

            var SequenceChase = new Sequence("SequenceChase", 1);
            _behaviorTree.AddChild(SequenceChase);

            var chaseSequence = new Sequence("ChaseSelector");
            chaseSequence.AddChild(new Leaf("LA_Chase Player", new Strategy_AI_Chase(executor, methods)));
            

            SequenceChase.AddChild(new Leaf("CanStartChasePlayer",
                new Condition(() => { return methods.CanStartChase(); })));
            SequenceChase.AddChild(chaseSequence);

            _behaviorTree.AddChild(new AI_StrategyPatrol().BuildBehaviorTree(executor, methods));
            AddAttackSubTree();
            // 创建可视化根节点
            var visualRoot = Instantiate(_visualRootPrefab, _visualParent);

            var visual = visualRoot.GetComponent<VisualNode>();
            visual.Initialize(_behaviorTree);
            visual.CreateChildVisualizers(_behaviorTree, _visualParent);
            visual.DestroyVisualizers(_behaviorTree);
            StartCoroutine(ExtendIEnumerator.ActionInNextFrame(() =>
            {
                _visualParent.gameObject.SetActive(true);
                VisualNode script = TryGetVisualTree(out Transform rootTree);
                if(rootTree != null && script != null)
                    script.RebalanceChildren(rootTree);

                StartCoroutine(ExtendIEnumerator.ActionInNextFrame(() =>
                {
                    if (rootTree != null && script != null)
                        script.RebalanceSubTree(rootTree);
                    _visualParent.gameObject.SetActive(false);
                }));
            }));

            Node TryGetSubTree<T>()
            {
                foreach(var strategy in strategies)
                {
                    if(strategy is T)
                    {
                        return strategy.BuildBehaviorTree(executor, methods);
                    }
                }
                return null;
            }
            VisualNode TryGetVisualTree(out Transform rootTree)
            {
                rootTree = null;
                foreach (Transform child in _visualParent)
                {
                    if (!child.CompareTag("BT_VisualObj")) continue;
                    rootTree = child;
                    break;
                }
                if (rootTree == null) return null;
                return rootTree.GetComponent<VisualNode>();
            }

            void AddAttackSubTree()
            {
                var AI_Attack = TryGetSubTree<AI_MeleeAttack>();
                if (AI_Attack != null) {
                    chaseSequence.AddChild(AI_Attack);
                    return;
                }
                AI_Attack = TryGetSubTree<AI_RangeAttack>();
                if (AI_Attack != null)
                {
                    chaseSequence.AddChild(AI_Attack);
                    return;
                }
                AI_Attack = TryGetSubTree<AI_DetonateAttack>();
                if (AI_Attack != null)
                {
                    chaseSequence.AddChild(AI_Attack);
                    return;
                }
            }
        }

        public virtual void OnHit()
        {
            _executor.IsHit = true;
            _executor.IsInvincible = true;
            _executor.Agent.ResetPath();
            _methods.ResetAllAnimationTriggers();
            _executor.SoundManager.PlaySound("Hurt");
            StartCoroutine(ExtendIEnumerator.DelayAction(0.3f, 
                () => { _executor.IsInvincible = false; }));
        }
    }

    public class Strategy_AI_Hit : IStrategy
    {
        readonly AIMethods _methods;
        readonly EnemyStateExecutor _executor;
        float _hitTimer;

        public Strategy_AI_Hit(EnemyStateExecutor executor, AIMethods methods)
        {
            _methods = methods;
            _executor = executor;
        }

        public Node.Status Evaluate()
        {
            //Debug.Log($"isHit: {_executor.IsHit}, hitTimer: {_hitTimer}");
            if (!_executor.IsHit || !_executor.Interruptable)
            {
                return Node.Status.FAILURE;
            }
            if (_hitTimer <= 0)
            {
                _executor.Agent.isStopped = true;
                _hitTimer = _executor.HitAnimTime;
            }

            _hitTimer -= Time.deltaTime;

            if (_hitTimer > 0)
            {
                return Node.Status.RUNNING;
            }

            if (_executor.HitAnimTime - _hitTimer > 0.5f)
            {
                _methods.LookPlayer(5.0f);
            }

            _executor.Agent.isStopped = false;
            _executor.IsHit = false;
            _executor.chasePlayerForever = true;
            _methods.ResetModel();
            return Node.Status.SUCCESS;
        }

        public void OnStatusRunning()
        {
            _hitTimer = _executor.HitAnimTime;
            _executor.Animator.Play("Hit");
        }

        public void Reset() => _hitTimer = 0;
    }

    public class Strategy_AI_Chase : IStrategy
    {
        readonly EnemyStateExecutor _executor;
        readonly AIMethods _methods;
        float _chaseTimer;
        Transform modelTransform;

        public Strategy_AI_Chase(
            EnemyStateExecutor executor,
            AIMethods methods)
        {
            _methods = methods;
            _executor = executor;
            modelTransform = executor.Animator.transform;
        }

        public Node.Status Evaluate()
        {
            _methods.ResetModel();
            _executor.Animator.SetBool("IsChasing", true);
            bool shouldStopChase = _methods.CanStopChase() || _chaseTimer > _executor.ChaseTime;
            if (shouldStopChase)
            {
                return Node.Status.FAILURE;
            }
            // 检查是否到达目标点
            if (_methods.CanAttackPlayer())
            {
                return Node.Status.SUCCESS;
            }
            _chaseTimer += Time.deltaTime;
            if (_executor.WaitTimer < _executor.CombatStats.AttackTime)
                _executor.WaitTimer += Time.deltaTime;
            _executor.Agent.SetDestination(_executor.Player.position);

            return Node.Status.RUNNING;
        }

        public void OnStatusRunning()
        {
            _executor.Agent.speed = _executor.CombatStats.Speed /10 * 1.5f; // change to running speed
            _executor.Agent.isStopped = false;
            _chaseTimer = 0;
        }

        public void OnStatusFailure()
        {
            _chaseTimer = 0;
            _executor.Agent.ResetPath();
            _executor.Animator.SetBool("IsChasing", false);
        }
    }

    
}
