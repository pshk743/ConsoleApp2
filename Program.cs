using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;

namespace GeneticsProject
{
    public struct GeneticData
    {
        public string name; // protein
        public string organism;
        public string formula;
    }

    class Program
    {
        static List<GeneticData> data = new List<GeneticData>();

        static string GetFormula(string proteinName)
        {
            foreach (GeneticData item in data)
            {
                if (item.name.Equals(proteinName)) return item.formula;
            }
            return null;
        }

        static void ReadGeneticData(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            while (!reader.EndOfStream) 
            {
                string line = reader.ReadLine(); 
                string[] fragments = line.Split('\t');
                GeneticData protein;
                protein.name = fragments[0];
                protein.organism = fragments[1];
                protein.formula = fragments[2];
                data.Add(protein);
            }
            reader.Close();
        }

        static string Decoding(string formula)
        {
            string decoded = string.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                if (char.IsDigit(formula[i]))
                {
                    char letter = formula[i + 1];
                    int count = formula[i] - '0';
                    decoded += new string(letter, count);
                    i++;
                }
                else
                {
                    decoded += formula[i];
                }
            }
            return decoded;
        }

        static int Search(string amino_acid)
        {
            string decoded = Decoding(amino_acid);
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].formula.Contains(decoded)) return i; 
            }
            return -1;
        }

        static int Diff(string protein1, string protein2)
        {
            string formula1 = Decoding(GetFormula(protein1));
            string formula2 = Decoding(GetFormula(protein2));

            if (formula1 == null || formula2 == null)
            {
                return -1;
            }

            int minLength = Math.Min(formula1.Length, formula2.Length);
            int diffCount = Math.Abs(formula1.Length - formula2.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (formula1[i] != formula2[i])
                {
                    diffCount++;
                }
            }

            return diffCount;
        }

        static string Mode(string proteinName)
        {
            string formula = Decoding(GetFormula(proteinName));
            if (formula == null) return null;

            Dictionary<char, int> frequency = new Dictionary<char, int>();
            foreach (char ch in formula)
            {
                if (frequency.ContainsKey(ch)) frequency[ch]++;
                else frequency[ch] = 1;
            }

            int maxCount = 0;
            char mostCommonAminoAcid = ' ';
            foreach (var item in frequency)
            {
                if (item.Value > maxCount || (item.Value == maxCount && item.Key < mostCommonAminoAcid))
                {
                    maxCount = item.Value;
                    mostCommonAminoAcid = item.Key;
                }
            }

            return $"{mostCommonAminoAcid}\t\t{maxCount}";
        }

        static void ReadHandleCommands(string inputFilename, string outputFilename)
        {
            StreamReader reader = new StreamReader(inputFilename);
            StreamWriter writer = new StreamWriter(outputFilename);
            {
                writer.WriteLine("Ivan Peshko");
                writer.WriteLine("Genetic Searching");
                writer.WriteLine("--------------------------------------------------------------------------");

                int counter = 0; // нумерация каждой команды
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    counter++;
                    string[] command = line.Split('\t');
                    writer.WriteLine($"{counter.ToString("D3")}   {command[0]}   {string.Join("   ", command[1..])}");

                    switch (command[0])
                    {
                        case "search":
                            int index = Search(command[1]);
                            writer.WriteLine("organism\t\t\tprotein");
                            if (index != -1)
                            {
                                writer.WriteLine($"{data[index].organism}\t\t{data[index].name}");
                            }
                            else
                            {
                                writer.WriteLine("NOT FOUND");
                            }
                            break;

                        case "diff":
                            int diffCount = Diff(command[1], command[2]);
                            writer.WriteLine("amino-acids difference: ");
                            if (diffCount != -1)
                            {
                                writer.WriteLine(diffCount.ToString());
                            }
                            else
                            {
                                writer.WriteLine("MISSING");
                            }
                            break;

                        case "mode":
                            string mostCommonAminoAcid = Mode(command[1]);
                            writer.WriteLine("amino-acid occurs: ");
                            if (mostCommonAminoAcid != null)
                            {
                                writer.WriteLine(mostCommonAminoAcid);
                            }
                            else
                            {
                                writer.WriteLine("MISSING");
                            }
                            break;

                        default:
                            writer.WriteLine("Unknown command");
                            break;
                    }

                    writer.WriteLine("--------------------------------------------------------------------------");
                }
            }
            reader.Close();
            writer.Close();
        }

        static void Main(string[] args)
        {
            ReadGeneticData("sequences2.txt");
            ReadHandleCommands("commands2.txt", "genedata2.txt");
        }
    }
}
