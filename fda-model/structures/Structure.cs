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
        private Statistics.ContinuousDistribution _foundationHeight;
        private Statistics.ContinuousDistribution _StructureValue;
        private Statistics.ContinuousDistribution _ContentValue;
        private Statistics.ContinuousDistribution _OtherValue;
        private OccupancyType _occtype;
        public double ComputeDamage(double depth)
        { 
            //sample stuff... (should probably be in a method to return a deterministic structure
            double foundHeightSample = _foundationHeight.InverseCDF(.5);
            double structValueSample = _StructureValue.InverseCDF(.5);
            paireddata.IPairedData structDamagePairedData = _occtype.StructureDamageFunction.SamplePairedData(.5);
            
            double depthabovefoundHeight = depth - foundHeightSample;
            double structDamagepercent = structDamagePairedData.f(depthabovefoundHeight);

            return structDamagepercent*structValueSample;
        }
    }
}

