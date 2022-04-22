using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace structures
{
    public class Structure
    {
        private string name;
        private double _x;
        private double _y;
        private double _foundationHeightMean;
        private Statistics.ContinuousDistribution _StructureValue;
        private Statistics.ContinuousDistribution _ContentValue;
        private Statistics.ContinuousDistribution _OtherValue;
        private string occtype_name;
        private string damcat_name;

        public string DamCatName { get { return damcat_name; } }
        public string OccTypeName { get { return occtype_name; } }
        public DeterministicStructure Sample(int seed, DeterministicOccupancyType occtype)
        {
            Random random = new Random(seed);
            double foundHeightSample = _foundationHeightMean + (_foundationHeightMean * occtype.FoundationHeightError);
            double structValueSample = _StructureValue.InverseCDF(random.NextDouble());
            //load up the deterministic structure
            return new DeterministicStructure(name,structValueSample,foundHeightSample);
        }

    }
}

