using System.Collections;
using UnityEngine;

namespace Assets.AI.BehaviourTree
{
    public abstract class AI_AttackStrategyBase : MonoBehaviour
    {
        protected Node _behaviorTree;

        protected EnemyStateExecutor _executor;
        protected AIMethods _methods;
    }
}