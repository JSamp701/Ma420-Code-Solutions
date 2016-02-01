using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BairstowMethod
{
    class Bairstow
    {
        //CONSTANT VARIABLE DEFINITIONS

        const double DEFAULT_ERROR = 0.01;
        const int DEFAULT_MAX_ITERS = 30;
        const double DEFAULT_R = -1;
        const double DEFAULT_S = -1;


        //INSTANCE VARIABLE DEFINITIONS

        List<double[]> coefficients; //stores the coefficients for every iteration

        double allowedError; //the error distance to be under

        double startR, startS;

        int maxIterNum; //the max number of iterations to perform before giving up

        public struct IterationResult
        {
            public double iterR, iterS, iterRError, iterSError, deltaR, deltaS;
            public bool dataRelevant, rootFound, allRootsFound, maxIterationsExceeded;
        }

        List<List<IterationResult>> iterResults; //stores the results of the iterations
        public List<List<IterationResult>> getIterationResults() { return iterResults; }

        //int iterNum; //stores what iteration is about to be performed

        //int overIterNum; //which set of coefficients to use for the current iteration;

        int foundRootCount; //how many roots have been found

        Tuple<double, double>[] roots; //the array storing the found roots, Item1 is the real component, Item2 is the imaginary component

        public class MAX_ITER_NUM_REACHED_EXCEPTION : Exception
        {
            int bob = 1;
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        //PUBLIC METHOD DEFINITIONS

        public static void TestModule()
        {
            //textbook example, in order from least power to highest power of x
            //1.25 -3.875 2.125 2.75 -3.5 1
            double[] testArray = { 1.25, -3.875, 2.125, 2.75, -3.5, 1 };

            IterationResult result;
            Bairstow b = new Bairstow(testArray, 0.001, 30, 0,0);
            do
            {
                result = b.performIteration();
                Console.WriteLine("R: " + result.iterR + " S: " + result.iterS + " R-ERR: " + result.iterRError + " S-ERR: " + result.iterSError + " DELTA-R: " +
                    result.deltaR + " DELTA-S: " + result.deltaS);
                if (result.rootFound) Console.WriteLine();
            } while (result.allRootsFound == false);

            Tuple<double, double>[] roots = b.calculateRoots();
            for(int i = 0; i < roots.Length; ++i)
            {
                Console.WriteLine("REAL: " + roots[i].Item1 + " COMPLEX: " + roots[i].Item2);
            }
        }

        public Bairstow(double[] polynomial) : this(polynomial, DEFAULT_ERROR) { ; }

        public Bairstow(double[] polynomial, double s, double r) : this(polynomial, DEFAULT_ERROR, s, r) { ; }

        public Bairstow(double[] polynomial, double error, double s, double r) : this (polynomial, error, DEFAULT_MAX_ITERS, s, r) { ; }

        public Bairstow(double[] polynomial, double error) : this(polynomial, error, DEFAULT_MAX_ITERS) { ; }

        public Bairstow(double[] polynomial, double error, int maxIters) : this (polynomial, error, maxIters, DEFAULT_S, DEFAULT_R) { ; }

        public Bairstow(double[] polynomial, double error, int maxIters, double s, double r)
        {
            coefficients = new List<double[]>();
            coefficients.Add(polynomial);
            allowedError = error;
            roots = new Tuple<double, double>[polynomial.Length - 1];
            iterResults = new List<List<IterationResult>>();
            iterResults.Add(new List<IterationResult>());
            maxIterNum = maxIters;
            foundRootCount = 0;
            //iterNum = 1;
            //overIterNum = 0;
            startR = r;
            startS = s;
        }

        // Performs a single iteration over the coefficients.  Returns the results of the iteration as well as stores them in the internal list
        // Will simply return the very last iteration result if all the roots have been found / no more roots need to be found via iterations
        public IterationResult performIteration()
        {
            IterationResult res = new IterationResult();
            if(roots.Length - foundRootCount >= 2) //are there 2 roots left to find?
            {
                //if actually 2 roots left to find, perform iteration step
                res = performIterationStep();
            } 
            else if(roots.Length - foundRootCount == 1) //is there only one root left to find
            {
                //if only one root left, calculate it using the subroutine solveSingle (and store it) and set dataRelevant to false
                roots[foundRootCount++] = solveSingle(coefficients[coefficients.Count-1][0], coefficients[coefficients.Count - 1][1]);
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
            Tuple<Tuple<double, double>, Tuple<double, double>> results;// = new Tuple<Tuple<double, double>, Tuple<double, double>>(new Tuple<double, double>(0, 0), new Tuple<double, double>(0, 0));
            if (roots.Length - foundRootCount > 0) //are we done yet?
            {   //guess not, so we need to find the next pair of roots
                //basically, call performIteration() until the result has rootFound set to true
                IterationResult iterResult = performIteration();
                while(!iterResult.rootFound)
                {
                    iterResult = performIteration();
                }
            }

            //return the last 2 or (if there is only 1) 1 root
            if (roots.Length > 1)
                results = new Tuple<Tuple<double, double>, Tuple<double, double>>(roots[roots.Length - 2], roots[roots.Length - 1]);
            else
                results = new Tuple<Tuple<double, double>, Tuple<double, double>>(new Tuple<double, double>(0, 0), roots[roots.Length - 1]);
            
            return results;
        }

        //calculates all the roots (assuming they are not already calculated)
        public Tuple<double, double>[] calculateRoots()
        {
            while (roots.Length - foundRootCount > 0)
            {
                performIteration(); //this does the heavy lifting
            }
            return roots;
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        //PRIVATE METHOD DEFINITIONS

        private IterationResult performIterationStep()
        {
            IterationResult result = new IterationResult();
            result.dataRelevant = true;

            //set up the arrays
            double[] aArray = coefficients[coefficients.Count - 1];
            double[] bArray = new double[aArray.Length];
            double[] cArray = new double[bArray.Length];

            //set up the r and s variables
            double r = startR;
            double s = startS;
            List<IterationResult> currSet = iterResults[iterResults.Count - 1];
            if (currSet.Count != 0) //is there current information for this iteration set
            {   //if so, set up the r and s using the previous specified values
                IterationResult prev = currSet[currSet.Count - 1];//iterResults[overIterNum][iterNum - 1];
                r = prev.iterR + prev.deltaR;
                s = prev.iterS + prev.deltaS;
            }
            else if(foundRootCount > 0) //have we found any roots
            {   //if so, use the previous 2 values of r and s as good starting values of r and s
                List<IterationResult> prevSet = iterResults[iterResults.Count - 2];
                IterationResult lastOfPrev = prevSet[prevSet.Count - 1];
                r = lastOfPrev.iterR + lastOfPrev.deltaR;
                s = lastOfPrev.iterS + lastOfPrev.deltaS;
            }

            if(currSet.Count >= maxIterNum)
            {
                result.maxIterationsExceeded = true;
                throw new MAX_ITER_NUM_REACHED_EXCEPTION();
            }

            //calculate the bArray and cArray values
            bArray[bArray.Length - 1] = aArray[bArray.Length - 1];
            bArray[bArray.Length - 2] = aArray[aArray.Length - 2] + r * bArray[bArray.Length - 1];
            cArray[bArray.Length - 1] = bArray[bArray.Length - 1];
            cArray[bArray.Length - 2] = bArray[bArray.Length - 2] + r * cArray[bArray.Length - 1];
            for(int i = bArray.Length - 3; i >= 0; --i)
            {
                bArray[i] = aArray[i] + r * bArray[i + 1] + s * bArray[i + 2];
                cArray[i] = bArray[i] + r * cArray[i + 1] + s * cArray[i + 2];
            }

            //assign some values for convenience
            double c1 = cArray[1];
            double c2 = cArray[2];
            double c3 = cArray[3];
            double b1 = bArray[1];
            double b0 = bArray[0];

            //get the deltas
            double deltaS = 0;
            double deltaR = 0;
            //solve a system of two equations for deltaS and deltaR
            //  c2 * deltaR + c3 * deltaS = -b1
            //  c1 * deltaR + c2 * deltaS = -b0
            //ds = ( ( -c1 * b1 / c2 ) + b0) / ( ( c1 * c3 / c2 ) - c2)
            deltaS = (((0 - c1) * b1 / c2) + b0) / ( ( c1 * c3 / c2 ) - c2 );
            //(-b1 - c3 * ds) / c2 = dr
            deltaR = ((0 - b1) - c3 * deltaS) / c2;

            //get the errors
            double sErr = Math.Abs(deltaS / s);
            double rErr = Math.Abs(deltaR / r);

            //if latest error is beneath allowedError, calculate roots using solveRSQuadratic and store the data the roots array as well as update necessary instance variables
            //make sure to set res.rootFound to true;
            if(sErr < allowedError && rErr < allowedError)
            {
                result.rootFound = true; //make sure the result knows it found a root

                //solve for the roots and assign them
                Tuple<Tuple<double, double>, Tuple<double, double>> rootPair = solveRSQuadratic(r, s);
                roots[foundRootCount++] = rootPair.Item1;
                roots[foundRootCount++] = rootPair.Item2;

                //if there are more roots to be found, add another iterationresults list
                if(roots.Length - foundRootCount > 0)
                {
                    iterResults.Add(new List<IterationResult>());

                    //calculate / assign the next set of coefficients
                    //supposedly, the next set of coefficients belong in b
                    double[] nextSet = new double[aArray.Length - 2];
                    for(int i = 0; i < nextSet.Length; ++i)
                    {
                        nextSet[i] = bArray[i + 2];
                    }
                    coefficients.Add(nextSet);
                }
                else
                {
                    result.allRootsFound = true;
                }
            }

            //assign all the relevant data
            result.iterR = r;
            result.iterS = s;
            result.iterRError = rErr;
            result.iterSError = sErr;
            result.deltaR = deltaR;
            result.deltaS = deltaS;

            iterResults[iterResults.Count - 1].Add(result);
            return result;
        }

        private Tuple<Tuple<double, double>, Tuple<double,double> > solveRSQuadratic(double r, double s)
        {
            double discriminant = r * r + 4 * s;
            double real1, real2, complex1, complex2;
            if(discriminant > 0)
            {
                real1 = (r + Math.Sqrt(discriminant)) / 2;
                real2 = (r - Math.Sqrt(discriminant)) / 2;
                complex1 = 0;
                complex2 = 0;
            }
            else
            {
                real1 = r / 2;
                real2 = real1;
                complex1 = Math.Sqrt(Math.Abs(discriminant)) / 2;
                complex2 = 0 - complex1;
            }
            Tuple<Tuple<double, double>, Tuple<double, double>> results;
            results = new Tuple<Tuple<double, double>, Tuple<double, double>>(new Tuple<double, double>(real1, complex1), new Tuple<double, double>(real2, complex2));
            return results;
        }

        private Tuple<double, double> solveSingle(double a0, double a1)
        {
            return new Tuple<double, double>((0 - a0) / a1, 0);
        }
    }
}
