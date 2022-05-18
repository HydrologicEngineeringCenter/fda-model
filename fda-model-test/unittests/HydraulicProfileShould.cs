using Xunit;
using fda_hydro.hydraulics;
using System.Collections.Generic;
using System;
using System.IO;
using Geospatial.GDALAssist;
using structures;

namespace fda_hydro_test
{
    /// <summary>
    ///Test data needs to all be in the same projection.
    /// </summary>
    public class HydraulicProfileShould
    {

        static string terrainFile = "C:\\TestData\\Terrain\\Terrain.hdf";
        static string SIFile = "C:\\TestData\\NSI\\StructureInventory.shp";
        static string impactAreaFile = "C:\\TestData\\ImpactAreas\\ImpactAreas.shp";

        static string[] resultsFiles = new string[]
         {
                "C:\\TestData\\Native Output Files\\Muncie.p08.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p07.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p06.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p05.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p04.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p03.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p02.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p01.hdf" 
        };

        static double[] probabilities = new double[] { .002, .005, .01, .05, .1, .2, .5 };


        static HydraulicProfileShould()
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
            List<HydraulicProfile> profiles = new List<HydraulicProfile>();
            for(int i = 0; i < resultsFiles.Length; i++)
            {
             profiles.Add(new HydraulicProfile(probabilities[i],resultsFiles[i],terrainFile));
            }
            HydraulicDataset dataset = new HydraulicDataset(profiles);
            Inventory inventory = new Inventory(SIFile,impactAreaFile);
            float[] depths = dataset.HydraulicProfiles[0].GetHydraulicDataFromUnsteadyHDFs(inventory.GetPointMs());
            Console.WriteLine("we did it!");
        }
    }

}



