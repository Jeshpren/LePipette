

using LePipette.Classes;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace LePipette.Helper
{
    internal class Helper
    {
        internal static Experiment[] GenerateExperimentArrayNEW(InputParams inputParams)
        {
            (int plateRows, int plateCols) = CalcuateRowsCols(inputParams.plateSize);

            //create the experiment array
            int nOfExperiments = inputParams.nOfReplicates.Length;
            Experiment[] experimentArray = new Experiment[nOfExperiments];
            for (int i = 0; i < nOfExperiments; i++)
            {
                int experimentRows = inputParams.sampleNames[i].Length;
                int experimentCols = inputParams.reagentNames[i].Length * inputParams.nOfReplicates[i];
                experimentArray[i] = new Experiment(new Well[experimentRows, experimentCols]);
                for (int j = 0; j < inputParams.reagentNames[i].Length; j++)
                {
                    for (int k = 0; k < inputParams.sampleNames[i].Length; k++)
                    {
                        int replicate = 0;
                        for (int l = 0; l < inputParams.nOfReplicates[i]; l++)
                        {
                            experimentArray[i].wells[k, replicate + j * inputParams.nOfReplicates[i]] = new Well(inputParams.sampleNames[i][k], inputParams.reagentNames[i][j]);
                            replicate++;
                        }
                    }
                }
            }

            // check for reagent duplicates in the whole input
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            List<string> allReagents = new List<string>();
            for (int i = 0; i < inputParams.reagentNames.Length; i++)
            {
                for (int j = 0; j < inputParams.reagentNames[i].Length; j++)
                {
                    if (allReagents.Contains(inputParams.reagentNames[i][j]))
                        Console.WriteLine("WARNING: Duplicate reagent found: " + inputParams.reagentNames[i][j]);
                    allReagents.Add(inputParams.reagentNames[i][j]);
                }
            }

            // check for sample duplicates in every experiment
            for (int i = 0; i < inputParams.sampleNames.Length; i++)
            {
                List<string> samplesInCurrentExp = new List<string>();
                for (int j = 0; j < inputParams.sampleNames[i].Length; j++)
                {
                    if (samplesInCurrentExp.Contains(inputParams.sampleNames[i][j]))
                        Console.WriteLine("WARNING: Duplicate sample found inside experiment " + (i + 1) + ": " + inputParams.sampleNames[i][j]);
                    samplesInCurrentExp.Add(inputParams.sampleNames[i][j]);
                }
            }

            Console.ForegroundColor = defaultColor;

            //split by height
            List<Experiment> experimentListByRows = new List<Experiment>();
            for (int i = 0; i < experimentArray.Length; i++)
            {
                int experimentRows = experimentArray[i].wells.GetLength(0);
                int experimentCols = experimentArray[i].wells.GetLength(1);
                if (experimentRows > plateRows)
                {
                    // cut (multiple times if needed)
                    int nOfCuts = (int)MathF.Ceiling(experimentRows / (float)plateRows);

                    for (int j = 0; j < nOfCuts; j++)
                    {
                        int nOfRowsToCut = plateRows;
                        if (j == nOfCuts - 1)
                            nOfRowsToCut = experimentRows - j * plateRows;

                        Experiment experiment = new Experiment(new Well[nOfRowsToCut, experimentCols]);
                        for (int rowIdx = 0; rowIdx < nOfRowsToCut; rowIdx++)
                        {
                            for (int colIdx = 0; colIdx < experimentCols; colIdx++)
                            {
                                experiment.wells[rowIdx, colIdx] = experimentArray[i].wells[rowIdx + j * plateRows, colIdx];
                            }
                        }
                        experimentListByRows.Add(experiment);
                    }

                }
                else
                {
                    experimentListByRows.Add(experimentArray[i]);
                }
            }

            //split by width
            List<Experiment> experimentListByCols = new List<Experiment>();
            for (int i = 0; i < experimentListByRows.Count; i++)
            {
                int experimentRows = experimentListByRows[i].wells.GetLength(0);
                int experimentCols = experimentListByRows[i].wells.GetLength(1);
                if (experimentCols > plateCols)
                {
                    // cut (multiple times if needed)
                    int nOfCuts = (int)MathF.Ceiling(experimentCols / (float)plateCols);

                    for (int j = 0; j < nOfCuts; j++)
                    {
                        int nOfColsToCut = plateCols;
                        if (j == nOfCuts - 1)
                            nOfColsToCut = experimentCols - j * plateCols;

                        Experiment experiment = new Experiment(new Well[experimentRows, nOfColsToCut]);
                        for (int rowIdx = 0; rowIdx < experimentRows; rowIdx++)
                        {
                            for (int colIdx = 0; colIdx < nOfColsToCut; colIdx++)
                            {
                                experiment.wells[rowIdx, colIdx] = experimentListByRows[i].wells[rowIdx, colIdx + j * plateCols];
                            }
                        }
                        experimentListByCols.Add(experiment);
                    }

                }
                else
                {
                    experimentListByCols.Add(experimentListByRows[i]);
                }
            }

            //return experimentArray;
            return experimentListByCols.ToArray();
        }
        internal static void PrintExperimentArray(Experiment[] experimentArray)
        {
            for (int i = 0; i < experimentArray.Length; i++)
            {
                int experimentRows = experimentArray[i].wells.GetLength(0);
                int experimentCols = experimentArray[i].wells.GetLength(1);
                string[] stringsToPrint = new string[experimentArray[i].wells.GetLength(0)];
                for (int j = 0; j < experimentRows; j++)
                {
                    for (int k = 0; k < experimentCols; k++)
                    {
                        string sample = experimentArray[i].wells[j, k].sample;
                        string reagent = experimentArray[i].wells[j, k].reagent;
                        stringsToPrint[j] += "[" + sample + "-" + reagent + "] ";
                    }
                    Console.WriteLine(stringsToPrint[j]);
                }
                Console.WriteLine("");
            }
        }
        internal static void PrintPlates(Well[][,] plates, int plateRows, int plateCols)
        {
            for (int i = 0; i < plates.Length; i++)
            {
                Console.WriteLine("\nPLATE" + (i + 1) + ":");
                Console.WriteLine("--------------------------------------------");
                string[] stringsToPrint = new string[plateRows];
                for (int j = 0; j < plateRows; j++)
                {
                    for (int k = 0; k < plateCols; k++)
                    {
                        if (plates[i][j, k] != null)
                            stringsToPrint[j] += "[" + plates[i][j, k].sample + "-" + plates[i][j, k].reagent + "]";
                        else
                            stringsToPrint[j] += "[       ]";
                    }
                    Console.WriteLine(stringsToPrint[j]);
                }
            }
        }
        internal static (int, int) CalcuateRowsCols(int plateSize)
        {
            int plateRows = 0;
            int plateCols = 0;
            if (plateSize == 96)
            {
                plateRows = 8;
                plateCols = 12;
            }
            else if (plateSize == 384)
            {
                plateRows = 16;
                plateCols = 24;
            }
            else
            {
                Console.WriteLine("error: valid values for plateSize are 96 and 384");
                Environment.Exit(0);
            }
            return (plateRows, plateCols);
        }
        internal static void CalculateLowerBound(InputParams inputParams, Experiment[] experiments)
        {
            // lower bound
            int experimentsSize = 0;
            foreach (Experiment experiment in experiments)
                experimentsSize += experiment.wells.Length;
            int lowerBound = (int)MathF.Ceiling(experimentsSize / (float)inputParams.plateSize);
            Console.WriteLine("PlateSize: " + inputParams.plateSize + ", ExperimentsSize: " + experimentsSize + ", LowerBound: " + lowerBound);
        }
        internal static void CalculateUsedPlatesRows(string algorithmName, Well[][,] plates)
        {
            int nOfPlatesUsed = plates.Length;
            int nOfRowsUsed = 0;
            for (int i = 0; i < plates.Length; i++)
            {
                if (plates[i][0, 0] == null)
                {
                    nOfPlatesUsed = i;
                    break;
                }
            }

            for (int i = 0; i < plates[nOfPlatesUsed - 1].GetLength(0); i++)
            {
                if (plates[nOfPlatesUsed - 1][i, 0] == null)
                {
                    nOfRowsUsed = i;
                    break;
                }
                if (i == plates[nOfPlatesUsed - 1].GetLength(0) - 1)
                {
                    nOfRowsUsed = i + 1;
                    break;
                }
            }

            string plateStr = "plates";
            string rowStr = "rows";
            if (nOfPlatesUsed == 1)
                plateStr = "plate";
            if (nOfRowsUsed == 1)
                rowStr = "row";

            Console.WriteLine(algorithmName + " used " + nOfPlatesUsed + " " + plateStr + " (" + nOfRowsUsed + " " + rowStr + " in the last one)");
        }
    }
}
