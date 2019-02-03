using System.Collections.Generic;


public class IterationContext
{
    public IterationContext(List<List<int>> antsRoutes, List<int> iterShortestPath, LowerTriangularMatrix<double> pheromoneMatrix
                    ,int currIter, int numOfIters){
        this.antsRoutes = antsRoutes;
        this.iterShortestPath = iterShortestPath;
        this.pheromoneMatrix = new LowerTriangularMatrix<double>(pheromoneMatrix);
        this.currIter = currIter;
        this.numOfIters = numOfIters;
    }
    public List<List<int>> antsRoutes;
    public List<int> iterShortestPath;
    public LowerTriangularMatrix<double> pheromoneMatrix;
    public int numOfIters;
    public int currIter{get;private set;}
}

