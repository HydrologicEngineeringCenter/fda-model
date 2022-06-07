using Statistics;
using System.Xml.Linq;
using System;
using Statistics.Distributions;
using System.Runtime.Remoting;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Geospatial.GDALAssist;
using fda_hydro.hydraulics;
using fda_model.compute;
using structures;

namespace fda_model_test.unittests
{
    public class StageAggregatedDamageComputeShould
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

        static StageAggregatedDamageComputeShould()
        {
            string gdalPath = "C:\\Programs\\6.x Development\\GDAL\\";
            if (!Directory.Exists(gdalPath))
            {
                Console.WriteLine("GDAL directory not found: " + gdalPath);
                return;
            }
            GDALSetup.InitializeMultiplatform(gdalPath);
        }

        public void Compute()
        {
            //HydraulicDataset hydros = new HydraulicDataset(terrainFile, resultsFiles, probabilities);
            //Inventory inventory = new Inventory(SIFile);
            //StageAggregatedDamageCompute SADC = new StageAggregatedDamageCompute(inventory,impactAreaFile,0,hydros,)
        }


    }

}
