using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GenericTreeNode
{
	public class TreeNode<T> : IEnumerable<TreeNode<T>>, ITreeNode
	{
		private T _data;

		public T Data
		{
			get => _data;
			set
			{
				if (IsReadOnly == false)
				{
					_data = value;
				}
			}
		}

		public TreeNode<T> Parent { get; protected set; }

		private List<TreeNode<T>> _children;
		public IEnumerable<TreeNode<T>> Children => _children ?? Enumerable.Empty<TreeNode<T>>();
		private List<TreeNode<T>> ChildList => _children ?? (_children = new List<TreeNode<T>>());

		public bool IsRoot => Parent == null;
		public bool IsLeaf => _children is null || _children.Count == 0;
		public bool IsReadOnly { get; set; }
		public int Count => _children?.Count ?? 0;
		public int Level => this.IsRoot ? 0 : Parent.Level + 1;

		public TreeNode(T data, bool isReadOnly = false)
		{
			_data = data;
			IsReadOnly = isReadOnly;
		}

		public TreeNode<T> AddChild(T data, bool isReadOnly = false)
		{
			var node = new TreeNode<T>(data, isReadOnly);
			AddChild(node);
			return node;
		}

		public void AddChild(TreeNode<T> child) => OnAddChild(child);

		protected virtual void OnAddChild(TreeNode<T> child)
		{
			ChildList.Add(child);
			child.Parent = this;
		}

		public TreeNode<T> InsertChild(int index, T data, bool isReadOnly = false)
		{
			var node = new TreeNode<T>(data, isReadOnly);
			return InsertChild(index, node) ? node : null;
		}

		public bool InsertChild(int index, TreeNode<T> child) => OnInsertChild(index, child);

		protected virtual bool OnInsertChild(int index, TreeNode<T> child)
		{
			if (index < 0 || index >= ChildList.Count) return false;
			ChildList.Insert(index, child);
			child.Parent = this;
			return true;
		}

		public bool RemoveChild(T data)
		{
			TreeNode<T> child = Children.FirstOrDefault(c => Equals(c.Data, data));
			if (child == null) return false;
			return ChildList.Remove(child);
		}

		public bool RemoveChild(TreeNode<T> child) => OnRemoveChild(child);

		protected virtual bool OnRemoveChild(TreeNode<T> child)
		{
			child.Parent = null;
			if (_children != null && _children.Count > 0 && _children.Remove(child))
			{
				child.Parent = null;
				return true;
			}

			return false;
		}

		public override string ToString() => Data?.ToString() ?? GetType().Name;

		#region iterating

		public IEnumerable<TreeNode<T>> BreadthFirst()
		{
			yield return this;
			foreach (TreeNode<T> node in Children.SelectMany(c => c.BreadthFirst()))
			{
				yield return node;
			}
		}

		public IEnumerable<TreeNode<T>> DepthFirst()
		{
			foreach (TreeNode<T> node in Children.SelectMany(c => c.BreadthFirst()))
			{
				yield return node;
			}

			yield return this;
		}

		public IEnumerable<TreeNode<T>> GetPath()
		{
			return AsEnumerable().Union(GetPath()); // will it work?
		}

		public IEnumerable<TreeNode<T>> GetDescendents()
		{
			return this.Skip(1);
		}

		private IEnumerable<TreeNode<T>> AsEnumerable()
		{
			yield return this;
		}



		//public void Clear()
		//{
		//	_children.Clear();
		//}

		//public TreeNode<T> FindInChildren(T data)
		//{
		//	int i = 0, l = Count;
		//	for (; i < l; ++i)
		//	{
		//		TreeNode<T> child = _children[i];
		//		if (child.Data.Equals(data)) return child;
		//	}

		//	return null;
		//}


		#endregion

		#region iterating

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<TreeNode<T>> GetEnumerator()
		{
			yield return this;
			foreach (var directChild in this.Children)
			{
				foreach (var anyChild in directChild)
					yield return anyChild;
			}
		}

		#endregion

	}

	public interface ITreeNode
	{
	}

	public class SelectionTreeNode<T> : TreeNode<T>
	{
		public bool IsSelected { get; set; }
		public bool IsChildSelected { get; set; }
		public bool IsExpanded { get; set; }


		public SelectionTreeNode(T data) : base(data)
		{
		}


	}
}