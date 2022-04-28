using Xunit;
using fda_hydro;
using fda_hydro.hydraulics;
using System.Collections.Generic;
using System;
using System.IO;
using Geospatial.GDALAssist;

namespace fda_hydro_test
{
    /// <summary>
    ///Test data needs to all be in the same projection.
    /// </summary>
    public class HydraulicDatasetShould
    {

        static string terrainFile = "C:\\TestData\\Terrain\\Terrain.hdf";
        static string SIFile = "C:\\TestData\\NSI\\StructureInventory.shp";
        static string impactAreaFile = "C:\\TestData\\ImpactAreas\\ImpactAreas.shp";

        static Dictionary<string, double> resultsFiles = new Dictionary<string, double>
         {
                {"C:\\TestData\\Native Output Files\\Muncie.p08.hdf",.002  },
                {"C:\\TestData\\Native Output Files\\Muncie.p07.hdf",.005  },
                {"C:\\TestData\\Native Output Files\\Muncie.p06.hdf",.01  },
                {"C:\\TestData\\Native Output Files\\Muncie.p05.hdf",.02  },
                {"C:\\TestData\\Native Output Files\\Muncie.p04.hdf",.05  },
                {"C:\\TestData\\Native Output Files\\Muncie.p03.hdf",.1  },
                {"C:\\TestData\\Native Output Files\\Muncie.p02.hdf",.2  },
                {"C:\\TestData\\Native Output Files\\Muncie.p01.hdf",.5  }
        };

        static HydraulicDatasetShould()
        {
            string gdalPath = "C:\\Programs\\6.x Development\\GDAL\\";
            if (!Directory.Exists(gdalPath))
            {
                Console.WriteLine("GDAL directory not found: " + gdalPath);
                return;
            }
            GDALSetup.InitializeMultiplatform(gdalPath);
        }

        [Fact]
        public void sampleSomeValues()
        {
            HydraulicDataset dataset = new HydraulicDataset(terrainFile, SIFile, impactAreaFile);
            var points = dataset.GetHydraulicDataFromUnsteadyHDFs(resultsFiles);
            bool isAnythingWet = false;
            foreach (HydraulicPoint point in points)
            {
                if (point.probabilityValue[.002] > 0)
                {
                    isAnythingWet = true;
                }
            }
            Assert.True(isAnythingWet);
        }

        [Fact]
        public void ReturnHydraulicPoints()
        {

            HydraulicDataset dataset = new HydraulicDataset(terrainFile, SIFile, impactAreaFile);
            var points = dataset.GetHydraulicDataFromUnsteadyHDFs(resultsFiles);
            Assert.True(points.Length > 0);
        }
    }

}



