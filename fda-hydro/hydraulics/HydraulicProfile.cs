using RasMapperLib;
using RasMapperLib.Mapping;
using System;
using System.Collections.Generic;
/// <summary>
/// Think about this rather than a list of double[]
/// </summary>
namespace fda_hydro.hydraulics
{
    public class HydraulicProfile
    {
        public double Probability { get; set; }
        public string FilePath { get; set; }
        public string TerrainPath { get; set; }
        public HydraulicProfile(double probability, string filepath, string terrainFile)
        {
            Probability = probability;
            FilePath = filepath;
            TerrainPath = terrainFile;
        }
        public float[] GetHydraulicDataFromUnsteadyHDFs(PointMs pts)
        {
            // Terrain is going to get sampled every time unnecessarily. This is an opportunity for refactor
            //create feature layers for the standard inputs
            TerrainLayer terrain = new TerrainLayer("Terrain", TerrainPath);
            // get terrain elevations
            float[] terrainElevs = terrain.ComputePointElevations(pts);
            // Construct a result from the given filename.
            var rasResult = new RASResults(FilePath);
            var rasGeometry = rasResult.Geometry;
            var rasWSMap = new RASResultsMap(rasResult, MapTypes.Depth);

            // Sample the geometry for the given points loaded from the shapefile.
            // If the geometry is the same for all of the results, we can actually reuse this object.
            // (It's pretty fast to recompute though, so I wouldn't bother)
            RASGeometryMapPoints mapPixels = rasGeometry.MapPixels(pts);

            // This will produce -9999 for NoData values.
            float[] depthVals = null;
            rasResult.ComputeSwitch(rasWSMap, mapPixels, RASResultsMap.MaxProfileIndex, terrainElevs, null, ref depthVals);
            
            return depthVals;
        }
    }
}
