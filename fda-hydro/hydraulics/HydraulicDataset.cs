using RasMapperLib;
using RasMapperLib.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace fda_model.hydraulics
{
    public class HydraulicDataset
{
        private HydraulicPoint[] _points;
        public HydraulicPoint[] Points { get; }

        public string terrainFile;
        public string structureInventoryFile;
        public IList<string> resultFiles;

        public void GetHydraulicDataForSI()
        {
            var structureInventory = new PointFeatureLayer("Structure_Inventory", structureInventoryFile);
            var terrain = new TerrainLayer("Terrain", terrainFile);
            PointMs pts = new PointMs(structureInventory.Points().Select(p => p.PointM()));
            float[] ptElevs = terrain.ComputePointElevations(pts);

            //Create containing objects
            _points = new HydraulicPoint[ptElevs.Length];

            //Fill Terrains
            for (int i = 0; i < ptElevs.Length; i++)
            {
                _points[i].terrainElevation = ptElevs[i];
            }

            foreach (var result in resultFiles)
            {
                // Construct a result from the given filename.
                var res = new RASResults(result);
                var geo = res.Geometry;
                var wsMap = new RASResultsMap(res, MapTypes.Elevation);

                // Sample the geometry for the given points loaded from the shapefile.
                // If the geometry is the same for all of the results, we can actually reuse this object.
                // (It's pretty fast to recompute though, so I wouldn't bother)
                RASGeometryMapPoints mapPixels = geo.MapPixels(pts);

                // This will produce -9999 for NoData values.
                float[] wsValues = null;
                res.ComputeSwitch(wsMap, mapPixels, RASResultsMap.MaxProfileIndex, ptElevs, null, ref wsValues);

                for (int i = 0; i < ptElevs.Length; i++)
                {
                    _points[i].hydraulicResults.Add(res.Name, wsValues[i]);
                }
            }
        }
    }

   
}
