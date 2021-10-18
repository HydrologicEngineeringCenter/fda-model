﻿

using Xunit;
using ead;
using paireddata;
using Statistics;
using System.Collections.Generic;

namespace fda_model_test
{
    public class SimulationShould
    {
        //These were previously used in pairedDataTest but were moved here to be used for ead compute testing. 
        static double[] Flows = { 0, 100000 };
        static double[] Stages = { 0, 150000 };
        //static double[] ProbabilitiesOfFailure = { .001, .01, .1, .5, 1 };
        //static double[] ElevationsOfFailure = { 600, 610, 650, 700, 750 };
        [Theory]
        [InlineData(150000)]
        public void ComputeEAD(double expected)
        {
            
            Statistics.IDistribution flow_frequency = IDistributionFactory.FactoryUniform(0, 100000, 1000);
            //create a stage distribution
            IDistribution[] stages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                stages[i] = IDistributionFactory.FactoryUniform(0, 300000*i, 10);
            }
            UncertainPairedData flow_stage = new UncertainPairedData(Flows, stages);
            //create a damage distribution
            IDistribution[] damages = new IDistribution[2];
            for (int i = 0; i < 2; i++)
            {
                damages[i] = IDistributionFactory.FactoryUniform(0, 600000*i, 10);
            }
            UncertainPairedData stage_damage = new UncertainPairedData(Stages, damages, "residential");
            List<UncertainPairedData> upd = new List<UncertainPairedData>();
            upd.Add(stage_damage);
            Simulation s = new Simulation(flow_frequency,flow_stage,upd);
            
            metrics.IContainResults r = s.Compute(0,1);
            double difference = expected - r.MeanEAD("residential");
            double percentDiff = difference / expected;
            Assert.True(percentDiff < .01);
        }

        
    }
}
