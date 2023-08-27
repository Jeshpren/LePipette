//using System.Runtime.CompilerServices;

#region variables
InputParams testInput1 = new InputParams(
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
InputParams testInput3 = new InputParams(
    96,
    new string[][] { new[] { "Sam1", "Sam2", "Sam3" }, new[] { "Sam7" }, new[] { "Sam4", "Sam5", "Sam6" }, new[] { "Sam8", "Sam9" }, new[] { "Sa10", "Sa11", "Sa12" }, new[] { "Sa13", "Sa14", "Sa15", "Sa16" }, new[] { "Sa17", "Sa18", "Sa19" } },
    new string[][] { new[] { "ReagA" }, new[] { "ReagD", "ReagE", "ReagF" }, new[] { "ReagB", "ReagC" }, new[] { "ReagG", "ReagH" }, new[] { "ReagI" }, new[] { "ReagJ", "ReagK" }, new[] { "ReagL" } },
    new int[] { 3, 1, 2, 3, 1, 2, 5 },
    1
    );
#endregion

#region MAIN PROGRAM EXECUTION

Well[,] result = GenerateOptimalWellPlacement(testInput3);

#endregion


#region methods
Well[,] GenerateOptimalWellPlacement(InputParams inputParams)
{
    int plateRows = 0;
    int plateCols = 0;
    if (inputParams.plateSize == 96)
    {
        plateRows = 8;
        plateCols = 12;
    }
    else if (inputParams.plateSize == 384)
    {
        plateRows = 16;
        plateCols = 24;
    }

    //x todo 1. generate array of 2d arrays (these will be the starting groups of wells (group of wells = experiment) that will then get sorted)
    Experiment[] experiments = GenerateExperimentArray(inputParams);
    // todo ce je experiment vecji od plate-a ga razrez
    PrintExperimentArray(experiments);

    // sort experiment array: primarily sort by height, if heights match then sort by width 
    Array.Sort(experiments, (a, b) => a.wells.GetLength(0) == b.wells.GetLength(0) ? b.wells.GetLength(1).CompareTo(a.wells.GetLength(1)) : b.wells.GetLength(0).CompareTo(a.wells.GetLength(0)));
    Console.WriteLine("SORTED:\n");
    PrintExperimentArray(experiments);

    // todo 2. find an algorithm for optimal space usage
    // torej: First Fit... ce je na trenutni poziciji dost placa, insertej experiment, drgac pejt v nasledno vrstico
    Well[,] plate = CreateEmtpyPlate(inputParams.plateSize);
    Well[,] plate1 = CreateEmtpyPlate(inputParams.plateSize);
    int currentRow = 0;
    int currentCol = 0;
    for (int i = 0; i < experiments.Length; i++)
    {
        int experimentRows = experiments[i].wells.GetLength(0);
        int experimentCols = experiments[i].wells.GetLength(1);
        while (!(experimentCols < plateCols - currentCol && experimentRows < plateRows - currentRow))
        {
            currentCol += experimentCols;
            if (currentCol >= plateCols)
            {
                currentCol = 0;
                while (plate[currentRow, 0] != null)
                {
                    currentRow++;
                    if (experimentRows >= plateRows - currentRow)
                    {
                        currentRow = 0;
                        currentCol = 0;
                        // * zdej dela --> nared array platov (glede na max plates)
                        plate1 = plate;
                        plate = CreateEmtpyPlate(inputParams.plateSize);
                        break;
                    }
                }
            }
        }
        // put experiment on plate
        for (int rowIdx = 0; rowIdx < experimentRows; rowIdx++)
        {
            for (int columnIdx = 0; columnIdx < experimentCols; columnIdx++)
            {
                plate[currentRow + rowIdx, currentCol + columnIdx] = experiments[i].wells[rowIdx, columnIdx];
            }
        }
        currentCol += experimentCols;
        if (currentCol >= plateCols)
        {
            currentCol = 0;
            while (plate[currentRow, 0] != null)
            {
                currentRow++;
            }
        }
    }

    // PRINT PLATE
    Console.WriteLine("PLATE:");
    string[] stringsToPrint = new string[plateRows];
    for (int i = 0; i < plateRows; i++)
    {
        for (int j = 0; j < plateCols; j++)
        {
            if (plate[i, j] != null)
            {
                stringsToPrint[i] += "[" + plate[i, j].sample + "-" + plate[i, j].reagent + "] ";
            }
            else
            {
                stringsToPrint[i] += "             ";
            }
        }
        Console.WriteLine(stringsToPrint[i]);
    }
    Console.WriteLine("PLATE1:");
    stringsToPrint = new string[plateRows];
    for (int i = 0; i < plateRows; i++)
    {
        for (int j = 0; j < plateCols; j++)
        {
            if (plate1[i, j] != null)
            {
                stringsToPrint[i] += "[" + plate1[i, j].sample + "-" + plate1[i, j].reagent + "] ";
            }
            else
            {
                stringsToPrint[i] += "             ";
            }
        }
        Console.WriteLine(stringsToPrint[i]);
    }

    return plate;
}

Experiment[] GenerateExperimentArray(InputParams inputParams)
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
void PrintExperimentArray(Experiment[] experimentArray)
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