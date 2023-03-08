using NUnit.Framework;
using GenericTreeNode;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTreeNode.Tests
{
	[TestFixture()]
	public class TreeNodeTests
	{
		private static TreeNode<string> GetTestTree()
		{
			var A = new TreeNode<string>("A");
				var B = A.AddChild("B");
					var G = B.AddChild("G");
					var H = B.AddChild("H");
						var M = H.AddChild("M");
						var N = H.AddChild("N");
					var I = B.AddChild("I");
				var C = A.AddChild("C");
			  var D = A.AddChild("D");
			  var E = A.AddChild("E");
					var J = E.AddChild("J");
					var K = E.AddChild("K");
			  var F = A.AddChild("F");
					var L = F.AddChild("L");

			return A;
		}

		private static IEnumerable<string> GetTreeData(IEnumerable<TreeNode<string>> nodes)
		{
			return nodes.Select(node => node.Data);
		}

		private static string GetTreeString(IEnumerable<TreeNode<string>> nodes)
		{
			return string.Join("", GetTreeData(nodes));
		}


		[Test]
		public static void When_ExaminingRoot_ExpectProperPlacing()
		{

			var A = GetTestTree();
			Assert.IsTrue(A.IsRoot);
			Assert.IsNull(A.Parent);
			CollectionAssert.IsNotEmpty(A.Children);
			Assert.AreEqual(A.Children.Count(), 5);
			Assert.AreEqual(A.Count, 5);
			Assert.AreEqual(A.Level, 0);

			string descendents = GetTreeString(A.GetDescendents());
			Assert.AreEqual(descendents, "BGHMNICDEJKFL");
			string all = GetTreeString(A);
			Assert.AreEqual(all, "ABGHMNICDEJKFL");
			string children = GetTreeString(A.Children);
			Assert.AreEqual(children, "BCDEF");
		}

		[Test]
		public static void When_ExaminingMiddleNode_ExpectProperPlacing()
		{
			var A = GetTestTree();

			var H = A.FirstOrDefault(n => string.Equals(n.Data, "H"));
			Assert.IsNotNull(H);
			CollectionAssert.IsNotEmpty(H.Children);
			Assert.AreEqual(H.Children.Count(), 2);
			Assert.AreEqual(H.Count, 2);
			Assert.IsFalse(H.IsRoot);
			Assert.IsFalse(H.IsLeaf);
			Assert.AreEqual(H.Level, 2);

			var B = A.FirstOrDefault(n => string.Equals(n.Data, "B"));
			Assert.AreEqual(H.Parent, B);

			string descendents = GetTreeString(H.GetDescendents());
			Assert.AreEqual(descendents, "MN");
			string all = GetTreeString(H);
			Assert.AreEqual(all, "HMN");
			string children = GetTreeString(H.Children);
			Assert.AreEqual(children, "MN");
		}

		[Test]
		public static void When_ExaminingLeafNode_ExpectProperPlacing()
		{
			var A = GetTestTree();

			var M = A.FirstOrDefault(n => string.Equals(n.Data, "M"));
			Assert.IsNotNull(M);
			CollectionAssert.IsEmpty(M.Children);
			Assert.IsFalse(M.IsRoot);
			Assert.IsTrue(M.IsLeaf);
			Assert.AreEqual(M.Level, 3);
			Assert.AreEqual(M.Children.Count(), 0);
			Assert.AreEqual(M.Count, 0);

			var H = A.FirstOrDefault(n => string.Equals(n.Data, "H"));
			Assert.AreEqual(M.Parent, H);

			string descendents = GetTreeString(M.GetDescendents());
			Assert.AreEqual(descendents, "");
			string all = GetTreeString(M);
			Assert.AreEqual(all, "M");
			string children = GetTreeString(M.Children);
			Assert.AreEqual(children, "");
		}

	}
}