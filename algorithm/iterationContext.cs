using System.Collections.Generic;
using LowerTriangularMatrixNamespace;

namespace IterContext{

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


}