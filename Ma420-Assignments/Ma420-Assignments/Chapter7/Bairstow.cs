﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ma420_Assignments.Chapter7
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
            if(roots.Length - foundRootCount > 2) //are there more than 2 roots left to find?
            {
                res = performIterationStep();
            } 
            else if(roots.Length - foundRootCount == 2)
            {
                //if actually 2 roots left to find, perform quadratic solution
                res = performQuadraticSolution();
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

            double deltaS = 0;
            double deltaR = 0;
            {
                int count = currSet.Count;

                int bob = 1;

                //assign some values for convenience
                double c1 = cArray[1];
                double c2 = cArray[2];
                double c3 = cArray[3];
                double b1 = bArray[1];
                double b0 = bArray[0];
                //solve a system of two equations for deltaS and deltaR
                //  c2 * deltaR + c3 * deltaS = -b1
                //  c1 * deltaR + c2 * deltaS = -b0

                //bad solution that involved maybe dividing by 0
                //ds = ( ( -c1 * b1 / c2 ) + b0) / ( ( c1 * c3 / c2 ) - c2)
                //deltaS = (((0 - c1) * b1 / c2) + b0) / ( ( c1 * c3 / c2 ) - c2 );
                //(-b1 - c3 * ds) / c2 = dr
                //deltaR = ((0 - b1) - c3 * deltaS) / c2;
                //here we go matrices
                double one_one, one_two, one_three, two_one, two_two, two_three, store_meh;
                one_one = c2;
                one_two = c3;
                one_three = -b1;
                two_one = c1;
                two_two = c2;
                two_three = -b0;
                //[c2   c3  -b1]
                //[c1   c2  -b0]

                //from here on out, whenever i use num1 or num2 or num3 or .. in a matrix, i'm just referring to some number, not a specific number from a previous statement
                //when i use a numX in an english statement, assume im referring to the diagram directly above

                //add the bottom row to the top and the top to the bottom in hopes of getting rid of all 0s
                //add the bottom row to the top
                {
                    bool good = false;
                    double temp_one = 0, temp_two = 0, temp_three = 0, mult = 1;
                    while (!good) {
                        temp_one = one_one + two_one * mult;
                        temp_two = one_two + two_two * mult;
                        temp_three = one_three + two_three * mult;
                        good = (temp_one != 0) && (temp_two != 0);
                        ++mult;
                    }
                    one_one = temp_one;
                    one_two = temp_two;
                    one_three = temp_three;

                    good = false;
                    mult = 0;
                    while (!good)
                    {
                        temp_one = one_one * mult + two_one;
                        temp_two = one_two * mult + two_two;
                        temp_three = one_three * mult + two_three;
                        good = (temp_one != 0) && (temp_two != 0);
                        ++mult;
                    }
                    two_one = temp_one;
                    two_two = temp_two;
                    two_three = temp_three;
                }
                /*
                one_one += two_one;
                one_two += two_two;
                one_three += two_three;
                //add the top row to the bottom
                two_one += one_one;
                two_two += one_two;
                two_three += one_three;
                */

                //hopefully, every cell has something in it (otherwise I may be screwed...)
                //store one_one and divide that row by it to get
                //[1    num1    num2]
                store_meh = one_one;
                one_one = one_one / store_meh;
                one_two = one_two / store_meh;
                one_three = one_three / store_meh;

                //current state is below
                //[1    num1    num2]
                //[num3 num4    num5]

                //subtract num3 from num 3, num1 * num 3 from num4, and num2 * num3 from num5
                //yields a bottom row of
                //[0    num1    num2]
                store_meh = two_one;
                two_one = 0;
                two_two -= (store_meh * one_two);
                two_three -= (store_meh * one_three);

                //current state is below
                //[1    num1    num2]
                //[0    num3    num4]

                //divide num3 by num3 to get 1 and num4 by num3
                //yields a bottom row of
                //[0    1   num1]
                store_meh = two_two;
                two_two = 1;
                two_three = two_three / store_meh;

                //current state is below
                //[1    num1    num2]
                //[0    1       num3]

                //subtract num1 from itself and subtract (num3 * num1) from num2
                //yields a top row of
                //[1    0       num1]

                store_meh = one_two;
                one_two = 0;
                one_three -= (store_meh * two_three);

                //current (and final) state is
                //[1    0   num1]
                //[0    1   num2]

                //so, deltaR = num1 and deltaS = num2
                deltaR = one_three;
                deltaS = two_three;
            }

            //get the errors
            double sErr = (s != 0) ? Math.Abs(deltaS / s) : (deltaS != 0) ? 1 : 0;
            double rErr = (r != 0) ? Math.Abs(deltaR / r) : (deltaR != 0) ? 1 : 0;

            //assign all the relevant data
            result.iterR = r;
            result.iterS = s;
            result.iterRError = rErr;
            result.iterSError = sErr;
            result.deltaR = deltaR;
            result.deltaS = deltaS;

            

            //if latest error is beneath allowedError, calculate roots using solveRSQuadratic and store the data the roots array as well as update necessary instance variables
            //make sure to set res.rootFound to true;
            if (sErr < allowedError && rErr < allowedError)
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
            if (!result.rootFound)
                iterResults[iterResults.Count - 1].Add(result);
            else
                iterResults[iterResults.Count - 2].Add(result);

            return result;
        }

        private IterationResult performQuadraticSolution()
        {
            IterationResult result = new IterationResult();
            result.dataRelevant = false;
            result.rootFound = true;

            double[] currentEq = coefficients[coefficients.Count - 1];

            //solve for the roots and assign them
            Tuple<Tuple<double, double>, Tuple<double, double>> rootPair = solveNormalQuadratic(currentEq[2], currentEq[1], currentEq[0]);
            roots[foundRootCount++] = rootPair.Item1;
            roots[foundRootCount++] = rootPair.Item2;

            if(roots.Length - foundRootCount == 0)
            {
                result.allRootsFound = true;
            }
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

        private Tuple<Tuple<double, double>, Tuple<double,double>> solveNormalQuadratic(double a, double b, double c)
        {
            double discriminant = b * b - 4 * a * c;
            double real1, real2, complex1, complex2;
            if(discriminant > 0)
            {
                real1 = (b + Math.Sqrt(discriminant)) / (2 * a);
                real2 = (b - Math.Sqrt(discriminant)) / (2 * a);
                complex1 = 0;
                complex2 = 0;
            }
            else
            {
                real1 = b / (2 * a);
                real2 = real1;
                complex1 = Math.Sqrt(Math.Abs(discriminant)) / (2 * a);
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
