using RasMapperLib;
using RasMapperLib.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace fda_hydro.hydraulics
{
    // Class needs to
    // get hydraulic data for a point and save it into an object
    // assign values from the point shapefile to the object
    // check which element of a polygon shapefile each structure belongs to, and save that to the object
    //
    //Also need to handle both grids and hdf. 
    //

    public class HydraulicDataset
{
        
        private string _terrainFile;
        private string _structureInventoryFile;
        private string _impactAreasFile;

        public HydraulicDataset(string terrain, string structureInventoryFile, string impactAreasFile)
        {
            _terrainFile = terrain;
            _structureInventoryFile = structureInventoryFile;
            _impactAreasFile = impactAreasFile;
        }

        public HydraulicPoint[] GetHydraulicDataFromUnsteadyHDFs(Dictionary<string, double> resultFileProbability)
        {
            var structureInventory = new PointFeatureLayer("Structure_Inventory", _structureInventoryFile);
            var terrain = new TerrainLayer("Terrain", _terrainFile);
            var impactAreas = new PolygonFeatureLayer("Impact_Areas", _impactAreasFile);
            PointMs pts = new PointMs(structureInventory.Points().Select(p => p.PointM()));
            float[] terrainElevs = terrain.ComputePointElevations(pts);

            //Create containing objects
            HydraulicPoint[] points = new HydraulicPoint[terrainElevs.Length];

            //Fill Terrains
            for (int i = 0; i < terrainElevs.Length; i++)
            {
                points[i] = new HydraulicPoint();
                points[i].terrainElevation = terrainElevs[i];
            }

            //Assign Impact Area
            for(int i = 0; i < points.Length; i++)
            {
                var pgons = impactAreas.Polygons();
                foreach(var pgon in pgons)
                {
                    if (pgon.Contains(pts[i]))
                    {
                        points[i].ImpactArea = "Good enough for now.";
                    }
                }
            }
            foreach (var result in resultFileProbability)
            {
                // Construct a result from the given filename.
                var rasResult = new RASResults(result.Key);
                var rasGeometry = rasResult.Geometry;
                var rasWSMap = new RASResultsMap(rasResult, MapTypes.Elevation);
                var rasWSMap = new RASResultsMap()

                // Sample the geometry for the given points loaded from the shapefile.
                // If the geometry is the same for all of the results, we can actually reuse this object.
                // (It's pretty fast to recompute though, so I wouldn't bother)
                RASGeometryMapPoints mapPixels = rasGeometry.MapPixels(pts);

                // This will produce -9999 for NoData values.
                float[] wsValues = null;
                rasResult.ComputeSwitch(rasWSMap, mapPixels, RASResultsMap.MaxProfileIndex, terrainElevs, null, ref wsValues);

                for (int i = 0; i < terrainElevs.Length; i++)
                {
                    points[i].probabilityValue.Add(result.Value, wsValues[i]);
                }
            }
            return points;
        }
    }


   
}
