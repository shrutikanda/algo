
using System.Collections;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Payloads;

namespace StringLibraryTest;

[TestClass]
public class Search
{
   [TestMethod]
   public void BinarySearch()
   {

      var data = new int[] { 2, 3, 4, 2, 6, 7, 89, 90 };

      var find = 900;
      int foundat = -1;
      Array.Sort(data);
      //{2,2,3,4,6,7,89,90};
      int minNum = 0;
      int maxNum = data.Length - 1;

      while (minNum <= maxNum)
      {
         if (minNum == maxNum)
         {
            if (find == data[minNum])
            {
               foundat = minNum;
               return;
            }
         }

         int mid = (minNum + maxNum) / 2;

         if (find == data[mid])
         {
            foundat = mid;
            return;
         }

         if (find < data[mid])
            maxNum = mid - 1;
         else minNum = mid + 1;

      }

      Console.WriteLine("find = " + foundat);
   }

   [TestMethod]
   public void MaxSumConsicutiveSlidingWindow()
   {
      //https://www.youtube.com/watch?v=qqXOZD4zKEg

      var data = new int[] { 7, 9, 20, 4, 9, 3, 11, 4, 3 };

      var windowSize = 2;

      int p1 = 0;
      int p2 = windowSize;
      int maxSum = 0;

      for (int i = 0; i < windowSize; i++)
         maxSum += data[i];

      int tempsum = maxSum;

      while (p2 < data.Length)
      {

         tempsum = tempsum - data[p1] + data[p2];

         if (tempsum > maxSum)
            maxSum = tempsum;

         p2++;
         p1++;

      }

      Console.WriteLine(maxSum);

   }

   [TestMethod]
   public void JumpGame()
   {
      int[] num = new int[] { 2, 3, 1, 1, 4 };
      bool canJump = true;

      for (int index = 0; index < num.Length - 1;)
      {
         if (index + num[index] < num.Length)
         {
            if (num[index] == 0)
            {
               canJump = false;
               break;
            }

            index = index + num[index];
         }

         else
         {

            canJump = false;
            break;
         }

      }

      Console.Write(canJump);

   }

   [TestMethod]
   public void MinJumpGame()
   {

      int[] num = new int[] { 2, 3, 0, 1, 4 };
      int s = 0, e = 0;

      int[] newarray = num.Skip(0).Take(1).ToArray();

      int steps = 0;
      int farthest = newarray.Max();

      if (farthest == num.Length)
      {
         Console.Write("yes" + steps);
         return;
      }

      for (int i = 1; i <= num.Length;)
      {
         steps = steps + 1;

         s = e + 1;
         e = e + farthest;

         if (s == num.Length || e == num.Length)
         {
            Console.Write("yes" + steps);
            break;

         }

         newarray = num.Skip(s).Take(e).ToArray();

         farthest = newarray.Max();


      }


   }

   [TestMethod]
   public void TwoSum()
   {

      int[] num = new int[] { 3, 2, 4 };

      var hashednums = num.ToHashSet();
      int sum = 6;

      int[] result = new int[2];

      for (int i = 0; i <= num.Length - 1; i++)
      {

         if (hashednums.TryGetValue(Math.Abs(sum - num[i]), out int r))
         {

            if (i == Array.IndexOf(num, r))
               continue;

            result[0] = i;
            result[1] = Array.IndexOf(num, r);

         }

      }

   }

    [TestMethod]
   public void LongestsubtringHash()
   {
      //https://www.youtube.com/watch?v=dvXyTOYVxB8


      //base case

     //https://www.youtube.com/watch?v=dvXyTOYVxB8

      var s = "bbbbb".ToCharArray();
      int p1 = 0, p2 = 0;
      HashSet<char> hs = new HashSet<char>();
      var result = new HashSet<char>();

      while (p2 < s.Length && p1 < s.Length)
      {

         if (!hs.TryGetValue(s[p2], out _))
         {
            hs.Add(s[p2]);

            p2++;
         }
         else
         {
            if (result.ToArray().Length < hs.ToArray().Length)
            {
               result = hs;
            }

            hs = new HashSet<char>();
            p1++;
            p2 = p1;

         }



      }

      Console.WriteLine(result.ToArray().Length);


     }
   
   [TestMethod]
   public void BinaryTree()
   {
      //https://www.kalkicode.com/insertion-in-binary-search-tree-without-recursion-in-csharp

      node? root = null;
      node? current = null;
      int[] data = new int[] { 10, 4, 3, 5, 15, 12 };

      foreach (var d in data)
      {

         var newNode = new node(d);

         if (root == null)
         {
            root = newNode;
            continue;
         }

         current = root;

         while (current!=null)
         {
           
            if (current.data >= d)
            {

               if (current.left == null)
               {
                  current.left = newNode;
                  break;
               }
               else
               {
                  current = current.left;
               }

            }

           else{

               if (current.Right == null)
               {
                  current.Right = newNode;
                  break;
               }
               else
               {
                  current = current.Right;
               }

            }

         }
      }



   }

   public class node
   {

      public node left, Right;

      public int data;
      public node(int data)
      {
         this.data = data;


      }
   }


     [TestMethod]
   public void CombinationBackTrack()
   {

     //https://www.abhinavpandey.dev/blog/backtracking-algorithms
      List<int> nums = new List<int> { 1, 2,2, 3, 4};

      List<int> subset = new List<int>();

      List<List<int> > res = new List<List<int> >();

        int index = 0;
        combination(nums, res, subset, index);
        Console.WriteLine(res);


   }

  static void combination(List<int> nums,
                           List<List<int> > res,
                           List<int> subset, int index)
    {
        // Add the current subset to the result list
       
       var sum =  subset.Sum(x=> x);

       if(sum == 5)
           res.Add(new List<int>(subset));
 
        // Generate subsets by recursively including and
        // excluding elements
        for (int i = index; i < nums.Count; i++) {
            // Include the current element in the subset
            subset.Add(nums[i]);
 
            // Recursively generate subsets with the current
            // element included
            combination(nums, res, subset, i + 1);
 
            // Exclude the current element from the subset
            // (backtracking)
            subset.RemoveAt(subset.Count - 1);
        }
    }


[TestMethod]
   public void Subsetbacktrack()
   {

//https://www.geeksforgeeks.org/backtracking-to-find-all-subsets/
//https://www.youtube.com/watch?v=aqM2K8dgVzQ
      List<int> nums = new List<int> { 1,2,3};

     List<int> subset = new List<int>();
        List<List<int> > res = new List<List<int> >();
        int index = 0;
        CalcSubset(nums, res, subset, index);
        Console.WriteLine(res);


   }

  static void CalcSubset(List<int> A,
                           List<List<int> > res,
                           List<int> subset, int index)
    {
       
 
        // Generate subsets by recursively including and
        // excluding elements
        for (int i = index; i < A.Count; i++) {
            // Include the current element in the subset
            subset.Add(A[i]);
 
            // Recursively generate subsets with the current
            // element included
            CalcSubset(A, res, subset, i + 1);

             // Add the current subset to the result list
            res.Add(new List<int>(subset));
 
            // Exclude the current element from the subset
            // (backtracking)
            subset.RemoveAt(subset.Count - 1);
        }
    }


    [TestMethod]
   public void PhoneNumber()
   {

     var digitToChar = new Dictionary<string, string>
     {
      //https://www.youtube.com/watch?v=0snEunUacZY (State tree)

       {"2", "abc"},
       {"3", "def"},
       {"4", "ghi"},
       {"5", "jkl"},
       {"6", "mno"},
       {"7", "qprs"},
       {"8", "tuv"},
       {"9", "wxyz"},
     };
     var digit = new string[]{"2","3"};
     var result = new List<List<string>>();


     backtrackphonenumber(digitToChar,digit,0, result, new List<string>());

     Console.WriteLine(result);


   }


   public void  backtrackphonenumber( Dictionary<string, string> digitToChar ,
   string[] digit,int i,List<List<string>> result,List<string> subset)
  {
      if(i >= digit.Length )
        return;
   
      foreach (var c in digitToChar[digit[i]]){
           subset.Add(c.ToString());
           backtrackphonenumber(digitToChar,digit ,i+1, result, subset); 
           result.Add(new List<string>(subset));
           subset.RemoveAt(subset.Count -1);

     }
  }

      [TestMethod]
   public void JumoGame3()
   {
       var data = new int[] {3,0,2,1,2};
        var result = jump(2,data,false);

    
   }
   public bool jump( int i ,int[] data, bool result)
  {
    
      if( i >= data.Length  || i < 0)
        return false;
       if(data[i] == 0)   
       {         
        result =  true;   
        return result;
       }

      return  jump(i+data[i],data,result)|| jump(i-data[i],data,result); 

     
      

     }




  }
  


