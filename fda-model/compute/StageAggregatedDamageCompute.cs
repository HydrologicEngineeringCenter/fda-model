using structures;
using System.Collections.Generic;
using RasMapperLib;
using fda_hydro.hydraulics;
using paireddata;
using System;
using fda_model.metrics;

namespace fda_model.compute
{
    public class StageAggregatedDamageCompute
    {
        private Inventory _inventory;
        private HydraulicDataset _hydraulics;

        public StageAggregatedDamageCompute(Inventory inventory, HydraulicDataset hydraulics)
        {
            _inventory = inventory;
            _hydraulics = hydraulics;
        }

        private void Compute(int seed, int iterations)
        {
            Random randomNumberGenerator = new Random(seed);
            //get the XYs off the inventory 
            PointMs structurePoints = _inventory.GetPointMs();
            StageDamageOrdinate stageDamageOrdinate = new StageDamageOrdinate();
            //Create StageDamageOrdinate 
            //Create IList of DamageResult

            //iterate across hydraulics datasets -- we want at each dataset - for a given impact area, for a given damage catagory,
            //for a given asset catagory, we need a distribution of damages
            foreach (HydraulicProfile set in _hydraulics.HydraulicProfiles)
            {
                //get depths for all structure in the inventory 
                float[] depths = set.GetDepths(structurePoints);

                //add uncertainty 
                for (int i = 0; i < iterations; i++)
                {
                    DeterministicInventory sampledInventory = _inventory.Sample(randomNumberGenerator.Next()); //resampling the variable properties on those structures 
                    //compute an iteration of damage for all structures for this hydraulic set
                    StructureDamageResult results = sampledInventory.ComputeDamages(depths);
                }
            }
        }

    }
}
