using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ma420_Assignments.Chapter7
{
    class Bairstow
    {
        float[,] coefficients; //stores the coefficients for every iteration

        float allowedError; //the error distance to be under

        int maxIterNum; //the max number of iterations to perform before giving up

        public struct IterationResult
        {
            float iterR, iterS, iterError, deltaR, deltaS;
        }

        List<IterationResult>[] iterResults; //stores the results of the iterations
        public List<IterationResult>[] getIterationResults() { return iterResults; }

        int iterNum; //stores what iteration is about to be performed

        int overIterNum; //which set of coefficients to use for the current iteration;

        int foundRootCount; //how many roots have been found

        Tuple<float,float>[] roots; //the array storing the found roots, Item1 is the real component, Item2 is the imaginary component

        public Bairstow(float[] polynomial, float error, int maxIters)
        {
            coefficients = new float[polynomial.Length, polynomial.Length];
            allowedError = error;
            roots = new Tuple<float, float>[polynomial.Length];
            iterResults = new List<IterationResult>[roots.Length];
            maxIterNum = maxIters;
            foundRootCount = 0;
            iterNum = 1;
            overIterNum = 0;
        }

        public IterationResult performIteration()
        {
            IterationResult res = new IterationResult();

            return res;
        }

        public Tuple<Tuple<float, float>, Tuple<float, float>> calculateRootPair()
        {
            Tuple<Tuple<float, float>, Tuple<float, float>> results = new Tuple<Tuple<float, float>, Tuple<float, float>>(new Tuple<float, float>(0, 0), new Tuple<float, float>(0, 0));

            return results;
        }

        public Tuple<float, float>[] calculateRoots()
        {
            while(roots.Length - foundRootCount > 0)
            {
                ;//do something, calculate all the roots....
            }
            return roots;
        }

        private Tuple<Tuple<float, float>, Tuple<float,float> > solveRSQuadratic(float r, float s)
        {
            Tuple<Tuple<float, float>, Tuple<float, float>> results = new Tuple<Tuple<float, float>, Tuple<float, float>>(new Tuple<float, float>(0,0), new Tuple<float, float>(0,0));

            return results;
        }

        private Tuple<float, float> solveSingle(float a0, float a1)
        {
            return new Tuple<float, float>((0 - a0) / a1, 0);
        }

        public static void TestModule()
        {

        }
    }
}
