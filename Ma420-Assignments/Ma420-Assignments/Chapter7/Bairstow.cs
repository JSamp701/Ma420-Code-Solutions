using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ma420_Assignments.Chapter7
{
    class Bairstow
    {
        List<double[]> coefficients; //stores the coefficients for every iteration

        double allowedError; //the error distance to be under

        int maxIterNum; //the max number of iterations to perform before giving up

        public struct IterationResult
        {
            public double iterR, iterS, iterError, deltaR, deltaS;
            public bool dataRelevant, rootFound, allRootsFound;
        }

        List<IterationResult>[] iterResults; //stores the results of the iterations
        public List<IterationResult>[] getIterationResults() { return iterResults; }

        int iterNum; //stores what iteration is about to be performed

        int overIterNum; //which set of coefficients to use for the current iteration;

        int foundRootCount; //how many roots have been found

        Tuple<double,double>[] roots; //the array storing the found roots, Item1 is the real component, Item2 is the imaginary component

        public Bairstow(double[] polynomial) : this(polynomial, 0.001, 20){ ; }

        public Bairstow(double[] polynomial, double error) : this(polynomial, error, 20) { ; }

        public Bairstow(double[] polynomial, int maxIters) : this (polynomial, 0.001, maxIters) { ; }

        public Bairstow(double[] polynomial, double error, int maxIters)
        {
            coefficients = new List<double[]>();
            coefficients.Add(polynomial);
            allowedError = error;
            roots = new Tuple<double, double>[polynomial.Length];
            iterResults = new List<IterationResult>[roots.Length];
            maxIterNum = maxIters;
            foundRootCount = 0;
            iterNum = 1;
            overIterNum = 0;
        }

        // Performs a single iteration over the coefficients.  Returns the results of the iteration as well as stores them in the internal list
        // Will simply return the very last iteration result if all the roots have been found / no more roots need to be found via iterations
        public IterationResult performIteration()
        {
            IterationResult res = new IterationResult();
            if(roots.Length - foundRootCount >= 2) //are there 2 roots left to find?
            {
                //if actually 2 roots left to find, perform iteration and store data in the correct list using overIterNum
                ;
                //if latest error is beneath allowedError, calculate roots using solveRSQuadratic and store the data the roots array as well as update necessary instance variables
                //make sure to set res.rootFound to true;
            } 
            else if(roots.Length - foundRootCount == 1) //is there only one root left to find
            {
                //if only one root left, calculate it using the subroutine solveSingle (and store it) and set dataRelevant to false
                res.dataRelevant = false;
                res.rootFound = true;
            }
            else
            {
                //all the roots have been found, set dataRelevant to false and set allRootsFound to true;
                res.allRootsFound = true;
                res.dataRelevant = false;
            }
            if (roots.Length - foundRootCount == 0)
            {
                //make sure allRootsFound is set to true;
                res.allRootsFound = true;
            }
            return res;
        }

        // Calculate a pair of roots by going through a set of iterations
        // Returns the last 2 roots in the roots array
        // If there is only a single root, it will be stored in Item1 and Item2 will be null
        public Tuple<Tuple<double, double>, Tuple<double, double>> calculateRootPair()
        {
            Tuple<Tuple<double, double>, Tuple<double, double>> results = new Tuple<Tuple<double, double>, Tuple<double, double>>(new Tuple<double, double>(0, 0), new Tuple<double, double>(0, 0));
            //basically, call performIteration() until the result has either allRootsFound or rootFound set to true
            //make sure to check and see if the roots array has multiple values, otherwise simply assign the singular root
            return results;
        }


        public Tuple<double, double>[] calculateRoots()
        {
            while(roots.Length - foundRootCount > 0)
            {
                performIteration(); //this does the heavy lifting
            }
            return roots;
        }

        private Tuple<Tuple<double, double>, Tuple<double,double> > solveRSQuadratic(double r, double s)
        {
            Tuple<Tuple<double, double>, Tuple<double, double>> results = new Tuple<Tuple<double, double>, Tuple<double, double>>(new Tuple<double, double>(0,0), new Tuple<double, double>(0,0));

            return results;
        }

        private Tuple<double, double> solveSingle(double a0, double a1)
        {
            return new Tuple<double, double>((0 - a0) / a1, 0);
        }

        public static void TestModule()
        {

        }
    }
}
