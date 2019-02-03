using System;
using System.Text;

public class LowerTriangularMatrix<T>
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

		public void readFromFile(string path){
		string[] lines = System.IO.File.ReadAllLines(path, Encoding.ASCII);
		string[] lines_minus_one = new string[lines.Length-1];
		int num_of_nodes = Int32.Parse(lines[0]);
		this.size = num_of_nodes;
		Console.WriteLine(num_of_nodes);
		Array.Copy(lines, 1, lines_minus_one, 0, lines.Length-1);//remove first element
		
		this.data = new T[num_of_nodes * (num_of_nodes+1)/2];
		int index = 0;//index at which we are currently enetring new value
		foreach(string line in lines_minus_one){
			string[] vals = line.Split(' ');
			foreach(string val in vals){
				if(val.Trim() == "")
					continue;
				data[index] = (T)Convert.ChangeType(val, typeof(T));
				++index;
			}
		}

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