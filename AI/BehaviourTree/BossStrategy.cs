using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AI.BehaviourTree;
using static Assets.AI.BehaviourTree.BossStrategy;

namespace Assets.AI.BehaviourTree
{
    public class BossStrategy : AI_DefaultStrategy
    {
        [Serializable]
        public struct BossBreakSetting
        {
            public float BossBreakAtHPperc;
            public float BossBreakTime;
            public CombatBuff AddBuff;
            //public 
        }

        int curTiming;

        [SerializeField] List<BossBreakSetting> _bossBreakTimings = new();
        void Start()
        {
        }

        void Update()
        {
            if (_executor.IsBoss && !_executor.bossActivated) return;
            if(gameObject.activeSelf && !_executor.IsDying)
                _behaviorTree?.Evaluate();
        }

        public override void BuildBehaviorTree(EnemyStateExecutor executor, AIMethods methods)
        {
            _executor = executor;
            _methods = methods;
            IBehaviourSubTree[] strategies = GetComponents<IBehaviourSubTree>();

            _behaviorTree = new PrioritySelector("Root");
            var hitSequence = new Sequence("HitSequence", 100);
            hitSequence.AddChild(new Leaf("CheckIsHit", new Condition(() => executor.IsHit)));

            // if boss is hit, check it's hp is dropped to certain percentage, and start breaking sequence
            // if not dropped, then 
            var HitCheckSelector = new PrioritySelector("HitCheckSelector");
            HitCheckSelector.AddChild(new Leaf("ProcessHit", new Strategy_Boss_Hit(executor, methods, _bossBreakTimings), 100));
            HitCheckSelector.AddChild(new Leaf("HitComfirm",
                new ActionStrategy(() => { executor.IsHit = false; })
            ));
            hitSequence.AddChild(HitCheckSelector);
            _behaviorTree.AddChild(hitSequence);

            var SequenceChase = new Sequence("SequenceChase", 1);
            _behaviorTree.AddChild(SequenceChase);

            var chaseSelector = new Sequence("ChaseSelector");
            chaseSelector.AddChild(new Leaf("LA_Chase Player", new Strategy_Boss_Chase(executor, methods)));
            SequenceChase.AddChild(chaseSelector);

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


            void AddAttackSubTree()
            {
                var AI_Attack = TryGetSubTree<AI_MeleeAttack>();
                if (AI_Attack != null)
                {
                    chaseSelector.AddChild(AI_Attack);
                    return;
                }
                AI_Attack = TryGetSubTree<AI_RangeAttack>();
                if (AI_Attack != null)
                {
                    chaseSelector.AddChild(AI_Attack);
                    return;
                }
                AI_Attack = TryGetSubTree<AI_DetonateAttack>();
                if (AI_Attack != null)
                {
                    chaseSelector.AddChild(AI_Attack);
                    return;
                }
            }
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
        }

        public override void OnHit()
        {
            _executor.IsHit = true;
            _executor.IsInvincible = true;
            _executor.Agent.ResetPath();
            _methods.ResetAllAnimationTriggers();
            _executor.SoundManager.PlaySound("Hurt");
            StartCoroutine(ExtendIEnumerator.DelayAction(0.15f, 
                () => { _executor.IsInvincible = false; }));
        }
    }

    public class Strategy_Boss_Hit : IStrategy
    {
        readonly AIMethods _methods;
        readonly EnemyStateExecutor _executor;
        float _hitTimer;
        List<BossBreakSetting> _bossBreakTimings;
        int curTiming = 0;
        bool _bossBreak = false;

        public Strategy_Boss_Hit(EnemyStateExecutor executor, AIMethods methods, List<BossBreakSetting> timings)
        {
            _methods = methods;
            _executor = executor;
            _bossBreakTimings = timings;
        }

        public Node.Status Evaluate()
        {
            if (curTiming >= _bossBreakTimings.Count) return Node.Status.FAILURE;
            BossBreakSetting curBreak = _bossBreakTimings[curTiming];
            float HPperc = _executor.CombatStats.HP / _executor.CombatStats.MaxHP;
            //Debug.Log($"Boss HP perc: {HPperc*100}%, cur HP determent: {_bossBreakTimings[curTiming].BossBreakAtHPperc}");
            if (HPperc > curBreak.BossBreakAtHPperc / 100)
            {
                return Node.Status.FAILURE;
            }

            if (!_bossBreak)
            {
                Debug.Log($"Boss HP drops less than {curBreak.BossBreakAtHPperc}%");
                _bossBreak = true;
                _hitTimer = curBreak.BossBreakTime;
                _executor.Animator.Play("Hit");
                _executor.SoundManager.PlaySound("Hurt");
                if (curBreak.AddBuff != null)
                {
                    _executor.GetComponent<IBuffReceiver>().AddBuff(curBreak.AddBuff);
                }
            }
            _hitTimer -= Time.deltaTime;
            if (_executor.HitAnimTime - _hitTimer > 0.5f)
            {
                _methods.LookPlayer(5.0f);
            }
            if (_hitTimer > 0)
            {
                return Node.Status.RUNNING;
            }

            curTiming = Mathf.Min(curTiming +1, _bossBreakTimings.Count);
            _bossBreak = false;
            _executor.IsHit = false;
            _methods.ResetModel();
            return Node.Status.SUCCESS;
        }

        public void OnStatusRunning()
        {
            if (curTiming >= _bossBreakTimings.Count) return;
        }

        public void Reset() => _hitTimer = 0;
    }

    public class Strategy_Boss_Chase : IStrategy
    {
        readonly EnemyStateExecutor _executor;
        readonly AIMethods _methods;

        public Strategy_Boss_Chase(
            EnemyStateExecutor executor,
            AIMethods methods)
        {
            _methods = methods;
            _executor = executor;
        }

        public Node.Status Evaluate()
        {
            _methods.ResetModel();
            _executor.Animator.SetBool("IsChasing", true);
            
            // chase player forever
            if (_methods.CanAttackPlayer())
            {
                return Node.Status.SUCCESS;
            }
            _executor.Agent.SetDestination(_executor.Player.position);

            return Node.Status.RUNNING;
        }

        public void OnStatusRunning()
        {
            _executor.Agent.speed = _executor.CombatStats.Speed /10 * 1.5f; // change to running speed
            _executor.Agent.isStopped = false;
        }

        public void OnStatusFailure()
        {
            _executor.Agent.ResetPath();
            _executor.Animator.SetBool("IsChasing", false);
        }
    }

    
}
