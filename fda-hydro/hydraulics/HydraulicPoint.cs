using System.Collections.Generic;

namespace fda_hydro.hydraulics
{
    public class HydraulicPoint
    {
        public Dictionary<double,double> probabilityValue = new Dictionary<double, double> { };
        public double terrainElevation;
        public string ImpactArea;
        public string uniqueID;
        public HydraulicPoint()
        {

        }
    }
}
