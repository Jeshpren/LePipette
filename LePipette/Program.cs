//using System.Runtime.CompilerServices;

#region variables
InputParams testInput = new InputParams(
    96,
    new string[][] { new[] { "Sam 1", "Sam 2", "Sam3" }, new[] { "Sam 1", "Sam 3", "Sam4" } },
    new string[][] { new[] { "Reag X", "Reag Y" }, new[] { "Reag Y", "Reag Z" } },
    new int[] { 1, 3 },
    1
    );
#endregion

#region MAIN PROGRAM EXECUTION

Well[,] result = GenerateOptimalWellPlacement(testInput);

#endregion


#region methods
Well[,] GenerateOptimalWellPlacement(InputParams inputParams)
{
    Well[,] outputParams = CreateEmtpyPlate(inputParams.plateSize);

    // todo generate array of 2d arrays (these will be the starting group of wells that will then get sorted)

    return outputParams;
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
#endregion