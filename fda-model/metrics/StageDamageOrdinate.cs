using fda_hydro.hydraulics;

namespace fda_model.metrics
{
    public class StageDamageOrdinate
{
        public HydraulicProfile HydraulicSet { get; set; }
        //public StageDamageResult DamageResult { get; set; }
        public double[] indexPointsWSE { get; set; } //multiple because of multiple impact areas. Once depth for each for this Hydraulic set. 
    }
}
