//TODO:
//igrati se sa parametrima
//proveriti updateovanje feromona

using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Diagnostics;
using graphGenerator;
using LowerTriangularMatrixNamespace;


namespace algorithm
{
   
    class IterationContext
    {
        public IterationContext(List<List<int>> antsRoutes, LowerTriangularMatrix<double> pheromoneMatrix
                        ,int currIter, int numOfIters){
            this.antsRoutes = antsRoutes;
            this.pheromoneMatrix = new LowerTriangularMatrix<double>(pheromoneMatrix);
            this.currIter = currIter;
            this.numOfIters = numOfIters;
        }
        private List<List<int>> antsRoutes;
        private LowerTriangularMatrix<double> pheromoneMatrix;
        private int numOfIters;
        public int currIter{get;private set;}
    }

    class AntColony
    {
        class Ant
        {
            public Ant(int start, List<int> notVisited, LowerTriangularMatrix<double> pheromoneMatrix,
                    LowerTriangularMatrix<double> distanceMatrix, double alpha, double beta, bool firstPass) 
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
            LowerTriangularMatrix<double> distanceMatrix;
            LowerTriangularMatrix<double> pheromoneMatrix;
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

                Dictionary<int, decimal> attractivness = new Dictionary<int, decimal>();
                decimal sum = 0m;
                int tmpCnt = 0;
                foreach(int nextPossible in this.notVisited)
                {
                    distance = distanceMatrix[currPosition, nextPossible];
                    pheromoneAmount = pheromoneMatrix[currPosition, nextPossible];
                    decimal pheromonePart = (decimal)Math.Pow(pheromoneAmount, alpha);
                    //Console.WriteLine("1/" + distance + ":" + Math.Pow(1/distance, beta));
                    decimal distancePart = (decimal)Math.Pow(1/distance, beta);
                    attractivness[nextPossible] =   pheromonePart*distancePart;

                    sum += attractivness[nextPossible];
                    tmpCnt++;
                }
                // foreach(var v in attractivness){
                //     Console.WriteLine(v.Key + "=>" + v.Value);
                // }
                
                
                if(sum.Equals(0m)){
                    //since we are changing dict, we need to create a copy of keys
                    List<int> keys = new List<int>(attractivness.Keys);
                    foreach(var key in keys){
                        attractivness[key] = IncrementLastDigit(attractivness[key]);
                    }
                    sum = IncrementLastDigit(sum);
                    //Console.WriteLine($"incemented sum: {sum}");
                }

                //ROULETE SELECTION
                decimal rand = (decimal)r.NextDouble();
                decimal tmpSum = 0;
                foreach(KeyValuePair<int, decimal> attr in attractivness){
                    decimal weight = attr.Value/sum;

                    if(tmpSum + weight >= rand){
                        return attr.Key;
                    }
                    tmpSum += weight;
                }

                return -1;//some error, should not happen
            }
            /*https://stackoverflow.com/questions/2237130/how-to-increase-a-decimals-smallest-fractional-part-by-one */
            //TODO: fix this function, it has malfunction, always returns 1
            private decimal IncrementLastDigit(decimal value)
            {
                //TODO: check for non 0
                int[] bits1 = decimal.GetBits(value);
                int saved = bits1[3];
                bits1[3] = 0;   // Set scaling to 0, remove sign
                int[] bits2 = decimal.GetBits(new decimal(bits1) + 1);
                bits2[3] = saved; // Restore original scaling and sign
                return new decimal(bits2);
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
                        this.matrix = new LowerTriangularMatrix<double>(matrix);
                        this.start = start;
                        this.antCount = antCount;
                        this.alpha = alpha;
                        this.beta = beta;

                        this.pheromoneMatrix = new LowerTriangularMatrix<double>(matrix.GetLength(0));
                        this.pheromoneMatrixCopy = new LowerTriangularMatrix<double>(matrix.GetLength(0));
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

        public AntColony(  LowerTriangularMatrix<double> matrix, int start, int antCount, double alpha, double beta, 
                    double pheromoneEvaporationCoef, double pheromoneConstant, int numOfIters,
                    ConcurrentQueue<IterationContext> cq)
                    {
                        this.matrix = new LowerTriangularMatrix<double>(matrix);
                        this.start = start;
                        this.antCount = antCount;
                        this.alpha = alpha;
                        this.beta = beta;

                        this.pheromoneMatrix = new LowerTriangularMatrix<double>(matrix.size);
                        this.pheromoneMatrixCopy = new LowerTriangularMatrix<double>(matrix.size);
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

        private LowerTriangularMatrix<double> matrix;
        private Ant[] ants;
        LowerTriangularMatrix<double> pheromoneMatrix, pheromoneMatrixCopy;
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
                this.pheromoneMatrixCopy = new LowerTriangularMatrix<double>(matrix.size);
                this.resultQueue.Enqueue(new IterationContext(antsRoutes,pheromoneMatrix,currIter,
                                                            numOfIters));
                ++currIter;
                //System.Threading.Thread.Sleep(100);//for testing purposes TODO: remove
                
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //testing matrix
            // double[,] matrix = new double[6,6]{ {-1,7,20,7,6,8},
            //                                     {7,-1,11,8,13,11},
            //                                     {20,11,-1,18,19,7},
            //                                     {7,8,18,-1,3,2},
            //                                     {6,13,19,3,-1,5},
            //                                     {8,11,7,2,5,-1}};
            ConcurrentQueue<IterationContext> cq = new ConcurrentQueue<IterationContext>();

            int start = 0;
            int antCount = 50;
            double alpha = .5;
            double beta  = 0.8;
            double pheromoneEvaporationCoef = .40;
            double pheromoneConstant = 500;
            int maxIters = 100;

            int numOfGraphs = 1;
            int maxNodes = 45;
            int minNodes = 10;
            int maxWeight = 3000;
            int minWeight = 500;
            Generator g = new Generator(numOfGraphs, maxNodes, minNodes, maxWeight, minWeight);

            var graphs = g.generate(); //array of matricies of graphs
            Console.WriteLine("Graphs generated");
            foreach(LowerTriangularMatrix<double> lt in graphs)
            {
                lt.writeToFile("test.txt");
                Console.WriteLine(lt.size);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                AntColony ac = new AntColony(lt,start,antCount,alpha,beta,pheromoneEvaporationCoef,
                                            pheromoneConstant ,maxIters, cq);

                Thread t = new Thread(new ThreadStart(ac.mainLoop));
                t.Start();
                // IterationContext currIterContext;
                // int currIter = 0;
                // while(currIter < maxIters - 1){
                //     while(cq.TryDequeue(out currIterContext) == false){
                //         //Console.WriteLine("Waiting");
                //     }
                //     Console.WriteLine($"New iter: {currIterContext.currIter}");
                //     currIter = currIterContext.currIter;
                // }
                t.Join();
                sw.Stop();

                var path = string.Join("=>",ac.shortestPath);
                var dist = ac.shrotestDistance;

                Console.WriteLine($"Elapsed time: {sw.Elapsed}");
                Console.WriteLine($"Shortest path has length of {dist} and it is: {path}");

            }

            
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

