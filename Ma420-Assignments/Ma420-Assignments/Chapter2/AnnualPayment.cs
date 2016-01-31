using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ma420_Assignments.Chapter2
{
    class AnnualPayment
    {
        public static void TestModule()
        {
            double principle = 35000;
            double interestRate = 0.15;

            for(int i = 1; i <= 5; ++i)
            {
                Console.WriteLine("N = " + i + ": " + CalculateAnnualPayment(principle, i, interestRate));
            }
        }

        public static double CalculateAnnualPayment(double principle, double numPayments, double interestRate)
        {
            double payment = 0, topPart = 0, bottomPart = 0;
            topPart = interestRate * Math.Pow(1 + interestRate, numPayments);
            bottomPart = Math.Pow(1 + interestRate, numPayments) - 1;
            payment = principle * topPart / bottomPart;
            return payment;
        }
    }
}
