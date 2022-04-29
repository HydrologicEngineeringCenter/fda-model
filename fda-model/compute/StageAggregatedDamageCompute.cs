using structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RasMapperLib;
using fda_hydro.hydraulics;

namespace fda_model.compute
{
    public class StageAggregatedDamageCompute
    {
        private Inventory _inventory;
        private string _polygonShapefilePath;
        private int _polygonID;
        private HydraulicDataset _hydraulics;


        private void Compute()
        {
            IList<IList<StructureDamageResult>> totalResults = new List<IList<StructureDamageResult>>();

            Polygon impactArea = GetSpecificImpactArea();
            Inventory impactAreaInventory = _inventory.GetInventoryTrimmmedToPolygon(impactArea);
            DeterministicInventory sampledInventory = impactAreaInventory.Sample(0);
            HydraulicPoint[] hydroPoints = _hydraulics.GetHydraulicDataFromUnsteadyHDFs(impactAreaInventory.GetPointMs());
            
            //create a array of points with depths for each WS
            //for each computed WS
            for (int i = 0; i < _hydraulics.Probabilities.Length; i++)
            {
                double[] depths = new double[hydroPoints.Length];
                //for each point
                for (int j = 0; j < hydroPoints.Length; j++)
                {
                    depths[j] = hydroPoints[j].depths[i];
                }
                IList<StructureDamageResult> results = sampledInventory.ComputeDamages(depths);
                totalResults.Add(results);
            }
        }
        private Polygon GetSpecificImpactArea()
        {
            PolygonFeatureLayer polygonFeatureLayer = new PolygonFeatureLayer(_polygonShapefilePath);
            var polygons = polygonFeatureLayer.Polygons();
            var polygonsList = polygons.ToList();
            for (int i = 0; i < polygonsList.Count; i++)
            {
                var row = polygonFeatureLayer.FeatureRow(i);
                if ((int)row[i] == _polygonID)
                {
                    Polygon impactArea = polygonsList[i];
                    return impactArea;
                }
            }
            return null;
        }
    }
}
