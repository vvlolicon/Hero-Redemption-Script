using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Metadata;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.HID;


namespace Assets.AI.BehaviourTree
{
#if UNITY_EDITOR
    public class VisualNode : MonoBehaviour
    {
        [SerializeField] public MeshRenderer _indicator;
        [SerializeField] public TMP_Text _label;
        public Canvas _canvas;
        public List<GameObject> prefabs;
        public Node node;
        static readonly float childSpacing = 1.2f;// 子节点之间的水平间距


        // color
        static readonly Color runningColor = Color.yellow;
        static readonly Color successColor = Color.green;
        static readonly Color failureColor = Color.red;
        static readonly Color inactiveColor = Color.gray;

        static readonly Color SequenceColor = Color.green;
        static readonly Color SelectorColor = Color.cyan;
        static readonly Color PrioritySelectorColor = new Color(0f, 0.5f, 1f); // lightblue
        static readonly Color SequenceLoopColor = new Color(1f,0.5f,0f); //orange
        static readonly Color ConditionColor = Color.yellow;
        static readonly Color ActionColor = new Color(0f, 0.5f, 0f);//deep green

        private Color _connectionLineColor = Color.white;
        private Color _connectionLineHighlight = Color.red;

        public void Initialize(Node node)
        {
            string name = (node.nodeName != null) ? node.nodeName : "Node Name";
            _label.text = name;
            gameObject.name = name;
            _canvas.worldCamera = Camera.main;
            _label.color = GetNodeTextColor(node);
            this.node = node;
            node.OnStatusChanged += HandleStatusChange;
            if (node.parent == null)
            {
                //Debug.Log("resetting root node pos");
                transform.localPosition = transform.localPosition.ChangeAxisValue(ExtendVector3.Axis.X, 0);
            }
        }

        private void OnEnable()
        {
            if(node != null)
                node.OnStatusChanged += HandleStatusChange;
        }
        private void OnDisable()
        {
            if (node != null)
                node.OnStatusChanged -= HandleStatusChange;
        }

        public void CreateChildVisualizers(Node node, Transform parentNodeTransform = null, float xOffset = 0)
        {
            var childObj = Instantiate(GetPrefabOfNode(node), parentNodeTransform);
            childObj.transform.localPosition = new Vector3(xOffset, -0.8f, 0);
            var visual = childObj.GetComponent<VisualNode>();
            visual.prefabs = this.prefabs;
            visual.Initialize(node);
            // 对所有子节点递归调用
            float childXOffset = 0f; // 子节点的水平偏移
            foreach (var childNode in node.children)
            {
                CreateChildVisualizers(childNode, visual.transform, childXOffset);
                childXOffset += childSpacing; // 更新子节点的水平偏移
            }
        }
        
        float GetWidthOfSubTree(Node node)
        {
            if (node.children.Count <= 1) return 0;
            
            float totalWidth = 0f;
            foreach (var child in node.children)
            {
                totalWidth += GetWidthOfSubTree(child) + childSpacing;
            }
            return totalWidth - childSpacing;

        }

        public void RebalanceChildren(Transform parentTransform)
        {
            int childCount = 0;
            List<Transform> childTransforms = new List<Transform>();
            foreach (Transform child in parentTransform)
            {
                if (child.CompareTag("BT_VisualObj"))
                {
                    childCount++;
                    childTransforms.Add(child);
                    RebalanceChildren(child);
                }
            }
            if (childCount < 2) return;
            Node parentNode = parentTransform.GetComponent<VisualNode>().node;
            float childMinusXOffset = CalculateNodeWidth(parentNode);
            foreach (Transform child in childTransforms)
            {
                child.localPosition = child.localPosition.ChangeAxisValue(
                        ExtendVector3.Axis.X,
                        child.localPosition.x - childMinusXOffset);
            }
            Transform grandParent = parentTransform.parent;
            if (grandParent.CompareTag("BT_VisualObj"))
            {
                int multiplier = 1;
                int childIndex = 0;
                int grandParentChildCount = grandParent.GetComponent<VisualNode>().node.children.Count;
                //Debug.Log($"grandParent {grandParent.gameObject.name} has {grandParentChildCount} children");
                for (int i = 0; i< grandParent.childCount; i++)
                {
                    Transform child = grandParent.GetChild(i);
                    if (child.CompareTag("BT_VisualObj"))
                    {
                        if (child == parentTransform)
                        {
                            //Debug.Log($"{parentTransform.gameObject.name} is at the {childIndex} position of its parent");
                            if ((grandParentChildCount - 1) / 2 == childIndex)
                            {
                                //Debug.Log($"{parentTransform.gameObject.name} is at the middle of its parent");
                                multiplier = 0; // node exactly at the middle
                            }
                            else if (childIndex < grandParentChildCount / 2)
                            {
                                //Debug.Log($"{parentTransform.gameObject.name} is at the LHS of its parent");
                                multiplier = -1; // node at the left hand side
                            }
                            else
                            {
                                //Debug.Log($"{parentTransform.gameObject.name} is at the RHS of its parent");
                            }
                            break;
                        }
                        childIndex++;
                    }
                }
                if(multiplier == 0)
                {
                    StartCoroutine(ExtendIEnumerator.ActionInNextFrame(() =>
                    {
                        parentTransform.localPosition = parentTransform.localPosition.ChangeAxisValue(
                            ExtendVector3.Axis.X, 0);
                    }));
                    return;
                }
                float parentXoffset = childSpacing / 2 * childTransforms.Count / 2 * multiplier;
                if (childTransforms.Count >= 2)
                {
                    parentTransform.localPosition = parentTransform.localPosition.ChangeAxisValue(
                        ExtendVector3.Axis.X,
                        parentTransform.localPosition.x + parentXoffset);
                }
            }

            float CalculateNodeWidth(Node node)
            {
                if (node.children.Count < 2) return 0;
                return childSpacing / 2 * (childCount-1);
            }
        }

        public void RebalanceSubTree(Transform root)
        {
            List<float> subTreeWidths = new List<float>();
            for(int i = 0; i< root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (!child.CompareTag("BT_VisualObj")) continue;
                Node node = child.GetComponent<VisualNode>().node;
                float width = GetWidthOfSubTree(node)+ childSpacing;
                //Debug.Log($"parent {child.gameObject.name} width: {width}");
                subTreeWidths.Add(width);
            }
            float totalWidth = subTreeWidths.Sum();
            int childIndex = 0;
            int middleIndex = subTreeWidths.Count%2 == 0? subTreeWidths.Count / 2: (subTreeWidths.Count - 1) /2;
            //Debug.Log($"middleIndex: {middleIndex}, subTreeWidths count: {subTreeWidths.Count}");
            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (!child.CompareTag("BT_VisualObj")) continue;
                float totalOffset = 0;
                float childMidWidth = subTreeWidths[childIndex] / 2;
                float midNodeHalfWidth = subTreeWidths[middleIndex] / 2;
                if ((subTreeWidths.Count - 1) / 2 == childIndex)
                {
                    //Debug.Log($"{child.gameObject.name} with index {childIndex} is at the middle of its parent");
                    childIndex++;
                    continue;// node exactly at the middle
                }
                else if (childIndex < subTreeWidths.Count / 2)
                {// node at the left hand side
                    //Debug.Log($"{child.gameObject.name} with index {childIndex} is at the LHS of its parent");
                    if (subTreeWidths.Count % 2 == 0)
                    { // has even number of nodes
                        totalOffset = childMidWidth + SumBetweenWidthOfIndex(childIndex + 1, middleIndex-1);
                    }
                    else
                    { // has odd number of nodes
                        totalOffset = childMidWidth + midNodeHalfWidth + SumBetweenWidthOfIndex(childIndex + 1, middleIndex - 1);
                    }
                    totalOffset *= -1;
                }
                else
                {
                    //Debug.Log($"{child.gameObject.name} with index {childIndex} is at the RHS of its parent");
                    if (subTreeWidths.Count % 2 == 0)
                    { // has even number of nodes
                        totalOffset = childMidWidth + SumBetweenWidthOfIndex(childIndex - 1, middleIndex);
                    }
                    else
                    { // has odd number of nodes
                        totalOffset = childMidWidth + midNodeHalfWidth + SumBetweenWidthOfIndex(middleIndex + 1, childIndex - 1);
                    }
                }
                childIndex++;
                //Debug.Log($"{child.gameObject.name} final X Position: {totalOffset}");
                child.localPosition = child.localPosition.ChangeAxisValue(ExtendVector3.Axis.X, totalOffset);
            }

            float SumBetweenWidthOfIndex(int a, int b)
            {
                if (a >= b) return 0;
                if (a + 1 == b) return subTreeWidths[a];
                float sum = 0;
                for (int i = a; i < b; i++)
                {
                    sum += subTreeWidths[i];
                }
                return sum;
            }
        }

        // 中心化子节点的位置，并调整父节点到子节点的中心
        public void CenterParent(Transform parentTransform)
        {
            int visualNodeCount = 0;
            // 递归调整子树
            foreach (Transform child in parentTransform)
            {
                if (!child.CompareTag("BT_VisualObj")) continue;
                visualNodeCount++;
                CenterParent(child);
            }
            if (parentTransform == null || visualNodeCount < 2) return;
            // 计算所有子节点的世界坐标范围
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            List<Vector3> childOriginPosition = new List<Vector3>();
            for (int i = 0; i< parentTransform.childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                if (!child.CompareTag("BT_VisualObj")) continue;
                // 获取子节点的世界坐标
                Vector3 childWorldPosition = child.position;
                childOriginPosition.Add(childWorldPosition);
                if (childWorldPosition.x < minX) minX = childWorldPosition.x;
                if (childWorldPosition.x > maxX) maxX = childWorldPosition.x;
                if (childWorldPosition.y < minY) minY = childWorldPosition.y;
                if (childWorldPosition.y > maxY) maxY = childWorldPosition.y;
            }

            // 计算子节点范围的中心点
            float centerX = (minX + maxX) / 2;
            float centerY = (minY + maxY) / 2;

            // 调整父节点的世界坐标到中心点
            //Vector3 parentPosition = parentTransform.position;
            //Vector3 centerAdjustment = new Vector3(centerX - parentPosition.x, centerY - parentPosition.y, 0);
            //parentTransform.position += centerAdjustment;
            parentTransform.position = parentTransform.position.ChangeAxisValue(ExtendVector3.Axis.X, centerX);
            // 递归调整子树
            int vaildChildIndex = 0;
            
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                if (!child.CompareTag("BT_VisualObj")) continue;
                child.position = childOriginPosition[vaildChildIndex];
                child.localPosition = child.localPosition.ChangeAxisValue(ExtendVector3.Axis.Z, 0);
                vaildChildIndex++;
            }
            parentTransform.localPosition = parentTransform.localPosition.ChangeAxisValue(ExtendVector3.Axis.Z, 0);
        }

        public void DestroyVisualizers(Node node)
        {
            node.OnStatusChanged -= HandleStatusChange;
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            // 判断是否为可见状态
            // 如果不是叶子节点，并且有子节点，则绘制连接线
            if (transform.childCount > 2)
            {
                // 获取节点的中心点
                Vector3 parentNodePosition = transform.position;

                // 遍历所有子节点，绘制连接线
                foreach (Transform child in transform)
                {
                    if (!child.CompareTag("BT_VisualObj")) continue;
                    // 子节点的位置
                    Vector3 childNodePosition = child.position;

                    // 绘制连接线
                    Gizmos.color = child.gameObject.activeSelf ? _connectionLineColor : _connectionLineHighlight;
                    Gizmos.DrawLine(parentNodePosition, childNodePosition);
                }
            }
        }

        void HandleStatusChange(Node.Status status)
        {
            if(gameObject.activeSelf)
                _indicator.material.color = GetStatusColor(status);
        }

        GameObject GetPrefabOfNode(Node node)
        {
            if (node is Leaf leaf) return prefabs[1];
            return prefabs[0];
        }

        Color GetStatusColor(Node.Status status)
        {
            return status switch
            {
                Node.Status.RUNNING => runningColor,
                Node.Status.SUCCESS => successColor,
                Node.Status.FAILURE => failureColor,
                _ => inactiveColor
            };
        }

        Color GetNodeTextColor(Node node)
        {
            if (node is Sequence) return SequenceColor;
            if (node is Selector) return SelectorColor;
            if (node is PrioritySelector) return PrioritySelectorColor;
            if (node is SequenceLoop) return SequenceLoopColor;
            if (node is Leaf leaf)
            {
                if(leaf.GetStrategy() is Condition) return ConditionColor;
                if(leaf.GetStrategy() is ActionStrategy) return ActionColor;
            }
            return Color.white;
        }
    }
#endif
}
