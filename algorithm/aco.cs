//TODO:
//igrati se sa parametrima
//proveriti updateovanje feromona

using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace algorithm
{

    class IterationContext
    {
        public IterationContext(List<List<int>> antsRoutes, LowerTriangularMatrix pheromoneMatrix
                        ,int currIter, int numOfIters){
            this.antsRoutes = antsRoutes;
            this.pheromoneMatrix = new LowerTriangularMatrix(pheromoneMatrix);
            this.currIter = currIter;
            this.numOfIters = numOfIters;
        }
        private List<List<int>> antsRoutes;
        private LowerTriangularMatrix pheromoneMatrix;
        private int numOfIters;
        public int currIter{get;private set;}
    }
    class LowerTriangularMatrix
    {
        public LowerTriangularMatrix(double[,] matrix)
        {
            this.size = matrix.GetLength(0);
            initializeData(matrix);
        } 
        public LowerTriangularMatrix(int size)
        {
            data = new double[size * (size+1)/2];
            this.size = size;
        }

        public LowerTriangularMatrix(LowerTriangularMatrix other){
            this.size = other.size;
            data = new double[size * (size+1)/2];

            //deep copy of array
            for(int i = 0; i < this.size; ++i){
                this.data[i] = other.data[i];
            }
        }
        double[] data;
        public int size {get;private set;}
        private void initializeData(double[,] matrix){
            int size = matrix.GetLength(0);
            data = new double[size * (size+1)/2];//LTM has (n*n+1)/2 elements
            int i = 0, j = 0;
            for(;i<size;++i){
                for(j = 0;j<=i;++j){
                    data[i*(i+1)/2 + j] = matrix[i,j];
                }
            }
        }
        public double this[int i, int j]
        {
            get {
                if(j > i){
                    int tmp = i;
                    i = j;
                    j = tmp;
                }
                return data[i*(i+1)/2 + j];
            }
            set {
                if(j > i){
                    int tmp = i;
                    i = j;
                    j = tmp;
                }
                data[i*(i+1)/2 + j] = value;
            }
        }
    }

    class AntColony
    {
        class Ant
        {
            //public Ant(){}
            public Ant(int start, List<int> notVisited, LowerTriangularMatrix pheromoneMatrix,
                    LowerTriangularMatrix distanceMatrix, double alpha, double beta, bool firstPass) 
            {
                this.notVisited = notVisited;
                this.pheromoneMatrix = pheromoneMatrix;
                this.distanceMatrix = distanceMatrix;
                this.alpha = alpha;
                this.beta = beta;
                this.firstPass = firstPass;
                this.route = new List<int>();
                updateRoute(start);
                this.done = false;
                this.distanceTraveled = 0;
            }

            private int currPosition;
            public List<int> route{get;private set;}
            private List<int> notVisited;
            LowerTriangularMatrix distanceMatrix;
            LowerTriangularMatrix pheromoneMatrix;
            private double alpha, beta;
            public double distanceTraveled{get;private set;}
            private bool firstPass, done;

            

            private void updateRoute(int newNode) 
            {
                this.route.Add(newNode);
                this.notVisited.Remove(newNode);
                
            }
            public void start()
            {
                while(this.notVisited.Count != 0){
                    int next = pickPath();
                   
                    if(next == -1){
                        System.Console.WriteLine("ERROR!!! NEXT NODE IS -1!");
                        System.Environment.Exit(1);
                    }
                    traverse(currPosition, next);
                }
            }

            private int pickPath()
            {
                Random r = new Random();
                if(this.firstPass){  
                    int index = r.Next(notVisited.Count);
                    return notVisited[index];
                }
                
                double pheromoneAmount;
                double distance;

                Dictionary<int, double> attractivness = new Dictionary<int, double>();
                double sum = 0.0;

                foreach(int nextPossible in this.notVisited)
                {
                    distance = distanceMatrix[currPosition, nextPossible];
                    pheromoneAmount = pheromoneMatrix[currPosition, nextPossible];

                    attractivness[nextPossible] =   Math.Pow(pheromoneAmount, alpha)
                                                    *Math.Pow(1/distance, beta);

                    sum += attractivness[nextPossible];
                }
                //TODO: check if you should check if sum is 0

                //ROULETE SELECTION
                double rand = r.NextDouble();
                double tmpSum = 0;
                foreach(KeyValuePair<int, double> attr in attractivness){
                    double weight = attr.Value/sum;

                    if(tmpSum + weight >= rand){
                        return attr.Key;
                    }
                    tmpSum += weight;
                }

                return -1;//some error, should not happen
            }

            private void traverse(int curr, int next)
            {
                updateRoute(next);
                distanceTraveled += distanceMatrix[curr, next];
                this.currPosition = next;
            }

        }
/*****************************************ANT COLONY CLASS*****************************************/
        public AntColony(  double[,] matrix, int start, int antCount, double alpha, double beta, 
                    double pheromoneEvaporationCoef, double pheromoneConstant, int numOfIters,
                    ConcurrentQueue<IterationContext> cq)
                    {
                        this.matrix = new LowerTriangularMatrix(matrix);
                        this.start = start;
                        this.antCount = antCount;
                        this.alpha = alpha;
                        this.beta = beta;

                        this.pheromoneMatrix = new LowerTriangularMatrix(matrix.GetLength(0));
                        this.pheromoneMatrixCopy = new LowerTriangularMatrix(matrix.GetLength(0));
                        this.pheromoneEvaporationCoef = pheromoneEvaporationCoef;
                        this.pheromoneConstant = pheromoneConstant;
                        this.numOfIters = numOfIters;
                        ants = new Ant[antCount];
                        this.firstPass = true;
                        this.shortestPath = null;
                        this.shrotestDistance = -1;
                        initAnts();
                        this.resultQueue = cq;
                    }

        private LowerTriangularMatrix matrix;
        private Ant[] ants;
        LowerTriangularMatrix pheromoneMatrix, pheromoneMatrixCopy;
        private int start, antCount, numOfIters;
        private double alpha, beta, pheromoneEvaporationCoef, pheromoneConstant;
        private bool firstPass;

        public double shrotestDistance{get;private set;}
        public List<int> shortestPath{get; private set;}
        private ConcurrentQueue<IterationContext> resultQueue;

        private void initAnts(){
            for(int i = 0; i<antCount;++i ){
                ants[i] = new Ant(start, new List<int>(Enumerable.Range(0, matrix.size)),
                            pheromoneMatrix, matrix, alpha, beta, firstPass);
            }
        }

        private void updatePheromoneMatrixCopy(Ant a)
        {
            List<int> r = a.route;

            for(int i = 0; i < r.Count - 1; ++i){
                double currPheromoneValue = pheromoneMatrixCopy[r[i],r[i+1]];
                double newPheromoneValue = this.pheromoneConstant/a.distanceTraveled;
                pheromoneMatrixCopy[r[i],r[i+1]] = currPheromoneValue + newPheromoneValue;
                
            }
        }
        
        private void updatePheromoneMatrix()
        {
            int n = pheromoneMatrix.size;
            for(int i = 0; i < n; ++i){
                for(int j = 0; j < n; ++j){
                    pheromoneMatrix[i,j] = (1-this.pheromoneEvaporationCoef) * pheromoneMatrix[i,j];
                    pheromoneMatrix[i,j] += pheromoneMatrixCopy[i,j];
                }
            }
        }

        public void mainLoop(){
            int currIter = 0;

            while(currIter < this.numOfIters){
                //Console.WriteLine($"iter: {currIter}");
                //let them loose
                foreach(Ant a in ants){
                    a.start();
                }
                List<List<int>> antsRoutes = new List<List<int>>();
                //they are done
                foreach(Ant a in ants){
                    updatePheromoneMatrixCopy(a);

                    antsRoutes.Append(a.route);

                    if(this.shrotestDistance == -1){
                        this.shrotestDistance = a.distanceTraveled;
                    }

                    if(this.shortestPath == null){
                        this.shortestPath = a.route;
                    }

                    if(this.shrotestDistance > a.distanceTraveled){
                        this.shrotestDistance = a.distanceTraveled;
                        this.shortestPath = a.route;
                    }
                    
                }
                updatePheromoneMatrix();
                this.firstPass = false;
                initAnts();
                this.pheromoneMatrixCopy = new LowerTriangularMatrix(matrix.size);
                this.resultQueue.Enqueue(new IterationContext(antsRoutes,pheromoneMatrix,currIter,
                                                            numOfIters));
                ++currIter;
                System.Threading.Thread.Sleep(100);//for testing purposes TODO: remove
                
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //testing matrix
            double[,] matrix = new double[6,6]{ {-1,7,20,7,6,8},
                                                {7,-1,11,8,13,11},
                                                {20,11,-1,18,19,7},
                                                {7,8,18,-1,3,2},
                                                {6,13,19,3,-1,5},
                                                {8,11,7,2,5,-1}};
            ConcurrentQueue<IterationContext> cq = new ConcurrentQueue<IterationContext>();

            int start = 0;
            int antCount = 50;
            double alpha = .5;
            double beta  = 1.2;
            double pheromoneEvaporationCoef = .40;
            double pheromoneConstant = 1000;
            int maxIters = 80;

        
            AntColony ac = new AntColony(matrix,start,antCount,alpha,beta,pheromoneEvaporationCoef,
                                        pheromoneConstant ,maxIters, cq);

            Thread t = new Thread(new ThreadStart(ac.mainLoop));
            t.Start();
            IterationContext currIterContext;
            int currIter = 0;
            while(currIter < maxIters - 1){
                while(cq.TryDequeue(out currIterContext) == false){
                    //Console.WriteLine("Waiting");
                }
                Console.WriteLine($"New iter: {currIterContext.currIter}");
                currIter = currIterContext.currIter;
            }
            t.Join();

            var path = string.Join("=>",ac.shortestPath);
            var dist = ac.shrotestDistance;
        

            Console.WriteLine($"Shortest path has length of {dist} and it is: {path}");
            

            
            // foreach(int node in path){
            //     Console.Write($"{node + 1}=>");
            // }
            // LowerTriangularMatrix ltm = new LowerTriangularMatrix(matrix);
            // Console.WriteLine(ltm[0,0]);
            // Console.WriteLine(ltm[1,0]);
            // Console.WriteLine(ltm[0,1]);
            // Console.WriteLine(ltm[5,4]);
            
        }
    }
}

