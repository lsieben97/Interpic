using System;
using System.Xml;

namespace Interpic.Utils
{
    public static class XmlUtils
    {
        /// <summary>
        /// Get an Xpath expression for the given node.
        /// </summary>
        /// <param name="node">The node to get an xpath expression for.</param>
        /// <returns>The xpath expression of the given node.</returns>
        /// <exception cref="InvalidOperationException"/> When a child node is not found in the parent's tree.
        public static string GetXPathToNode(XmlNode node)
        {
            string result = "";
            XmlNode parent = node.ParentNode;
            while (parent != null)
            {
                result = "/" + node.Name + "[" + GetNodePosition(node).ToString() + "]" + result;
                node = parent;
                parent = parent.ParentNode;
            }
            return result;
        }

        /// <summary>
        /// Get the position of the node in it's parent. 
        /// </summary>
        /// <param name="child">The node to git the position from.</param>
        /// <returns>The position of the node in it's parent.</returns>
        /// <exception cref="InvalidOperationException"/> When a child node is not found in the parent's tree.
        public static int GetNodePosition(XmlNode child)
        {
            int samenodes = 0;
            
            for (int i = 0; i < child.ParentNode.ChildNodes.Count; i++)
            {
                if (child.ParentNode.ChildNodes[i] == child)
                {
                    // tricksy XPath, not starting its positions at 0 like a normal language
                    return samenodes + 1;
                }
                else
                {
                    if (child.ParentNode.ChildNodes[i].Name == child.Name)
                    {
                        samenodes++;
                    }
                }
            }
            throw new InvalidOperationException("Child node somehow not found in its parent's ChildNodes property.");
        }
    }
}
