// namespace StringLibraryTest;


// public class Node
// {

//     public Node? Left;
//     public Node? Right;

//     public string Data;

//     public Node(string data)
//     {
//         Left = null;
//         Right = null;
//         this.Data = data;

//     }

// }

// [TestClass]
// public class UnitTest1
// {
//     [TestMethod]
//     public void TestMethod1()
//     {

//         var root = new Node("Les");

//         root.Left = new Node("Cathy");
//         root.Left.Left = new Node("Alex");
//         root.Left.Right = new Node("Frank");

//         root.Right = new Node("Sam");
//         root.Right.Left = new Node("Nancy");
//         root.Right.Right = new Node("Violet");

//         root.Right.Right.Left = new Node("Tony");
//         root.Right.Right.Right = new Node("Wendy");

//         LevelOrderTraversal(root);
//         //   inOrderTraversal(root);
//         //  PreOrderTraversal(root);
//         //  PostOrderTraversal(root);

//     }

//     public void inOrderTraversal(Node root)
//     {
//         if (root == null)
//             return;
//         inOrderTraversal(root.Left);
//         Console.Write(root.Data + ",");
//         inOrderTraversal(root.Right);
//     }

//     public void PreOrderTraversal(Node root)
//     {
//         if (root == null)
//             return;

//         Console.Write(root.Data + ",");
//         PreOrderTraversal(root.Left);
//         PreOrderTraversal(root.Right);
//     }

//     public void PostOrderTraversal(Node root)
//     {
//         if (root == null)
//             return;
//         PostOrderTraversal(root.Left);
//         PostOrderTraversal(root.Right);
//         Console.Write(root.Data + ",");
//     }

//     public void LevelOrderTraversal(Node root)
//     {
//         if (root == null)
//             return;

//         int depth = 0;
//         Queue<Node> q = new Queue<Node>();
//         q.Enqueue(root);
//         q.Enqueue(null);


//         while (q?.Count() > 0)
//         {

//             var node = q.Dequeue();

//             if (node == null)
//                 depth++;

//             if (node != null)
//             {
//                 Console.WriteLine(node.Data);

//                 if (node.Left != null)
//                     q.Enqueue(node.Left);

//                 if (node.Right != null)
//                     q.Enqueue(node.Right);
//             }


//             else if (q.Count != 0)
//                 q.Enqueue(null);

//         }
//         Console.WriteLine($"Level={depth}");

//     }

// }