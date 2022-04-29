using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace structures
{
    public class Structure
    {
        private int _fdid;
        private double _x;
        private double _y;
        private double _foundationHeightMean;
        private Statistics.ContinuousDistribution _StructureValue; //ehhh, how do we get these distributions? we don't define them at the structure level. 
        private Statistics.ContinuousDistribution _ContentValue;
        private Statistics.ContinuousDistribution _OtherValue;
        private double _StructureValueFromInput;
        private double _ContentValueFromInput;
        private double _OtherValueFromInput;
        private string _occtype_name;
        private string _damcat_name;
        private int _pop2amu65;
        private int _pop2amo65;
        private int _pop2pmu65;
        private int _pop2pmo65;

        //This parameter list lines up with columnsOfInterest in the Inventory 
        public Structure(int name, double x, double y, double foundationHeightMean,double structureValue, double contentValue, 
            double vehicleValue, string damCat, string occtype, int pop2amu65, int pop2amo65, int pop2pmu65, int pop2pm065)
        {
            _fdid = name;
            _x = x;
            _y = y;
            _foundationHeightMean = foundationHeightMean;
            _StructureValueFromInput = structureValue;
            _ContentValueFromInput = contentValue;
            _OtherValueFromInput = vehicleValue;
            _occtype_name = occtype;
            _damcat_name = damCat;
            _pop2amu65 = pop2amu65;
            _pop2amo65 = pop2amo65;
            _pop2pmu65 = pop2pmu65;
            _pop2pmo65 = pop2pm065;
        }
        public string DamCatName { get { return _damcat_name; } }
        public string OccTypeName { get { return _occtype_name; } }
        public DeterministicStructure Sample(int seed, DeterministicOccupancyType occtype)
        {
            Random random = new Random(seed);
            double foundHeightSample = _foundationHeightMean + (_foundationHeightMean * occtype.FoundationHeightError);
            double structValueSample = _StructureValue.InverseCDF(random.NextDouble());
            //load up the deterministic structure
            return new DeterministicStructure(_fdid,structValueSample,foundHeightSample);
        }

    }
}

