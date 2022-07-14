using System;
using System.Collections.Generic;
using alternatives;
using compute;
using metrics;
using paireddata;
using scenarios;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;
using Xunit;


namespace fda_model_test.integrationtests
{
    public class DesPeresStudyDataTests
    {
        private static int impactAreaID = 1;
        private static int erl = 50;
        private static double discountRate = 0.025;
        private static int poa = 50;
        private static double[] exceedanceProabilities = new double[] { .5, .2, .1, .04, .02, .01, .005, .002 };
        private static double[] stagesForFrequency_impID1 = new double[] { .001, .002, .003, .004, .005, .006, .007, .553 };
        private static CurveMetaData metaDataDefault = new CurveMetaData("x","y","res","struct");
        private static GraphicalUncertainPairedData graphicalUncertain = new GraphicalUncertainPairedData(exceedanceProabilities, stagesForFrequency_impID1, erl, metaDataDefault);
        private static double[] stagesForZeroDamageCurve = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2, 2.1 };

        private static IDistribution[] zeroDamageDistributions = new IDistribution[]
        {
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                                new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                                new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                                new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0),
                    new Normal(0,0)
        };
        private static UncertainPairedData zeroStageDamage = new UncertainPairedData(stagesForZeroDamageCurve, zeroDamageDistributions, metaDataDefault);
        private static List<UncertainPairedData> zeroDamageStageDamageList = new List<UncertainPairedData>();
        private static int seed = 1234;
        private static RandomProvider randomProvider = new RandomProvider(seed);
        private static int baseYear = 2025;
        private static int futureYear = 2040;

        private static double[] stagesWithoutFutureImpactArea18 = new double[] { 0.001, 0.002, 1.851, 3.063, 3.352, 4, 5, 6 };
        private static GraphicalUncertainPairedData stageFrequencyWithoutFutureImpactArea18 = new GraphicalUncertainPairedData(exceedanceProabilities, stagesWithoutFutureImpactArea18, erl, metaDataDefault, false);
        private static double[] stagesWithoutBaseImpactArea18 = new double[] { .001, .002, 1.851, 3.063, 3.352, 3.983, 4.814, 5.868 };
        private static GraphicalUncertainPairedData stageFrequencyWithoutBaseImpactArea18 = new GraphicalUncertainPairedData(exceedanceProabilities, stagesWithoutBaseImpactArea18, erl, metaDataDefault, false);
        private static double[] stagesForDamageImpactArea18WithoutBaseYear = new double[]
        {
            0, 0.5, 1, 1.5, 2, 2.5, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7, 7.5, 8, 8.5, 9, 9.5, 10, 10.5, 11, 11.5
        };
        private static double[] meansImpactArea18DamageBaseWithout = new double[]
        {
            0,0,0,0,125.864041,257.718839,494.662219,2441.381,4557.5978,7169.34217,10306.0281,13661.0593,18325.6849,23657.2966,29061.7113,34261.1793,39273.074,44010.2348,48562.848,52999.7949,57227.1359,61152.7429,64790.4545,68175.8357
        };
        private static double[] standardDeviationsImpactArea18DamageBaseWithout = new double[]
        {
            0,0,0,0,63.148925,128.885036,204.638731,612.9643,882.427944,1070.74118,1206.48709,1368.12856,1891.23798,2253.31403,2457.36166,2721.39185,2985.85403,3209.71033,3482.13836,3766.95502,4006.82226,4232.84892,4473.73049,4701.48844
        };
        private static double[] meansImpactArea18DamageFutureWithout = new double[]
        {
            0,0,0,0,125.864041,257.718839,494.662219,2441.381,4557.5978,7169.34217,10306.0281,13661.0593,18325.6849,23657.2966,29061.7113,34261.1793,39273.074,44010.2348,48562.848,52999.7949,57227.1359,61152.7429,64790.4545,68175.8357
        };
        private static double[] standardDeviationsImpactArea18DamageFutureWithout = new double[]
        {
            0,0,0,0,63.148925,128.885036,204.638731,612.9643,882.427944,1070.74118,1206.48709,1368.12856,1891.23798,2253.31403,2457.36166,2721.39185,2985.85403,3209.71033,3482.13836,3766.95502,4006.82226,4232.84892,4473.73049,4701.48844
        };

        [Theory]
        [InlineData(270.22)]
        public void ShouldNotHitTwosComplementForManyIterationsToConvergence(double expected)
        {
            IDistribution[] baseDamages = new IDistribution[stagesForDamageImpactArea18WithoutBaseYear.Length];
            IDistribution[] futureDamages = new IDistribution[stagesForDamageImpactArea18WithoutBaseYear.Length];

            for (int i = 0; i < stagesForDamageImpactArea18WithoutBaseYear.Length; i++)
            {
                baseDamages[i] = new Normal(meansImpactArea18DamageBaseWithout[i], standardDeviationsImpactArea18DamageBaseWithout[i]);
                futureDamages[i] = new Normal(meansImpactArea18DamageFutureWithout[i], standardDeviationsImpactArea18DamageFutureWithout[i]);
            }

            UncertainPairedData baseStageDamage = new UncertainPairedData(stagesForDamageImpactArea18WithoutBaseYear, baseDamages, metaDataDefault);
            List<UncertainPairedData> baseStageDamages = new List<UncertainPairedData> { baseStageDamage };
            UncertainPairedData futureStageDamage = new UncertainPairedData(stagesForDamageImpactArea18WithoutBaseYear, futureDamages, metaDataDefault);
            List<UncertainPairedData> futureStageDamages = new List<UncertainPairedData> { futureStageDamage };

            ImpactAreaScenarioSimulation baseSimulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFrequencyStage(stageFrequencyWithoutBaseImpactArea18)
                .withStageDamages(baseStageDamages)
                .build();
            List<ImpactAreaScenarioSimulation> baseSimulations = new List<ImpactAreaScenarioSimulation> { baseSimulation };
            Scenario baseScenario = new Scenario(baseYear, baseSimulations);
            ScenarioResults baseResults = baseScenario.Compute(randomProvider, new ConvergenceCriteria());
            ImpactAreaScenarioSimulation futureSimulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFrequencyStage(stageFrequencyWithoutFutureImpactArea18)
                .withStageDamages(futureStageDamages)
                .build();
            List<ImpactAreaScenarioSimulation> futureSimulations = new List<ImpactAreaScenarioSimulation> { futureSimulation };
            Scenario futureScenario = new Scenario(futureYear, futureSimulations);
            ScenarioResults futureResults = futureScenario.Compute(randomProvider, new ConvergenceCriteria());
            int altID = 2;
            AlternativeResults alternativeResults = Alternative.AnnualizationCompute(randomProvider, discountRate, poa, altID, baseResults, futureResults);
            double actual = alternativeResults.MeanAAEQDamage(impactAreaID);
            double difference = Math.Abs(actual - expected);
            double relativeDifference = difference / expected;
            double tolerance = 0.05;
            Assert.True(relativeDifference < tolerance);
        }

        [Fact]
        public void ComputeShouldRunSuccessfullyForZeroDamageDamageCurves()
        {
            zeroDamageStageDamageList.Add(zeroStageDamage);
            ImpactAreaScenarioSimulation simulation = ImpactAreaScenarioSimulation.builder(impactAreaID)
                .withFrequencyStage(graphicalUncertain)
                .withStageDamages(zeroDamageStageDamageList)
                .build();

            ImpactAreaScenarioResults impactAreaScenarioResults = simulation.Compute(randomProvider, new ConvergenceCriteria());

            Assert.True(impactAreaScenarioResults.ConsequenceResults.GetConsequenceResultsHistogram("res", "struct", impactAreaID).HistogramIsZeroValued);

        }
    }
}
 