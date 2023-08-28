

namespace LePipette.Classes
{
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
}
