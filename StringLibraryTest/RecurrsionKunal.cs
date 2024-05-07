using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.VisualBasic;

namespace StringLibraryTest;

[TestClass]
public class recurrsionKunal
{
    [TestMethod]
    public void fibnocchi()
    {

        var ans = fibnocchiRC(4);
    }

    public int fibnocchiRC(int n)
    {
        if (n < 2)
            return n;

        return fibnocchiRC(n - 1) + fibnocchiRC(n - 2);

    }

    [TestMethod]
    public void BinarySearch()
    {
        int[] arr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        BinarySearchRC(arr, 0, arr.Length - 1, 4);
    }

    public void BinarySearchRC(int[] arr, int s, int e, int target)
    {
        if (s >= e)
            return;

        int m = s + (e - s) / 2;

        if (arr[m] == target)
        {
            Console.WriteLine("found");
        }
        else if (arr[m] < target)
        {
            BinarySearchRC(arr, s, m - 1, target);

        }
        else if (arr[m] > target)
        {
            BinarySearchRC(arr, m + 1, e, target);

        }

    }


    [TestMethod]
    public void ReverseNumber()
    {

        string reverseNumber = string.Empty;
        string ans = string.Empty;
        ans = ReverseNumberRC(12345, reverseNumber);
    }

    public string ReverseNumberRC(int n, string reverseNumber)
    {
        if (n % 10 <= 0)
        {
            return reverseNumber;

        }

        var lastdigit = n % 10;

        reverseNumber = reverseNumber.Insert(reverseNumber.Length, lastdigit.ToString());

        return ReverseNumberRC(n / 10, reverseNumber);


    }

    [TestMethod]
    public void IfArrayisSorted()
    {


        var data = new int[] { 1, 0, 3, 4, 5, 6, 7, 7 };

        var ans = IfArrayisSortedRC(data);
    }

    public bool IfArrayisSortedRC(int[] n)
    {
        if (n.Length == 1)
            return true;

        if (n[0] > n[1])
            return false;

        return IfArrayisSortedRC(n.Skip(1).ToArray());


    }

    [TestMethod]
    public void FindAllIndexUsingAnsListInBody()
    {


        var data = new int[] { 1, 0, 3, 4, 4 };

        var ans = FindAllIndexUsingAnsListInBodyRC(data, 4, 0);
    }

    public ArrayList FindAllIndexUsingAnsListInBodyRC(int[] n, int target, int index)
    {

        var list = new ArrayList();

        if (index == n.Length)
            return list;

        if (n[index] == target)
            list.Add(index);

        var ansFromBelowCalls = FindAllIndexUsingAnsListInBodyRC(n, target, index + 1);

        list.AddRange(ansFromBelowCalls);

        return list;



    }

    [TestMethod]
    public void RemoveA()
    {

        var ans = RemoveARC("bbacca", "b", 0);
    }

    public string RemoveARC(string str, string toCompare, int index)
    {
        string ans = "";
        if (String.IsNullOrWhiteSpace(toCompare) || index == str.Length - 1)
            return ans;

        if (toCompare == "a")
            ans = ans + "";

        if (toCompare != "a")
        {
            ans = ans + toCompare;
        }

        var s = RemoveARC(str, str.Substring(index + 1, 1), index + 1);

        ans = ans + s;

        return ans;

    }
    [TestMethod]
    public void subset()
    {

        var ans = subsetRC("", "abc");
    }

    public List<string> subsetRC(string processed, string unProcessed)
    {


        if (string.IsNullOrWhiteSpace(unProcessed))
        {
            var ans = new List<string>();
            ans.AddRange(new List<string>() { processed });
            return ans;
        }

        var ch = unProcessed.Take(1).First();

        var leftans = subsetRC(processed + ch, unProcessed.Substring(1));
        var rightAns = subsetRC(processed, unProcessed.Substring(1));
        leftans.AddRange(rightAns);

        return leftans;

    }

    [TestMethod]
    public void permutation()
    {
    
        var ans = permutationRC("", "abc");
    }

    public List<string> permutationRC(string processed, string unProcessed)
    {
        var ans = new List<string>();

        if (string.IsNullOrWhiteSpace(unProcessed))
        {

            ans.AddRange(new List<string>() { processed });
            return ans;
        }

        var ch = unProcessed.Take(1).First();

        for (int i = 0; i <= processed.Length; i++)
        {
            var f = processed.Substring(0, i);
            var e = processed.Substring(i, processed.Length-i);
            ans.AddRange(permutationRC(f + ch + e, unProcessed.Substring(1)));


        }

        return ans;

    }

        [TestMethod]
    public void phonePad()
    {
        var data = new Dictionary<int,List<string>>(){
             {2, new List<string>(){"a","b","c"}},
             {3, new List<string>(){"d","e","f"}},
             {4, new List<string>(){"g","h","i"}}
             
        };
        int[] keys = new int[] {1,2};
        var ans = phonePadRC("","123");
    }

    public List<string> phonePadRC(string p,string up)
    {
        
      var ans = new List<string>();

        if (string.IsNullOrWhiteSpace(up))
        {

            ans.AddRange(new List<string>() { p });
            return ans;
        }
     
      for(int i = index;i<= data[k].Count - 1 ; i++)
      {



      }


    }



}

