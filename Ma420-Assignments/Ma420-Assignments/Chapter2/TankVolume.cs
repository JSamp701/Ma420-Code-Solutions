using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ma420_Assignments.Chapter2
{
    class TankVolume
    {
        public class OvertopException : Exception
        {
            double depth;

            public OvertopException(double depth)
            {
                this.depth = depth;
            }
        }

        public static void TestModule()
        {
            double[] depths = { 0.3, 0.8, 1.0, 2.2, 3.0, 3.1 }; 

            for(int i = 0; i < 6; ++i)
            {
                try {
                    Console.WriteLine("DEPTH: " + depths[i] + " VOLUME: " + CalculateTankVolume(1, depths[i]));
                }
                catch(OvertopException e)
                {
                    Console.WriteLine("OVERTOP! DEPTH: " + depths[i]);
                }
            }
        }

        public static double CalculateTankVolume(double radius, double depth)
        {
            double totalVolume = 0, conicalVolume = 0, cylinderVolume = 0;

            double threeRadius = 3 * radius;

            if (depth > threeRadius) throw new OvertopException(depth);

            double cylinderHeight = (depth > radius) ? depth - radius : 0;
            
            double conicalHeight = (depth > radius) ? radius : depth;

            cylinderVolume = SimpleCylinderVolume(radius, cylinderHeight);
            conicalVolume = SimpleConicalVolume(radius, conicalHeight);

            totalVolume = conicalVolume + cylinderVolume;
            return totalVolume;
        }

        public static double SimpleCylinderVolume(double radius, double height)
        {
            return Math.PI * radius * radius * height;
        }

        public static double SimpleConicalVolume(double radius, double height)
        {
            return SimpleCylinderVolume(radius, height) / 3;
        }
    }
}
