using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace UniversalUE4UnlockerDumpFormatter {
    class Program {
        static void Main(string[] args) {
            string outputString;
            string line;
            string type;
            string[] locationArr;
            string[] lineSplit;
            bool error = false;
            bool customparse = false;
            var data = new SortedList<string, dynamic>();
            string path = "";

            Console.WriteLine("Author: EliteHunter");
            if (args.Length > 0) {
                path = args[0];
            } else {
                Console.WriteLine("Path wasn't entered as an argument, you can drop the .txt file onto this exe.");
                Console.WriteLine("Enter file path:");
                path = Console.ReadLine();
                path = path.Replace("\"", "");
            }
            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);
            string outputPath = directory + @"\" + filename + "-Formatted";

            do {
                Console.WriteLine("1. Use JSON Formatting");
                Console.WriteLine("2. Use Custom Formatting");
                Console.WriteLine("Enter the corresponding number on the menu to use that option:");
                string input = Console.ReadLine();
                if (input.Equals("1")) {
                    customparse = false;
                    error = false;
                    outputPath += ".json";
                } else if (input.Equals("2")) {
                    customparse = true;
                    error = false;
                    outputPath += ".txt";
                } else {
                    error = true;
                    Console.WriteLine("Wrong input, try again.");
                }
            } while (error);

            StreamReader file = new StreamReader(path);
            while ((line = file.ReadLine()) != null) {
                lineSplit = line.Split(" ", 4);
                type = lineSplit[2];
                locationArr = lineSplit[3].Split(".");
                AddData(data, locationArr, type);
            }
            file.Close();

            if (customparse) {
                File.WriteAllLines(outputPath, CustomFormat(data).ToArray());
            } else {
                var options = new JsonSerializerOptions {
                    WriteIndented = true
                };
                outputString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(outputPath, outputString);
            }
        }

        static List<string> CustomFormat(SortedList<string, dynamic> data, int depth = 0) {
            List<string> output = new List<string>();
            for (int i = 0; i < data.Count; i++) {
                output.Add("");
                for (int j = 0; j < depth; j++) {
                    output[output.Count - 1] += ("    ");
                }
                output[output.Count - 1] += data.Keys[i].ToString();

                if (!data.Values[i].GetType().Equals(typeof(string))) {
                    if (data.Values[i].Count != 0) {
                        output.AddRange(CustomFormat(data.Values[i], depth + 1));
                    }
                }
            }
            return output;
        }

        static void AddData(SortedList<string, dynamic> data, string[] locationArr, string type) {
            for (int i = 0; i < locationArr.Length - 1; i++) {
                data = AddDataSub(data, locationArr[i]);
            }
            AddDataSub(data, locationArr[locationArr.Length - 1], type);
        }

        static SortedList<string, dynamic> AddDataSub(SortedList<string, dynamic> data, string location, string type = "") {
            dynamic value = new SortedList<string, dynamic>();
            if (!data.TryGetValue(location, out value)) {
                if (type != "") {
                    if (!data.TryGetValue(type, out value)) {
                        data.Add(type, new SortedList<string, dynamic>());
                    }
                    if (!data[type].TryGetValue(location, out value)) {
                        data[type].Add(location, "");
                    }
                    value = data[type];
                } else {
                    data.Add(location, new SortedList<string, dynamic>());
                    value = data[location];
                }
            }
            return value;
        }
    }
}
