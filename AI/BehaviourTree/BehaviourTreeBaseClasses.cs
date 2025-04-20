using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.AI.BehaviourTree
{
    public interface IStrategy
    {
        Node.Status Evaluate();
        virtual void Reset() { }

        public virtual void OnStatusSuccess() { }
        public virtual void OnStatusRunning() { }
        public virtual void OnStatusFailure() { }
    }

    public interface IBehaviourSubTree
    {
        Node BuildBehaviorTree(EnemyStateExecutor executor, AIMethods methods);
    }

    public abstract class Node
    {
        public enum Status { SUCCESS, FAILURE, RUNNING, NONE };
        public string nodeName;
        public int priority;
        public List<Node> children = new();
        public Node parent;
        public virtual event Action<Status> OnStatusChanged;

        protected Status _lastStatus = Status.NONE;
        protected int curChild;

        public Node(string name = "NewNode", int priority = 0)
        {
            nodeName = name;
            this.priority = priority;
        }

        public virtual void AddChild(Node child)
        {
            children.Add(child);
            child.parent = this;
        }

        public virtual Status Evaluate()
        {
            var newStatus = EvaluateImpl();
            CheckStatusChange(newStatus);
            return newStatus;
        }

        public abstract Status EvaluateImpl();

        public virtual void Reset()
        {
            curChild = 0;
            _lastStatus = Status.FAILURE;
            foreach (Node child in children)
            {
                child.Reset();
            }
        }

        protected bool CheckStatusChange(Status newStatus)
        {
            if (newStatus != _lastStatus)
            {
                OnStatusChanged?.Invoke(newStatus);
                _lastStatus = newStatus;
                return true;
            }
            return false;
        }

        protected virtual void AddStrategyChangedEventToLeaf(Action<Status> e) { }

        protected virtual void RemoveStrategyChangedEventFromLeaf(Action<Status> e) { }
    }

    public abstract class EventNode : Node
    {
        public EventNode(string name = "NewNode", int priority = 0) : base(name, priority) { }

        public override event Action<Status> OnStatusChanged
        {
            add
            {
                base.OnStatusChanged += value;
                AddStrategyChangedEventToLeaf(value);
            }
            remove
            {
                base.OnStatusChanged -= value;
                RemoveStrategyChangedEventFromLeaf(value);
            }
        }
    }

    public class BehaviourTree : EventNode
    {
        public BehaviourTree(string name) : base(name) { }

        public override Status EvaluateImpl()
        {
            while (curChild < children.Count)
            {
                Status status = children[curChild].Evaluate();
                if (status != Status.SUCCESS)
                {
                    return status;
                }
                curChild++;
            }
            return Status.SUCCESS;
        }
    }

    public class Leaf : EventNode
    {
        readonly IStrategy strategy;
        Status _lastStrategyStatus = Status.NONE;

        event Action<Status> _strategyChangedEvent;

        public Leaf(string name, IStrategy strategy, int priority = 0) : base(name, priority)
        {
            this.strategy = strategy;
        }

        public override Status Evaluate()
        {
            var baseStatus = base.Evaluate();
            return baseStatus;
        }

        public override Status EvaluateImpl()
        {
            var rawStatus = strategy.Evaluate();
            CheckStatusChange(rawStatus);
            if (_lastStrategyStatus != rawStatus)
            {
                //Debug.Log($"{nodeName}：change strategy from {_lastStrategyStatus} to {rawStatus}");
                _strategyChangedEvent?.Invoke(rawStatus);
                HandleStrategyStatusChange(rawStatus);
                _lastStrategyStatus = rawStatus;
            }
            return rawStatus;
        }

        public override void Reset()
        {
            strategy.Reset();
            _lastStrategyStatus = Status.NONE;
            base.Reset();
        }

        public IStrategy GetStrategy() => strategy;

        void HandleStrategyStatusChange(Status status)
        {
            switch (status)
            {
                case Status.RUNNING:
                    //Debug.Log($"{nodeName}：strategy started to running");
                    strategy.OnStatusRunning();
                    break;
                case Status.SUCCESS:
                    //Debug.Log($"{nodeName}：strategy changed to success");
                    strategy.OnStatusSuccess();
                    break;
                case Status.FAILURE:
                    //Debug.Log($"{nodeName}：strategy changed to failure");
                    strategy.OnStatusFailure();
                    break;
            }
        }

        protected override void AddStrategyChangedEventToLeaf(Action<Status> e)
        {
            _strategyChangedEvent += e;
        }

        protected override void RemoveStrategyChangedEventFromLeaf(Action<Status> e)
        {
            _strategyChangedEvent -= e;
        }
    }

    public class Sequence : EventNode
    {
        public Sequence(string name, int priority = 0) : base(name, priority) { }

        public override Status EvaluateImpl()
        {
            if (curChild < children.Count)
            {
                switch (children[curChild].Evaluate())
                {
                    case Status.RUNNING:
                        return Status.RUNNING;
                    case Status.FAILURE:
                        Reset();
                        return Status.FAILURE;
                    default:
                        curChild++;
                        return curChild == children.Count ? Status.SUCCESS : Status.RUNNING;
                }
            }
            Reset();
            return Status.SUCCESS;
        }
    }

    public class SequenceLoop : EventNode
    {
        public SequenceLoop(string name, int priority = 0) : base(name, priority) { }

        public override Status EvaluateImpl()
        {
            foreach (var child in children)
            {
                if (child.Evaluate() == Status.FAILURE)
                {
                    Reset();
                    return Status.FAILURE;
                }
            }
            return Status.RUNNING;
        }
    }

    public class Selector : EventNode
    {
        public Selector(string name, int priority = 0) : base(name, priority) { }

        public override Status EvaluateImpl()
        {
            if (curChild < children.Count)
            {
                switch (children[curChild].Evaluate())
                {
                    case Status.RUNNING:
                        return Status.RUNNING;
                    case Status.SUCCESS:
                        Reset();
                        return Status.SUCCESS;
                    default:
                        curChild++;
                        return Status.RUNNING;
                }
            }

            return Status.FAILURE;
        }
    }

    public class PrioritySelector : Selector
    {
        List<Node> AfterSortedChildren => children ??= SortChildren();

        public PrioritySelector(string name, int priority = 0) : base(name, priority) { }

        protected virtual List<Node> SortChildren() => children.OrderByDescending(c => c.priority).ToList();

        public override void Reset()
        {
            base.Reset();
            //SortedChildren = null;
        }

        public override void AddChild(Node child)
        {
            base.AddChild(child);
        }

        public override Status EvaluateImpl()
        {
            Status status = Status.FAILURE;
            bool childSelected = false;
            foreach (var child in AfterSortedChildren)
            {
                if (!childSelected) { 
                    //Debug.Log($"Checking child {child.nodeName} with priority {child.priority}");
                    switch (child.Evaluate())
                    {
                        case Status.RUNNING:
                            status = Status.RUNNING;
                            childSelected = true;
                            break;
                        case Status.SUCCESS:
                            status = Status.SUCCESS;
                            childSelected = true;
                            break;
                    }
                }
                else
                {
                    child.Reset();
                }
            }
            return status;
        }
    }

    public class Invertor : EventNode
    {
        public Invertor(string name, int priority = 0) : base(name, priority) { }

        public override Status EvaluateImpl()
        {
            switch (children[curChild].Evaluate())
            {
                case Status.RUNNING:
                    return Status.RUNNING;
                case Status.FAILURE:
                    return Status.SUCCESS;
                default:
                    return Status.FAILURE;
            }
        }
    }

    public class UntilFail : EventNode
    {
        public UntilFail(string name, int priority = 0) : base(name, priority) { }

        public override Status EvaluateImpl()
        {
            if (children[0].Evaluate() == Status.FAILURE)
            {
                Reset();
                return Status.FAILURE;
            }
            return Status.RUNNING;
        }
    }

    public class Condition : IStrategy
    {
        readonly Func<bool> predicate;

        public Condition(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public Node.Status Evaluate()
        {
            return predicate() ? Node.Status.SUCCESS : Node.Status.FAILURE;
        }

        public void Reset()
        {
            // nothing happens default
        }
    }

    public class ActionStrategy : IStrategy
    {
        readonly Action action;

        public ActionStrategy(Action doSomething)
        {
            action = doSomething;
        }

        public Node.Status Evaluate()
        {
            action();
            return Node.Status.SUCCESS;
        }

        public void Reset()
        {
            // nothing happens default
        }
    }
}
