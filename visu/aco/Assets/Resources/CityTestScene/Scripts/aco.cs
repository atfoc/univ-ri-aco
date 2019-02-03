using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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
            this.distanceTraveled = 0;
        }

        private int currPosition;
        public List<int> route{get;private set;}
        private List<int> notVisited;
        LowerTriangularMatrix<double> distanceMatrix;
        LowerTriangularMatrix<double> pheromoneMatrix;
        private double alpha, beta;
        public double distanceTraveled{get;private set;}
        private bool firstPass;
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
            double pheromoneAmount;
            double distance;

            Dictionary<int, double> attractivness = new Dictionary<int, double>();
            double sum = 0;
            int tmpCnt = 0;
            Random r = new Random();
            if(this.firstPass){  
                foreach(int nextPossible in this.notVisited)
                {
                    sum += 1/distanceMatrix[currPosition, nextPossible];
                    attractivness[nextPossible] = 1/distanceMatrix[currPosition, nextPossible];
                }
            }
            else{
                foreach(int nextPossible in this.notVisited)
                {
                    distance = distanceMatrix[currPosition, nextPossible];
                    pheromoneAmount = pheromoneMatrix[currPosition, nextPossible];
                    double pheromonePart = (double)Math.Pow(pheromoneAmount, alpha);
                    //Console.WriteLine("1/" + distance + ":" + Math.Pow(1/distance, beta));
                    double distancePart = (double)Math.Pow(1/distance, beta);
                    attractivness[nextPossible] = pheromonePart*distancePart;

                    sum += attractivness[nextPossible];
                    tmpCnt++;
                }
                if(sum.Equals(0)){
                    //since we are changing dict, we need to create a copy of keys
                    List<int> keys = new List<int>(attractivness.Keys);
                    foreach(var key in keys){
                        attractivness[key] = attractivness[key] + Double.Epsilon;
                    }
                    sum += keys.Count * Double.Epsilon;
                }
            }
            //ROULETE SELECTION
            double rand = (double)r.NextDouble();
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
                Queue<IterationContext> cq)
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
                    Queue<IterationContext> cq)
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
        this.running = true;
        this.sendingStep = 5;//CAREFULL WHEN CHANGING THIS, numOfIters % sendingStep == 0(this must be true!)
    }
    private LowerTriangularMatrix<double> matrix;
    private Ant[] ants;
    private Thread t;
    private bool running;
    LowerTriangularMatrix<double> pheromoneMatrix, pheromoneMatrixCopy;
    private int start, antCount, numOfIters;
    private double alpha, beta, pheromoneEvaporationCoef, pheromoneConstant;
    private bool firstPass;

    public double shrotestDistance{get;private set;}
    public List<int> shortestPath{get; private set;}
    private Queue<IterationContext> resultQueue;
    int sendingStep;
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
    public void stop(){
        running = false;
        this.t.Join();//no need to terminate, join will sufice
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

    public void begin(){
        this.t = new Thread(new ThreadStart(this.mainLoop));
        this.running = true;
        t.Start();
    }
    public void mainLoop(){
        int currIter = 0;
        double shortestIterDist = -1;//shortest path in this iteration
        List<int> shortestIterPath = null;
        List<List<int>> antsRoutes = new List<List<int>>();
        while(running && currIter < this.numOfIters){
            //Console.WriteLine($"iter: {currIter}");
            //let them loose
            foreach(Ant a in ants){
                a.start();
            }
            //they are done

            shortestIterDist = -1;//shortest path in this iteration
            shortestIterPath = null;
            antsRoutes = new List<List<int>>();
            
            foreach(Ant a in ants){
                updatePheromoneMatrixCopy(a);

                antsRoutes.Add(a.route);
                
                if(this.shrotestDistance == -1 || this.shrotestDistance > a.distanceTraveled){
                    this.shrotestDistance = a.distanceTraveled;
                    this.shortestPath = a.route;

                }
                if(shortestIterDist == -1 || shortestIterDist > a.distanceTraveled){
                    shortestIterDist = a.distanceTraveled;
                    shortestIterPath = a.route;
                }
                
            }
            updatePheromoneMatrix();
            this.firstPass = false;
            initAnts();
            this.pheromoneMatrixCopy = new LowerTriangularMatrix<double>(matrix.size);
            if(currIter % this.sendingStep == 0){
                lock(this.resultQueue){
                    this.resultQueue.Enqueue(new IterationContext(antsRoutes,shortestIterPath,
                                                pheromoneMatrix,currIter,numOfIters));
                }
            }
            ++currIter;                
        }
        //last iter
        lock(this.resultQueue){
            this.resultQueue.Enqueue(new IterationContext(antsRoutes,shortestIterPath,
                                        pheromoneMatrix,currIter-1,numOfIters));
        }
    }
}

