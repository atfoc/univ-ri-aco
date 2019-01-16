using System;

namespace LowerTriangularMatrixNamespace{
	class LowerTriangularMatrix<T>
	{
		public LowerTriangularMatrix(T[,] matrix)
		{
			this.size = matrix.GetLength(0);
			initializeData(matrix);
		} 
		public LowerTriangularMatrix(int size)
		{
			data = new T[size * (size+1)/2];
			this.size = size;
		}

		public LowerTriangularMatrix(LowerTriangularMatrix<T> other){
			this.size = other.size;
			data = new T[size * (size+1)/2];

			//deep copy of array
			for(int i = 0; i < this.size*(this.size+1)/2; ++i){
				this.data[i] = other.data[i];
			}
		}
		public LowerTriangularMatrix(int size, T[] data)
        {
            this.size = size;
			this.data = new T[size * (size+1)/2];
            data.CopyTo(this.data, 0);
        } 
		

		public bool writeToFile(String path){
			bool ret = true;
			string[] lines = new string[this.size + 1];
			lines[0] = this.size +"";
			int written = 0;
			for(int i=0;i<this.size;++i){
				int tmp = i+1;
				while(tmp != 0){
					lines[i+1] += this.data[written] + " ";
					--tmp;
					++written;
				}
			}
			System.IO.File.WriteAllLines(path, lines);
			

			return ret;
		}

		T[] data;
		public int size {get;private set;}
		private void initializeData(T[,] matrix){
			int size = matrix.GetLength(0);
			data = new T[size * (size+1)/2];//LTM has (n*n+1)/2 elements
			int i = 0, j = 0;
			for(;i<size;++i){
				for(j = 0;j<=i;++j){
					data[i*(i+1)/2 + j] = matrix[i,j];
				}
			}
		}
		public void print(){
			Console.WriteLine("size: " + this.size);
			for(int i =0;i<size;++i){
				for(int j=0;j<size;++j){
					Console.Write(this[i,j] + "  ");
				}
				Console.Write("\n");
			}
		}
		public T this[int i, int j]
		{
			get {
				//since this is LTM, swaping indices is required
				if(j > i){
					int tmp = i;
					i = j;
					j = tmp;
				}
				return data[i*(i+1)/2 + j];
			}
			set {
				//since this is LTM, swaping indices is required
				if(j > i){
					int tmp = i;
					i = j;
					j = tmp;
				}
				data[i*(i+1)/2 + j] = value;
			}
		}
	}
}