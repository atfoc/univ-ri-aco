using System.Collections.Generic;
using LowerTriangularMatrixNamespace;

namespace IterContext{

    class IterationContext
    {
        public IterationContext(List<List<int>> antsRoutes, List<int> iterShortestPath,
                            LowerTriangularMatrix<double> pheromoneMatrix ,int currIter,
                            int numOfIters)
        {
            this.antsRoutes = antsRoutes;
            this.iterShortestPath = iterShortestPath;
            this.pheromoneMatrix = new LowerTriangularMatrix<double>(pheromoneMatrix);
            this.currIter = currIter;
            this.numOfIters = numOfIters;
        }
        private List<List<int>> antsRoutes;
        List<int> iterShortestPath;
        private LowerTriangularMatrix<double> pheromoneMatrix;
        private int numOfIters;
        public int currIter{get;private set;}
    }


}