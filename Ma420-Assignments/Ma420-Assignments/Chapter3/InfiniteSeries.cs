using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ma420_Assignments.Chapter3
{
    class InfiniteSeries
    {
        public abstract class SumEquation
        {
            protected double internalSum = 0;

            protected abstract void addNum(int num);

            public double SeriesSum(int firstNum, int lastNum, int increment)
            {
                double result = 0;
                for (int i = firstNum; i != lastNum + increment; i += increment)
                {
                    this.addNum(i);
                }
                return this.internalSum;
            }
        }

        private class ThreeFourEquation : SumEquation
        {
            protected override void addNum(int num)
            {
                double modifier = (1) / ((double)(num * num));
                internalSum += modifier;
            }
        }

        public static void TestModule()
        {
            ThreeFourEquation ascending = new ThreeFourEquation();
            ThreeFourEquation descending = new ThreeFourEquation();
            Console.WriteLine("Ascending: " + ascending.SeriesSum(1, 10000, 1));
            Console.WriteLine("Descending: " + descending.SeriesSum(10000, 1, -1));
        }
    }
}
