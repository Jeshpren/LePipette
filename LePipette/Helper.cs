

using LePipette.Classes;

namespace LePipette.Helper
{
    internal class Helper
    {
        internal static Experiment[] GenerateExperimentArray(InputParams inputParams)
        {
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
            return experimentArray;
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
                Console.WriteLine("\nPLATE" + i + ":");
                Console.WriteLine("--------------------------------------------");
                string[] stringsToPrint = new string[plateRows];
                for (int j = 0; j < plateRows; j++)
                {
                    for (int k = 0; k < plateCols; k++)
                    {
                        if (plates[i][j, k] != null)
                            stringsToPrint[j] += "[" + plates[i][j, k].sample + "-" + plates[i][j, k].reagent + "] ";
                        else
                            stringsToPrint[j] += "[       ] ";
                    }
                    Console.WriteLine(stringsToPrint[j]);
                }
            }
        }
        internal static Well[,] CreateEmtpyPlate(int plateSize)
        {
            Well[,] outputPlate;
            if (plateSize == 96)
            {
                outputPlate = new Well[8, 12];
            }
            else if (plateSize == 384)
            {
                outputPlate = new Well[16, 24];
            }
            else
            {
                outputPlate = new Well[0, 0];
                Console.WriteLine("error: valid values for plateSize are 96 and 384");
                Environment.Exit(0);
            }
            return outputPlate;
        }
    }
}
