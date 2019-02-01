import json


def main():
	RESULT_FILEPATH = "../data/all_cities.txt"
	name_to_index = {}
	index = 0
	with open("../data/final.json") as names_json:
		names = json.load(names_json)
		
		for country in names["array"]:
			name_to_index[country["capitalName"]] = index
			index += 1

	distances = [[0 for _ in range(index)] for _ in range(index)]

	with open("../data/distances.json") as distances_json:
		info = json.load(distances_json)
		for path_info in info["array"]:
			distances[name_to_index[path_info["to"]]][name_to_index[path_info["from"]]] = path_info["distance"]
	with open(RESULT_FILEPATH, "w") as result_file:
		for (name, index) in name_to_index.items():
			result_file.write(f"{name}: {index}\n")

		result_file.write("-----------------\n")
		for i in range(len(distances)):
			for j in range(0,i+1):
				result_file.write(f"{distances[i][j]} ")
			result_file.write("\n")

if __name__ == "__main__":
	main()

