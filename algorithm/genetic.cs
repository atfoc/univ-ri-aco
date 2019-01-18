using System;
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
            this.valuesRange.Add(new Tuple<double, double>(25, 200)); //AntCount
            this.valuesRange.Add(new Tuple<double, double>(0.01, 2)); //Alpha
            this.valuesRange.Add(new Tuple<double, double>(0.01, 2)); //Beta
            this.valuesRange.Add(new Tuple<double, double>(0, 1));//pheromoneEvaporation 
            this.valuesRange.Add(new Tuple<double, double>(100, 50000)); //pheromoneConstant
            this.valuesRange.Add(new Tuple<double, double>(10, 200)); //maxIters
            this.limitMs = limitMs;
            maxLength = -1;
            this.currentGeneration = new List<Colony>();

        }
        int limitMs; 
        List<Tuple<double,double>> valuesRange;
        List<Colony> currentGeneration;
        double maxLength;
        class Colony{
            public double[] parameters{get; set;}
            public double fitness{get; set;}
            
            public Colony(double[] parameters, double fitness){
                this.parameters = parameters;
                this.fitness = fitness; 
                parameters = new double[6];
                
            }

            public Colony(double[] parameters){
                this.parameters = parameters;
            }

            public Colony(){
            }


        }

        private double fitnessFunction(double pathLength, int timeInMs){
            return (maxLength - pathLength) + (limitMs - timeInMs);
        }
    

        /*private double arrayOfFitnesses<>(int fitnessFunction()){
            foreach (int res in fitnessFunction){
                arrayOfFitnesses[i] = res;
            }
        }*/

        //Choosing 32 random parents
        private List<Colony> tournamentParents(){
            List<Colony> parents = new List<Colony>(32);
            Random rnd = new Random();
            var indices = new List<int>(Enumerable.Range(0, 100));
            for(int i = 0; i < 31; i++){
                int p = rnd.Next(0, indices.Count);
                parents[i] = this.currentGeneration[indices[p]];
                indices.RemoveAt(p);
            }
            return parents;
        }
        //Choosing winner from 32
        private Colony winnerParent(List<Colony> parents){
            // Choosing parent with best (max) fitness function
            if(parents.Count == 0){
                Console.WriteLine("Empty list!");
                Environment.Exit(1);
            }
            double max = double.MinValue;
            Colony colonyWinner = null;
            foreach(Colony parent in parents){
                if(parent.fitness > max){
                    max = parent.fitness;
                    colonyWinner = parent;
                }   
            }
            return colonyWinner;
        }

        //Tournament selection
        private List<Colony> tournamentSelection(){
            Random rnd = new Random();
            int nrNewChild = rnd.Next(25, 100);
            List<Colony> hallOfFame = new List<Colony>(nrNewChild);
            for(int i = 0; i < nrNewChild; i++){
                List<Colony> parents = tournamentParents();
                Colony winner = winnerParent(parents);
                hallOfFame[i] = winner; 
            }
            return hallOfFame;
        }

        // Taking winners from hall of fame and crossing them to make children
        private List<Colony> crossingSpecies(){
            List<Colony> winners = tournamentSelection();

            // Taking random end cross point
            Random rnd = new Random();
            var crossPoint = rnd.Next(0, 5);

            List<Colony> children = new List<Colony>(winners.Count);
            for(int i = 0; i < winners.Count; i+=2){
                for(int j = 0; j < 6; j++){
                    if(j < crossPoint){
                        children[i].parameters[j] = winners[i].parameters[j];
                        children[i+1].parameters[j] = winners[i+1].parameters[j];
                    }
                    else{
                        children[i+1].parameters[j] = winners[i].parameters[j];
                        children[i].parameters[j] = winners[i+1].parameters[j];
                    }
                }
            }
            return children;
        }


        // Swap worst parents with newly crossed children
        private void swapWorstParents(List<Colony> children){        
            List<Colony> sortedParents = this.currentGeneration.OrderBy(o => o.fitness).ToList();
            for(int i = 0 ; i < children.Count; i++){
                sortedParents[i] = children[i];
            }
            this.currentGeneration = sortedParents;
            return;
        }

        // Mutations of certain species 
        private void mutateSpecies(List<Colony> children){
            Random rnd = new Random();
            foreach(Colony child in children){
                double mutationProb = rnd.NextDouble();
                if(mutationProb <= 0.1){
                    // Mutate
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
                for(int i = 0; i < 100; i++){
                    double[] parameters = new double[6];
                    for(int j = 0; j < 6; j++){
                        parameters[j] = takeRandomValue(j);
                    }
                    this.currentGeneration.Add(new Colony(parameters));
                }
            }

        private double calculateLongestPath(LowerTriangularMatrix<double> lt){
            double pathLength = 0;
            int currNode = 0;
            List<int> unvisitedNodes = new List<int>(Enumerable.Range(0,lt.size));
            unvisitedNodes.Remove(currNode);
            while(unvisitedNodes.Count != 0){
                double max = 0;
                int next = -1;
                foreach(int node in unvisitedNodes){
                    if(lt[currNode,node] > max){
                        max = lt[currNode, node];
                        next = node;
                    }
                }
                pathLength += max;
                unvisitedNodes.Remove(currNode);
            }

            return pathLength;
        }
        private void calculateFitness(){
            LowerTriangularMatrix<double> lt = new LowerTriangularMatrix<double>(0);
            ConcurrentQueue<IterationContext> cq = new ConcurrentQueue<IterationContext>();

            lt.readFromFile("test.txt");

            this.maxLength = calculateLongestPath(lt);

            for(int i = 0 ; i < 100; i++){
                Colony tmp = currentGeneration[i];
                Stopwatch sw = new Stopwatch();
                sw.Start();
                AntColony ac = new AntColony(lt, 0, (int)tmp.parameters[0], tmp.parameters[1], tmp.parameters[2],
                    tmp.parameters[3], tmp.parameters[4], (int) tmp.parameters[5], cq);
                ac.mainLoop();
                sw.Stop();
                currentGeneration[i].fitness = fitnessFunction(ac.shrotestDistance, (int)sw.ElapsedMilliseconds);
            }
        }

        public void start(){
            int iterCount = 20;
            int iter = 0;
            init();
            for(iter = 0;iter < iterCount; ++iter){
                calculateFitness();
                List<Colony> newGeneration = new List<Colony>();
                newGeneration = tournamentSelection();
                mutateSpecies(newGeneration);
                swapWorstParents(newGeneration);
                if(iter % 10 == 0){
                    Console.WriteLine(this.currentGeneration[this.currentGeneration.Count -1].fitness);
                }
            }
        }
    }

   

}