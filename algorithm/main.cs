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
	    static void begin(LowerTriangularMatrix<double> lt, int start, int antCount, double alpha,
                double beta,double pheromoneEvaporationCoef, double pheromoneConstant, int maxIters,
                Queue<IterationContext> cq)
        {
            AntColony ac = new AntColony(lt,start,antCount,alpha,beta,pheromoneEvaporationCoef,
                                        pheromoneConstant ,maxIters, cq);

            ac.begin();
            int currIter = 0;
            while(currIter < maxIters-1){
                IterationContext it;
                while(cq.TryDequeue(out it) == false);
                currIter = it.currIter;
                Console.WriteLine(""+it.currIter);
            }
            var path = string.Join("=>",ac.shortestPath);
            var dist = ac.shrotestDistance;
            Console.WriteLine($"Shortest path has length of {dist} and it is: {path}");
        }
		public static void Main(string[] args)
		{
            Queue<IterationContext> cq = new Queue<IterationContext>();
            int start = 0;
            int antCount = 100;
            double alpha = .27;
            double beta  = 8.8;
            double pheromoneEvaporationCoef = 0.7;
            double pheromoneConstant = 250;
            int maxIters = 100;
            
            if(args[0] == "-genarate"){
                int numOfGraphs = 1;
                int maxNodes = 20;
                int minNodes = 5;
                int maxWeight = 3000;
                int minWeight = 500;
                
                Generator g = new Generator(numOfGraphs, maxNodes, minNodes, maxWeight, minWeight);

                var graphs = g.generate(); //array of matricies of graphs

                Console.WriteLine("Graphs generated");
                foreach(LowerTriangularMatrix<double> lt in graphs)
                {
                    lt.writeToFile("test.txt");
                    Console.WriteLine(lt.size);
                    begin(lt,start, antCount, alpha, beta, pheromoneEvaporationCoef, pheromoneConstant
                    ,maxIters, cq);
                }

            }
            else if(args[0] == "-run"){
                LowerTriangularMatrix<double> lt = new LowerTriangularMatrix<double>(0);
                lt.readFromFile(args[1]);
                begin(lt,start, antCount, alpha, beta, pheromoneEvaporationCoef, pheromoneConstant
                        ,maxIters, cq);
            }
            else if(args[0] == "-genetic"){
                GeneticAlg g = new GeneticAlg(5000);
                g.start();
            }
            else{
                Console.WriteLine("Need an option besides [-generate,-genetic,-run <path>]? Add it :D");
                return;
            }
            
        }
	}

}