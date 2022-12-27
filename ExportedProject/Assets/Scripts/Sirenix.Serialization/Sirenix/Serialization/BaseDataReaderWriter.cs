using System;
using System.Collections.Generic;

namespace Sirenix.Serialization
{
	public abstract class BaseDataReaderWriter
	{
		private TwoWaySerializationBinder binder;

		private Stack<NodeInfo> nodes = new Stack<NodeInfo>();

		public TwoWaySerializationBinder Binder
		{
			get
			{
				if (binder == null)
				{
					binder = new DefaultSerializationBinder();
				}
				return binder;
			}
			set
			{
				binder = value;
			}
		}

		public bool IsInArrayNode
		{
			get
			{
				return CurrentNode.IsArray;
			}
		}

		protected int NodeDepth
		{
			get
			{
				return nodes.Count;
			}
		}

		protected NodeInfo CurrentNode
		{
			get
			{
				if (nodes.Count != 0)
				{
					return nodes.Peek();
				}
				return NodeInfo.Empty;
			}
		}

		protected void PushNode(NodeInfo node)
		{
			nodes.Push(node);
		}

		protected void PushNode(string name, int id, Type type)
		{
			nodes.Push(new NodeInfo(name, id, type, false));
		}

		protected void PushArray()
		{
			NodeInfo t;
			if (NodeDepth == 0 || CurrentNode.IsArray)
			{
				t = new NodeInfo(null, -1, null, true);
			}
			else
			{
				NodeInfo currentNode = CurrentNode;
				t = new NodeInfo(currentNode.Name, currentNode.Id, currentNode.Type, true);
			}
			nodes.Push(t);
		}

		protected void PopNode(string name)
		{
			if (nodes.Count == 0)
			{
				throw new InvalidOperationException("There are no nodes to pop.");
			}
			NodeInfo currentNode = CurrentNode;
			if (currentNode.Name != name)
			{
				throw new InvalidOperationException("Tried to pop node with name " + name + " but current node's name is " + currentNode.Name);
			}
			nodes.Pop();
		}

		protected void PopArray()
		{
			if (!CurrentNode.IsArray)
			{
				throw new InvalidOperationException("Was not in array when exiting array.");
			}
			nodes.Pop();
		}
	}
}
