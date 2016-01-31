using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ma420_Assignments.Chapter2
{
    class AverageDailyTemperature
    {
        static double ANNUAL_VARIATION_FREQUENCY = (Math.PI * 2) / 365;

        static int DAY_PEAK = 205;

        public enum Areas { Miami, Yuma, Bismarck, Seatle, Boston };

        private static double[] SELECTED_MEANS = { 22.1, 23.1, 5.2, 10.6, 10.7 };

        private static double[] SELECTED_PEAKS = { 28.3, 33.6, 22.1, 17.6, 22.9 };

        public static void TestModule()
        {
            Console.WriteLine("a) " + Convert.ToString(AverageTemperatureBetweenTwoDays(Areas.Bismarck, 0, 59)));
            Console.WriteLine("b) " + Convert.ToString(AverageTemperatureBetweenTwoDays(Areas.Yuma, 180, 242)));
        }

        public static double AverageTemperatureBetweenTwoDays(Areas area, int dayOne, int dayTwo)
        {
            return AverageTemperatureBetweenTwoDays(SELECTED_MEANS[(int)area], SELECTED_PEAKS[(int)area], dayOne, dayTwo);
        }

        public static double AverageTemperatureBetweenTwoDays(double meanTemp, double peakTemp, int dayOne, int dayTwo)
        {
            double dayOneTemp = ComputeAverageDailyTemperature(meanTemp, peakTemp, dayOne);
            double dayTwoTemp = ComputeAverageDailyTemperature(meanTemp, peakTemp, dayTwo);
            return (dayOneTemp + dayTwoTemp) / 2;
        }

        private static double ComputeAverageDailyTemperature(double meanTemp, double peakTemp, int day)
        {
            double averageTemp = 0;
            averageTemp = meanTemp + (peakTemp - meanTemp) * Math.Cos(ANNUAL_VARIATION_FREQUENCY * (day - DAY_PEAK));
            return averageTemp;
        }

        
    }
}
