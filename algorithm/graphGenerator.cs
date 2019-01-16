using System;
using System.Collections.Generic;
using LowerTriangularMatrixNamespace;

namespace graphGenerator
{
   
    class Generator
    {  
        public Generator(int numOfGraphs, int maxNodes, int minNodes, int maxWeight
                        ,int minWeight)
        {
            this.numOfGraphs   = numOfGraphs;
            this.maxNodes      = maxNodes;
            this.minNodes      = minNodes;
            this.maxWeight     = maxWeight;
            this.minWeight     = minWeight;
        }

        public List<LowerTriangularMatrix<double>> generate()
        {
            List<LowerTriangularMatrix<double>> graphs = new List<LowerTriangularMatrix<double>>(this.numOfGraphs);
            Random rand = new Random();
            for(int i = 0; i< numOfGraphs; ++i){
                int graphSize = rand.Next(minNodes, maxNodes);
                double[] graphData = new double[graphSize*(graphSize + 1) / 2];
                for(int j = 0;j<graphSize*(graphSize + 1) / 2;++j){
                    graphData[j] = rand.Next(minWeight, maxWeight);
                }

                graphs.Add(new LowerTriangularMatrix<double>(graphSize, graphData));
                
            }
            return graphs;
        }

        int numOfGraphs;
        int maxNodes;
        int minNodes;
        int maxWeight;
        int minWeight;
        // static void Main(string[] args)
        // {
        //     //metadata
        //     int numOfGraphs = 3;
        //     int maxNodes = 300;
        //     int minNodes = 20;
        //     int maxWeight = 30;
        //     int minWeight = 10;
        //     Generator g = new Generator(numOfGraphs, maxNodes, minNodes, maxWeight, minWeight);

        //     var graphs = g.generate(); //array of matricies of graphs
        // }
    }
}
