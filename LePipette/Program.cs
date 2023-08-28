//using System.Runtime.CompilerServices;
using LePipette.Classes;
using LePipette.Helper;

#region variables
InputParams _testInput1 = new InputParams(
    96,
    new string[][] { new[] { "Sm1", "Sm2", "Sm3" }, new[] { "Sm1", "Sm3", "Sm4" } },
    new string[][] { new[] { "RaX", "RaY" }, new[] { "RaY", "RaZ" } },
    new int[] { 1, 3 },
    1
    );
InputParams _testInput2 = new InputParams(
    96,
    new string[][] { new[] { "Sm1", "Sm2", "Sm3" }, new[] { "Sm7" }, new[] { "Sm4", "Sm5", "Sm6" }, new[] { "Sm8", "Sm9" }, new[] { "S10", "S11", "S12" } },
    new string[][] { new[] { "RaA" }, new[] { "RaD", "RaE", "RaF" }, new[] { "RaB", "RaC" }, new[] { "RaG", "RaH" }, new[] { "RaI" } },
    new int[] { 3, 1, 2, 3, 1 },
    1
    );
InputParams _testInput3 = new InputParams(
    96,
    new string[][] { new[] { "Sm1", "Sm2", "Sm3" }, new[] { "Sm7" }, new[] { "Sm4", "Sm5", "Sm6" }, new[] { "Sm8", "Sm9" }, new[] { "S10", "S11", "S12" }, new[] { "S13", "S14", "S15", "S16" }, new[] { "S17", "S18", "S19" } },
    new string[][] { new[] { "RaA" }, new[] { "RaD", "RaE", "RaF" }, new[] { "RaB", "RaC" }, new[] { "RaG", "RaH" }, new[] { "RaI" }, new[] { "RaJ", "RaK" }, new[] { "RaL" } },
    new int[] { 3, 1, 2, 3, 1, 2, 5 },
    2
    );
InputParams _testInput4 = new InputParams(
    96,
    new string[][] { new[] { "S01", "S02", "S03" }, new[] { "S07" }, new[] { "S04", "S05", "S06" }, new[] { "S08", "S09" }, new[] { "S10", "S11", "S12" }, new[] { "S13", "S14", "S15", "S16" }, new[] { "S17", "S18", "S19" }, new[] { "S20", "S21", "S22", "S23", "S24" }, new[] { "S25", "S26", "S27", "S28" } },
    new string[][] { new[] { "RaA" }, new[] { "RaD", "RaE", "RaF" }, new[] { "RaB", "RaC" }, new[] { "RaG", "RaH" }, new[] { "RaI" }, new[] { "RaJ", "RaK" }, new[] { "RaL" }, new[] { "RaM", "RaN" }, new[] { "RaO" } },
    new int[] { 3, 1, 2, 3, 1, 2, 5, 1, 1 },
    2
    );
InputParams _testInput5 = new InputParams(
    96,
    new string[][] { new[] { "S01", "S02", "S03" }, new[] { "S07" }, new[] { "S04", "S05", "S06" }, new[] { "S08", "S09" }, new[] { "S10", "S11", "S12" }, new[] { "S13", "S14", "S15", "S16" }, new[] { "S17", "S18", "S19" }, new[] { "S20", "S21", "S22", "S23", "S24" }, new[] { "S25", "S26", "S27", "S28" }, new[] { "S29" }, new[] { "S30", "S31" }, new[] { "S32", "S33", "S34" }, new[] { "S35", "S36", "S37", "S38" }, new[] { "S39", "S40", "S41", "S42", "S43" } },
    new string[][] { new[] { "RAA" }, new[] { "RAD", "RAE", "RAF" }, new[] { "RAB", "RAC" }, new[] { "RAG", "RAH" }, new[] { "RAI" }, new[] { "RAJ", "RAK" }, new[] { "RAL" }, new[] { "RAM", "RAN" }, new[] { "RAO" }, new[] { "RAP", "RAQ" }, new[] { "RAR", "RAS", "RAT", "RAU" }, new[] { "RAV" }, new[] { "RAZ", "RBA", "RBB" }, new[] { "RBC", "RBD", "RBE", "RBF", "RBG", "RBH" } },
    new int[] { 3, 1, 2, 3, 1, 2, 5, 1, 1, 4, 2, 1, 3, 1 },
    3
    );
#endregion

#region MAIN PROGRAM EXECUTION

Well[][,] result = GeneratePlates(_testInput5);

#endregion

#region methods
Well[][,] GeneratePlates(InputParams inputParams)
{
    // todo ku printas plate, poskrb za enako dolzino (trenutnu so vsi name-i istu dolgi in ne rabs na tu skrbet)
    // todo ce je experiment vecji od plate-a ga razrez
    Experiment[] experiments = Helper.GenerateExperimentArray(inputParams);

    //Console.WriteLine("EXPERIMENTS:");
    //Helper.PrintExperimentArray(experiments);

    // First Fit Decreasing
    Well[][,] plates = FirstFitDecreasing(inputParams, experiments);


    return plates;
}
Well[][,] FirstFitDecreasing(InputParams inputParams, Experiment[] experiments)
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
    // sort experiment array: primarily sort by height, if heights match then sort by width 
    Array.Sort(experiments, (a, b) => a.wells.GetLength(0) == b.wells.GetLength(0) ? b.wells.GetLength(1).CompareTo(a.wells.GetLength(1)) : b.wells.GetLength(0).CompareTo(a.wells.GetLength(0)));
    Console.WriteLine("SORTED EXPERIMENTS:");
    Console.WriteLine("--------------------------------------------");
    Helper.PrintExperimentArray(experiments);

    Well[][,] plates = new Well[inputParams.maxPlates][,];
    int[][] platesCurrentPosition = new int[inputParams.maxPlates][];

    for (int i = 0; i < inputParams.maxPlates; i++)
    {
        plates[i] = Helper.CreateEmtpyPlate(inputParams.plateSize);
        platesCurrentPosition[i] = new int[] { 0, 0 };
    }
    int currentRow = 0;
    int currentCol = 0;
    int currentPlate = 0;


    for (int i = 0; i < experiments.Length; i++)
    {
        currentPlate = 0;
        int experimentRows = experiments[i].wells.GetLength(0);
        int experimentCols = experiments[i].wells.GetLength(1);

        currentRow = platesCurrentPosition[currentPlate][0];
        currentCol = platesCurrentPosition[currentPlate][1];

        while (true)
        {
            // wide enough?
            if (experimentCols <= plateCols - currentCol)
            {//yes
                // high enough?
                if (experimentRows <= plateRows - currentRow)
                {//yes
                    // insert experiment
                    for (int rowIdx = 0; rowIdx < experimentRows; rowIdx++)
                        for (int columnIdx = 0; columnIdx < experimentCols; columnIdx++)
                            plates[currentPlate][currentRow + rowIdx, currentCol + columnIdx] = experiments[i].wells[rowIdx, columnIdx];

                    platesCurrentPosition[currentPlate][0] = currentRow;
                    platesCurrentPosition[currentPlate][1] = currentCol + experimentCols;
                    break;
                }
                else
                {
                    currentPlate++;
                    if (currentPlate >= inputParams.maxPlates)
                    {
                        Console.WriteLine("error: max number of plates exceeded");
                        Environment.Exit(0);
                    }
                    currentRow = platesCurrentPosition[currentPlate][0];
                    currentCol = platesCurrentPosition[currentPlate][1];
                }
            }
            else
            {
                // back to first column
                currentCol = 0;
                // find next empty row (in first column)
                while (plates[currentPlate][currentRow, 0] != null)
                {
                    currentRow++;
                    if (currentRow >= plateRows)
                        break;
                }
            }
        }
    }

    // PRINT PLATES
    Helper.PrintPlates(plates, plateRows, plateCols);

    return plates;
}
#endregion