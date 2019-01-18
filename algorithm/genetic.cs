using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using algorithm;


namespace GeneticNamespace {
    class GeneticAlg{
       
        public Genetic(int limitMs){
            this.valuesRange = new List<Tuple<double, double>>();
            this.valuesRange[0] = new Tuple<double, double>(25, 200); //AntCount
            this.valuesRange[1] = new Tuple<double, double>(0.01, 2); //Alpha
            this.valuesRange[2] = new Tuple<double, double>(0.01, 2); //Beta
            this.valuesRange[3] = new Tuple<double, double>(0, 1); //pheromoneEvaporation 
            this.valuesRange[4] = new Tuple<double, double>(100, 50000); //pheromoneConstant
            this.valuesRange[5] = new Tuple<double, double>(10, 200); //maxIters
            this.limitMs = limitMs;

        }
        int limitMs; 
        List<Tuple<double,double>> valuesRange;
        List<Colony> currentGeneration;

        class Colony(double[6] parameters, double fitness){
            public List<double[]> parameters = new List<double[6]>(){get; set;};
            public double fitness = {get; set;};

            Colony(double[6] parameters, double fitness){
                this.parameters = parameters;
                this.fitness = fitness; 
            }

            Colony(double[6] parameters){
                this.parameters = parameters;
            }

            Colony(){
            }


        }

        private double fitnessFunction(double pathLength, int timeInMs){
            return (maxLength - pathLength) + (limitMs - timeInMs)
        }
    

        /*private double arrayOfFitnesses<>(int fitnessFunction()){
            foreach (int res in fitnessFunction){
                arrayOfFitnesses[i] = res;
            }
        }*/

        //Choosing 32 random parents
        private List[] tournamentParents(){
            List<Colony> parents = new List<Colony>(32);
            Random rnd = new Random();
            var indices = new List<int>(Enumerable.Range(0, 100));
            for(int i = 0; i < 31; i++){
                int p = rnd.Next(0, indices.length);
                parents[i] = this.currentGeneration[indices[p]];
                indices.removeAt(p);
            }
            return parents;
        }
        //Choosing winner from 32
        private Colony winnerParent(List[] parents){
            // Choosing parent with best (max) fitness function
            if(parents.count == 0){
                Console.WriteLine("Empty list!");
                Environment.Exit(1);
            }
            double max = double.MinValue;
            Colony colonyWinner;
            foreach(Colony parent in parents){
                if(parent.fitness > max){
                    max = parent.fitness
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
                List[] parents = tournamentParents();
                Colony winner = winnerParent(parents)
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

            List<Colony> children = new List<Colony>[winners.Count];
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
                double r = rnd.NextDouble();
                if(r <= 0.1){
                    // Mutate
                    int r = rnd.Next(0, 6);
                    child.parameters[r] = takeRandomValue(r);
                }
            }
            return;
        }


    }

    private double takeRandomValue(int index){
        Random rnd = new Random();
        return rnd.NextDouble() * (valuesRange[index].Item2 - valuesRange[index].Item1) + valuesRange[index].Item1;
    }

    private void init(){
        for(int i = 0; i < 100; i++){
            double[] parameters = new double[6];
            for(j = 0; j < 6; j++){
                parameters[j] = takeRandomValue(j);
            }
            this.currentGeneration.add(new Colony(parameters));
        }
    }


    private void calculateFitness(){
        for(i = 0 ; i < 100; i++){
            Colony tmp = currentGeneration[i];
            AntColony ac = new AntColony(/*HERE GOES GRAPH*/, 0, (int)tmp.parameters[0], tmp.parameters[1], tmp.parameters[2]
                tmp.parameters[3], tmp.parameters[4], (int) tmp.parameters[5]);
        }
    }

    public void start(){

        // Add aco calculate fitness

        init();
        List<Colony> newGeneration = new List<Colony>;
        newGeneration = tournamentSelection();
        mutateSpecies(newGeneration);
        swapWorstParents(newGeneration);

        // 
    }

}