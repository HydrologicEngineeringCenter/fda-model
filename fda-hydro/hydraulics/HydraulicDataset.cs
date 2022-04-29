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
        //relies on result files and probabilities being linked by index.
        private string _terrainFile;
        private string[] _resultFiles;

        public double[] Probabilities { get; set; }

        public HydraulicDataset(string terrain, string[] resultFiles, double[] probabilities)
        {
            _terrainFile = terrain;
            _resultFiles = resultFiles;
            Probabilities = probabilities;

        }

        public HydraulicPoint[] GetHydraulicDataFromUnsteadyHDFs(PointMs pts)
        {
            HydraulicPoint[] hydropoints = new HydraulicPoint[pts.Count()];
            //create feature layers for the standard inputs
            TerrainLayer terrain = new TerrainLayer("Terrain", _terrainFile);
            // get terrain elevations
            float[] terrainElevs = terrain.ComputePointElevations(pts);

            foreach (var result in _resultFiles)
            {
                // Construct a result from the given filename.
                var rasResult = new RASResults(result);
                var rasGeometry = rasResult.Geometry;
                var rasWSMap = new RASResultsMap(rasResult, MapTypes.Depth);

                // Sample the geometry for the given points loaded from the shapefile.
                // If the geometry is the same for all of the results, we can actually reuse this object.
                // (It's pretty fast to recompute though, so I wouldn't bother)
                RASGeometryMapPoints mapPixels = rasGeometry.MapPixels(pts);

                // This will produce -9999 for NoData values.
                float[] depthVals = null;
                rasResult.ComputeSwitch(rasWSMap, mapPixels, RASResultsMap.MaxProfileIndex, terrainElevs, null, ref depthVals);
                
                //Fill in the depths
                for(int i = 0; i < depthVals.Length; i++)
                {
                    hydropoints[i].depths.Add(depthVals[i]);
                }
            }
            return hydropoints;
        }

    }
}


   
}
