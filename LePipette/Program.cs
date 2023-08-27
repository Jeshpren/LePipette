//using System.Runtime.CompilerServices;

#region variables
InputParams testInput = new InputParams(
    96,
    new string[][] { new[] { "Sam1", "Sam2", "Sam3" }, new[] { "Sam1", "Sam3", "Sam4" } },
    new string[][] { new[] { "ReagX", "ReagY" }, new[] { "ReagY", "ReagZ" } },
    new int[] { 1, 3 },
    1
    );
InputParams testInput2 = new InputParams(
    96,
    new string[][] { new[] { "Sam1", "Sam2", "Sam3" }, new[] { "Sam7" }, new[] { "Sam4", "Sam5", "Sam6" }, new[] { "Sam8", "Sam9" }, new[] { "Sa10", "Sa11", "Sa12" } },
    new string[][] { new[] { "ReagA" }, new[] { "ReagD", "ReagE", "ReagF" }, new[] { "ReagB", "ReagC" }, new[] { "ReagG", "ReagH" }, new[] { "ReagI" } },
    new int[] { 3, 1, 2, 3, 1 },
    1
    );
#endregion

#region MAIN PROGRAM EXECUTION

Well[,] result = GenerateOptimalWellPlacement(testInput2);

#endregion


#region methods
Well[,] GenerateOptimalWellPlacement(InputParams inputParams)
{
    Well[,] outputParams = CreateEmtpyPlate(inputParams.plateSize);

    // todo 1. generate array of 2d arrays (these will be the starting groups of wells (group of wells = experiment) that will then get sorted)
    Experiment[] experiments = GenerateExperimentArray(inputParams);
    PrintExperimentArray(experiments);

    // todo 2. find an algorithm for optimal space usage
    // sort experiment array: primarily sort by height, if heights match then sort by width 
    Array.Sort(experiments, (a, b) => a.wells.GetLength(1) == b.wells.GetLength(1) ? b.wells.GetLength(0).CompareTo(a.wells.GetLength(0)) : b.wells.GetLength(1).CompareTo(a.wells.GetLength(1)));

    Console.WriteLine("SORTED:\n");
    PrintExperimentArray(experiments);

    return outputParams;
}

Experiment[] GenerateExperimentArray(InputParams inputParams)
{
    //create the experiment array
    int nOfExperiments = inputParams.nOfReplicates.Length;
    Experiment[] experimentArray = new Experiment[nOfExperiments];
    for (int i = 0; i < inputParams.reagentNames.Length; i++)
    {
        int experimentHeight = inputParams.sampleNames[i].Length;
        int experimentWidth = inputParams.reagentNames[i].Length * inputParams.nOfReplicates[i];
        experimentArray[i] = new Experiment(new Well[experimentWidth, experimentHeight]);
        for (int j = 0; j < inputParams.reagentNames[i].Length; j++)
        {
            for (int k = 0; k < inputParams.sampleNames[i].Length; k++)
            {
                int replicate = 0;
                for (int l = 0; l < inputParams.nOfReplicates[i]; l++)
                {
                    experimentArray[i].wells[replicate + j * inputParams.nOfReplicates[i], k] = new Well(inputParams.sampleNames[i][k], inputParams.reagentNames[i][j]);
                    replicate++;
                }
            }
        }
    }
    return experimentArray;
}
void PrintExperimentArray(Experiment[] experimentArray)
{
    for (int i = 0; i < experimentArray.Length; i++)
    {
        int experimentHeight = experimentArray[i].wells.GetLength(1);
        int experimentWidth = experimentArray[i].wells.GetLength(0);
        string[] stringsToPrint = new string[experimentArray[i].wells.GetLength(1)];
        for (int j = 0; j < experimentHeight; j++)
        {
            for (int k = 0; k < experimentWidth; k++)
            {
                string sample = experimentArray[i].wells[k, j].sample;
                string reagent = experimentArray[i].wells[k, j].reagent;
                stringsToPrint[j] += "[" + sample + "-" + reagent + "] ";
            }
            Console.WriteLine(stringsToPrint[j]);
        }
        Console.WriteLine("");
    }
}
//Well[,] GenerateExperimentArrayOLD(InputParams inputParams)
//{
//    // calculate dimensions of experiment array
//    int height = 0;
//    int width = 0;
//    for (int i = 0; i < inputParams.sampleNames.Length; i++)
//        if (inputParams.sampleNames[i].Length > height)
//            height = inputParams.sampleNames[i].Length;
//    for (int i = 0; i < inputParams.reagentNames.Length; i++)
//        width += inputParams.reagentNames[i].Length * inputParams.nOfReplicates[i];

//    Console.WriteLine("Experiment Arary size: " + width + "x" + height);
//    // create and fill the experiment array
//    Well[,] experimentArray = new Well[width, height];
//    int currentWidth = 0;
//    for (int i = 0; i < inputParams.reagentNames.Length; i++)
//    {
//        for (int j = 0; j < inputParams.reagentNames[i].Length; j++)
//        {
//            for (int k = 0; k < inputParams.sampleNames[i].Length; k++)
//            {
//                int replicate = 0;
//                for (int l = 0; l < inputParams.nOfReplicates[i]; l++)
//                {
//                    experimentArray[currentWidth + replicate, k] = new Well(inputParams.sampleNames[i][k], inputParams.reagentNames[i][j]);
//                    replicate++;
//                }
//            }
//            currentWidth += inputParams.nOfReplicates[i];
//        }
//    }
//    // print experiment array in console
//    string[] stringsToPrint = new string[height];
//    for (int i = 0; i < experimentArray.GetLength(1); i++)
//    {
//        for (int j = 0; j < experimentArray.GetLength(0); j++)
//        {
//            if (experimentArray[j, i] != null)
//            {
//                stringsToPrint[i] += "[" + experimentArray[j, i].sample + "-" + experimentArray[j, i].reagent + "] ";
//            }
//            else
//            {
//                stringsToPrint[i] += "             ";
//            }
//        }
//        Console.WriteLine(stringsToPrint[i]);
//    }

//    return experimentArray;
//}

Well[,] CreateEmtpyPlate(int plateSize)
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
#endregion

#region classes
class InputParams
{
    public int plateSize;
    public string[][] sampleNames;
    public string[][] reagentNames;
    public int[] nOfReplicates;
    public int maxPlates;

    public InputParams(int plateSize, string[][] sampleNames, string[][] reagentNames, int[] nOfReplicates, int maxPlates)
    {
        this.plateSize = plateSize;
        this.sampleNames = sampleNames;
        this.reagentNames = reagentNames;
        this.nOfReplicates = nOfReplicates;
        this.maxPlates = maxPlates;
    }
}

class Well
{
    public string sample;
    public string reagent;

    public Well(string sample, string reagent)
    {
        this.sample = sample;
        this.reagent = reagent;
    }
}
class Experiment
{
    public Well[,] wells;

    public Experiment(Well[,] wells)
    {
        this.wells = wells;
    }
}
#endregion