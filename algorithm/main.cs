using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using IterContext;
using graphGenerator;
using LowerTriangularMatrixNamespace;
using System.Threading;
using System.Diagnostics;
using algorithm;
using GeneticNamespace;

namespace main{
	class Program{
	    static void begin(LowerTriangularMatrix<double> lt, int start, int antCount, double alpha, double beta,
            double pheromoneEvaporationCoef, double pheromoneConstant, int maxIters,
            ConcurrentQueue<IterationContext> cq)
        {

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    AntColony ac = new AntColony(lt,start,antCount,alpha,beta,pheromoneEvaporationCoef,
                                                pheromoneConstant ,maxIters, cq);

                    Thread t = new Thread(new ThreadStart(ac.mainLoop));
                    t.Start();
                    t.Join();
                    sw.Stop();

                    var path = string.Join("=>",ac.shortestPath);
                    var dist = ac.shrotestDistance;

                    Console.WriteLine($"Elapsed time: {sw.Elapsed}");
                    Console.WriteLine($"Shortest path has length of {dist} and it is: {path}");
        }
		public static void Main(string[] args)
		{
            //testing matrix
            // double[,] matrix = new double[6,6]{ {-1,7,20,7,6,8},
            //                                     {7,-1,11,8,13,11},
            //                                     {20,11,-1,18,19,7},
            //                                     {7,8,18,-1,3,2},
            //                                     {6,13,19,3,-1,5},
            //                                     {8,11,7,2,5,-1}};
            ConcurrentQueue<IterationContext> cq = new ConcurrentQueue<IterationContext>();
            // int start = 0;
            // int antCount = 50;
            // double alpha = 0.4;
            // double beta  = 0.87;
            // double pheromoneEvaporationCoef = .90;
            // double pheromoneConstant = 10000;
            // int maxIters = 100;

            GeneticAlg g = new GeneticAlg(5000);
            g.start();

            // if(args[0] == "-g"){
            //     int numOfGraphs = 1;
            //     int maxNodes = 45;
            //     int minNodes = 3;
            //     int maxWeight = 3000;
            //     int minWeight = 500;
                
            //     Generator g = new Generator(numOfGraphs, maxNodes, minNodes, maxWeight, minWeight);

            //     var graphs = g.generate(); //array of matricies of graphs

            //     Console.WriteLine("Graphs generated");
            //     foreach(LowerTriangularMatrix<double> lt in graphs)
            //     {
            //         lt.writeToFile("test.txt");
            //         Console.WriteLine(lt.size);
            //         begin(lt,start, antCount, alpha, beta, pheromoneEvaporationCoef, pheromoneConstant
            //         ,maxIters, cq);
            //     }

            // }
            // else if(args[0] == "-r"){
            //     LowerTriangularMatrix<double> lt = new LowerTriangularMatrix<double>(0);
            //     lt.readFromFile(args[1]);
            //     begin(lt,start, antCount, alpha, beta, pheromoneEvaporationCoef, pheromoneConstant
            //     ,maxIters, cq);
            // }
            // else{
            //     Console.WriteLine("Need an option besides [-g,-r <path>]? Add it :D");
            //     return;
            // }
            
        }
	}

}