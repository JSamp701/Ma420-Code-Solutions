using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ma420_Assignments.Chapter2
{
    class PolarCoordinates
    {

        public static void TestModule()
        {
            double[] xCoords = { 1, 1, 1, -1, -1, -1, 0, 0, 0 };
            double[] yCoords = { 1, -1, 0, 1, -1, 0, 1, -1, 0 };

            for(int i = 0; i < 9; ++i)
            {
                Console.WriteLine(xCoords[i] + " " + yCoords[i] + " " + ComputeRadius(xCoords[i], yCoords[i]) + " " + ComputeAngle(xCoords[i], yCoords[i]));
            }
        }

        public static double ComputeRadius(double xCoord, double yCoord)
        {
            double radius = Math.Sqrt(xCoord * xCoord + yCoord * yCoord);
            return radius;
        }

        public static double ComputeAngle(double xCoord, double yCoord)
        {
            double angle = 0;
            if(xCoord > 0)
            {
                if(yCoord != 0)
                {
                    angle = Math.Atan(yCoord / xCoord);
                }
            }
            else if (xCoord == 0)
            {
                if(yCoord > 0)
                {
                    angle = Math.PI / 2;
                }
                else if (yCoord == 0)
                {
                    angle = 0;
                }
                else
                {
                    angle = 0 - Math.PI / 2;
                }
            }
            else
            {
                if(yCoord > 0)
                {
                    angle = Math.Atan(yCoord / xCoord) + Math.PI;
                }
                else if (yCoord == 0)
                {
                    angle = Math.PI;
                }
                else
                {
                    angle = Math.Atan(yCoord / xCoord) - Math.PI;
                }
            }
            return angle;
        }
    }
}
