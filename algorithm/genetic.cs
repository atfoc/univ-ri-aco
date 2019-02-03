using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using algorithm;
using LowerTriangularMatrixNamespace;
using IterContext;


namespace GeneticNamespace {
    class GeneticAlg{
       
        public GeneticAlg(int limitMs){
            this.valuesRange = new List<Tuple<double, double>>();
            this.valuesRange.Add(new Tuple<double, double>(25, 80)); //AntCount
            this.valuesRange.Add(new Tuple<double, double>(0.01, 10)); //Alpha
            this.valuesRange.Add(new Tuple<double, double>(0.01, 200)); //Beta
            this.valuesRange.Add(new Tuple<double, double>(0, 1));//pheromoneEvaporation 
            this.valuesRange.Add(new Tuple<double, double>(100, 10000)); //pheromoneConstant
            this.valuesRange.Add(new Tuple<double, double>(10, 50)); //maxIters
            this.limitMs = limitMs;
            this.currentGeneration = new List<Colony>();
            this.newGeneration = new List<Colony>();
            this.generationSize = 35;//TODO: change
            this.allTimeBest = new Colony();
            this.numberOfRepetitions = 3;
        }
        int limitMs; 
        List<Tuple<double,double>> valuesRange;
        List<Colony> currentGeneration;
        List<Colony> newGeneration;
        Colony allTimeBest;
        int generationSize;
        int numberOfRepetitions;
        class Colony{
            public double[] parameters{get; set;}
            public double fitness{get; set;}
            public double shorestDistance{get; set;}
            public int time {get;set;}
            public Colony(double[] parameters, double fitness, double shorestDistance, int time){
                this.parameters = parameters;
                this.fitness = fitness; 
                this.shorestDistance = shorestDistance;
                this.time = time;
            }
            public Colony(Colony other){
                this.parameters = new double[6];
                Array.Copy(other.parameters, this.parameters, 6);
                this.fitness = other.fitness; 
                this.shorestDistance = other.shorestDistance;
                this.time = other.time;
            }
            public Colony(double[] parameters){
                this.parameters = parameters;
            }

            public Colony(){
                this.parameters = new double[6];
                this.fitness = Double.MaxValue;
            }

        }

        private double fitnessFunction(double pathLength, int timeInMs){
            if(timeInMs < limitMs)
                return pathLength;
            return (pathLength) + (timeInMs - limitMs);
        }

        //Choosing 32 random parents
        private List<Colony> tournamentParents(){
            List<Colony> parents = new List<Colony>();
            Random rnd = new Random();
            var indices = new List<int>(Enumerable.Range(0, this.generationSize));
            for(int i = 0; i < 32; i++){
                int p = rnd.Next(0, indices.Count);
                parents.Add(this.currentGeneration[indices[p]]);
                indices.RemoveAt(p);
            }
            return parents;
        }
        //Choosing winner from 32
        private Colony winnerParent(List<Colony> parents){
            // Choosing parent with best (min) fitness function
            if(parents.Count == 0){
                Console.WriteLine("Empty list!");
                Environment.Exit(1);
            }
            double min = double.MaxValue;
            Colony colonyWinner = null;
            foreach(Colony parent in parents){
                if(parent.fitness < min){
                    min = parent.fitness;
                    colonyWinner = parent;
                }   
            }
            return colonyWinner;
        }

        //Tournament selection
        private List<Colony> tournamentSelection(){
            Random rnd = new Random();
            int nrNewChild;
            do{
                nrNewChild = rnd.Next(this.generationSize/4,3*this.generationSize/4);
            }while(nrNewChild%2 != 0);
            List<Colony> hallOfFame = new List<Colony>();
            for(int i = 0; i < nrNewChild; i++){
                List<Colony> parents = tournamentParents();
                Colony winner = winnerParent(parents);
                hallOfFame.Add(winner); 
            }
            return hallOfFame;
        }

        // Taking winners from hall of fame and crossing them to make children
        private void crossingSpecies(){
            List<Colony> winners = tournamentSelection();

            // Taking random end cross point
            Random rnd = new Random();
            var crossPoint = rnd.Next(0, 5);

            for(int i = 0; i < winners.Count; i+=2){
                newGeneration.Add(new Colony());
                newGeneration.Add(new Colony());
                for(int j = 0; j < 6; j++){
                    if(j < crossPoint){
                        newGeneration[i].parameters[j] = winners[i].parameters[j];
                        newGeneration[i+1].parameters[j] = winners[i+1].parameters[j];
                    }
                    else{
                        newGeneration[i+1].parameters[j] = winners[i].parameters[j];
                        newGeneration[i].parameters[j] = winners[i+1].parameters[j];
                    }
                }
            }
        }


        // Swap worst parents with newly crossed children
        private void swapWorstParents(){        
            List<Colony> sortedParents = this.currentGeneration.OrderByDescending(o => o.fitness).ToList();
            for(int i = 0 ; i < this.newGeneration.Count; i++){
                sortedParents[i] = this.newGeneration[i];
            }
            this.currentGeneration = sortedParents;
            return;
        }

        // Mutations of certain species 
        private void mutateSpecies(){
            Random rnd = new Random();
            foreach(Colony child in this.newGeneration){
                double mutationProb = rnd.NextDouble();
                if(mutationProb < 0.03){
                    // Mutate
                    Console.WriteLine("Mutation!");
                    int r = rnd.Next(0, 6);
                    child.parameters[r] = takeRandomValue(r);
                }
            }
            return;
        }

        private double takeRandomValue(int index){
            Random rnd = new Random();
            return rnd.NextDouble() * (valuesRange[index].Item2 - valuesRange[index].Item1) + valuesRange[index].Item1;
        }

        private void init(){
            for(int i = 0; i < this.generationSize; i++){
                double[] parameters = new double[6];
                for(int j = 0; j < 6; j++){
                    parameters[j] = takeRandomValue(j);
                }
                this.currentGeneration.Add(new Colony(parameters));
            }
        }


        private void threadRun(ConcurrentQueue<int> unservisedColonies, LowerTriangularMatrix<double> lt){
            
            while(!unservisedColonies.IsEmpty){
                int index;
                Queue<IterationContext> cq = new Queue<IterationContext>();//unused here
                unservisedColonies.TryDequeue(out index);

                Colony tmp = currentGeneration[index];
                Stopwatch sw = new Stopwatch();

                //since ACO is nondeterministic, we will repeat algoritam few times and take average
                double[] dists = new double[this.numberOfRepetitions];
                int[] times = new int[this.numberOfRepetitions];

                for(int i = 0 ; i < this.numberOfRepetitions; ++i){
                    sw.Start();
                    AntColony ac = new AntColony(lt, 0, (int)tmp.parameters[0], tmp.parameters[1], tmp.parameters[2],
                        tmp.parameters[3], tmp.parameters[4], (int) tmp.parameters[5], cq);
                    ac.mainLoop();
                    sw.Stop();
                    dists[i] = ac.shrotestDistance;
                    times[i] = (int)sw.ElapsedMilliseconds;
                }
                
                double averageDist = dists.Sum()/dists.Length;
                int averageTime = times.Sum()/times.Length;
                
                int time = (int)sw.ElapsedMilliseconds;
                currentGeneration[index].fitness = fitnessFunction(averageDist, averageTime);
                currentGeneration[index].shorestDistance = averageDist;
                currentGeneration[index].time = averageTime;
                // Console.WriteLine("index: " + index + " fitness: " + currentGeneration[index].fitness
                //                 +" for " + time + "ms");
            }
        }

        private void calculateFitness(LowerTriangularMatrix<double> lt){
        
            int threadNum = Environment.ProcessorCount * 2;
            ConcurrentQueue<int> unservisedColonies =
                            new ConcurrentQueue<int>(Enumerable.Range(0,currentGeneration.Count));
            
            Task[] tasks = new Task[threadNum];
            for(int i = 0; i < threadNum; ++i){
                tasks[i] = Task.Factory.StartNew(() => threadRun(unservisedColonies, lt));
            }
            Task.WaitAll(tasks);
        }

        public void start(){
            int iterCount = 200;
            int iter = 0;
            
            init();
            LowerTriangularMatrix<double> lt = new LowerTriangularMatrix<double>(0);
            lt.readFromFile("../data/cities_dist.txt");
            for(iter = 0;iter < iterCount; ++iter){
                Console.WriteLine("iteration: " + iter);
                calculateFitness(lt);
                crossingSpecies();
                mutateSpecies();
                swapWorstParents();
                if(allTimeBest.fitness > 
                                    this.currentGeneration[this.currentGeneration.Count -1].fitness){
                    //saving all time best
                    Console.WriteLine("New All time best!");
                    allTimeBest = new Colony(this.currentGeneration[this.currentGeneration.Count -1]);
                }
                Console.WriteLine("fitness: " + this.currentGeneration[this.currentGeneration.Count -1]
                                                                                            .fitness);
                Console.WriteLine("length: " + this.currentGeneration[this.currentGeneration.Count -1]
                                                                                        .shorestDistance);
                Console.WriteLine("time: " + this.currentGeneration[this.currentGeneration.Count -1].time);
                Console.WriteLine(string.Join(" ",allTimeBest.parameters));
                // if(iter % 10 == 0){
                //     Console.WriteLine("fitness: " + this.currentGeneration[this.currentGeneration.Count -1].fitness);
                // }
                this.newGeneration.Clear();
            }
            
            Console.WriteLine("fitness: " + allTimeBest.fitness);
            Console.WriteLine("path: " + allTimeBest.shorestDistance);
            Console.WriteLine("time: " + allTimeBest.time);
            string outputString = string.Join(" ",allTimeBest.parameters);
            Console.WriteLine(outputString);
            string[] lines = new string[1];
            lines.Append(outputString);
			System.IO.File.WriteAllLines("ACO_parameters.txt", lines);
        }
    }

   

}