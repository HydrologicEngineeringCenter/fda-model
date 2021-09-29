using NUnit.Framework;
using paireddata;

namespace fda_model_test
{
    public class PairedDataTest
    {
        static double[] Probabilities = new double[] { .99, .95, .9, .8, .7, .6, .5, .4, .3, .2, .1, .01, .002, .001 };
        static double[] Flows = new double[] { 10, 100, 1000, 2000, 2500, 4000, 5000, 10000, 25000, 50000, 100000, 120000, 150000, 175000 };
        static double[] Stages = new double[] { 556, 562, 565, 566, 570, 575, 578, 600, 610, 650, 700, 750, 800, 850 };
        static double[] Damages = new double[] { 1, 10, 30, 45, 59, 78, 89, 102, 140, 180, 240, 330, 350, 370 };
        static double[] ProbabilitiesOfFailure = new double[] { .001, .01, .1, .5, 1 };
        static double[] ElevationsOfFailure = new double[] { 600, 610, 650, 700, 750 };

        PairedData myRatingCurve = new PairedData(Flows, Stages);
        PairedData myFlowFrequencyCurve = new PairedData(Probabilities, Flows);
        PairedData myDamageFrequencyCurve = new PairedData(Probabilities, Damages);
        PairedData myFragilityCurve = new PairedData(ElevationsOfFailure, ProbabilitiesOfFailure);
        PairedData myStageDamageCurve = new PairedData(Stages, Damages);

        [Test]
        public void FInterpolatesCorrectBetween()
        {
            double expected = 565.5; //linear interp of ys when x=1500
            double actual = myRatingCurve.f(1500);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FReturnsMaxIfaAboveBounds()
        {
            double expected = 850;
            double actual = myRatingCurve.f(10000000);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FReturnsMinIfBelowBounds()
        {
            double expected = 556;
            double actual = myRatingCurve.f(0);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void FReturnsExactIfExact()
        {
            double expected = 562;
            double actual = myRatingCurve.f(100);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ComposeComposes()
        {
            IPairedData expected = new PairedData(Probabilities, Stages); //Probability-Stage
            IPairedData actual = myRatingCurve.compose(myFlowFrequencyCurve);
            Assert.AreEqual(expected.Xvals,actual.Xvals);
            Assert.AreEqual(expected.Yvals, actual.Yvals);
        }
        [Test]
        public void IntegrateIntegrates()
        {
            double expected = 113.125;
            double actual = myDamageFrequencyCurve.integrate();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void MultiplyMultiplies()
        {
            double[] expectedXs = { 556, 562, 565, 566, 570, 575, 578, 599.999999999999, 600, 610, 650, 700, 750, 750.00000001, 800, 850 };
            double[] expectedYs = { 0, 0, 0, 0, 0, 0, 0, 0, 0.10200000000000001, 1.4000000000000001, 18, 120, 330, 330.000000004, 350, 370 };
            PairedData expected = new PairedData(expectedXs, expectedYs); //Stage-Damage
            PairedData actual = (PairedData)myStageDamageCurve.multiply(myFragilityCurve);
            Assert.AreEqual(expected.Xvals, actual.Xvals);
            Assert.AreEqual(expected.Yvals, actual.Yvals);
        }
    }
}