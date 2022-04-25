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
            //create feature layers for the standard inputs
            PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", _structureInventoryFile);
            TerrainLayer terrain = new TerrainLayer("Terrain", _terrainFile);
            PolygonFeatureLayer impactAreas = new PolygonFeatureLayer("Impact_Areas", _impactAreasFile);

            //get individual points out of the SI feature layer as a list
            IEnumerable<Point> sIPoints = structureInventory.Points();

            //change those points to point Ms because that's what RASMapper wants to work with
            PointMs pts = new PointMs();
            foreach(Point point in sIPoints)
            {
                pts.Add(point.PointM());
            }

            // get terrain elevations
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
