using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringLibraryTest
{
    [TestClass]
    public class BubbleSort
    {
        [TestMethod]
        public void tests()
        {

            int[] data = { 12,9,8,3,1 };
            int temp;

            for (int i = 0;i < data.Length - 1; i++)
            {
                for (int j = 0; j < data.Length -i - 1; j++)
                {
                    if (data[j] > data[j +1])
                    {
                        temp = data[j];
                        data[j] = data[j + 1];
                        data[j + 1] = temp;
                    }
                }
            }
            for (int i = 0; i < data.Length - 1; i++)
            {
                Console.WriteLine(data[i]);
            }


        }
    }
}
