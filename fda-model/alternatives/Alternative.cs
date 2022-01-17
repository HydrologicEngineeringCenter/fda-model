using System;
using System.Collections.Generic;
using scenarios;
using System.Linq;
using Statistics.Histograms;
using metrics;

namespace alternatives
{
    public class Alternative
    {
        private Scenario _currentYear;
        private Scenario _futureYear;
        private Int64 _periodOfAnalysis;
        private double _discountRate;

        //probably need getters and setters
        public Alternative(Scenario currentYear, Scenario futureYear, Int64 periodOfAnalysis){
            _currentYear = currentYear;
            _futureYear = futureYear;
            _periodOfAnalysis = periodOfAnalysis;
        }

        public Dictionary<int,Dictionary<string,Histogram>> AnnualizationCompute(interfaces.IProvideRandomNumbers rp, Int64 iterations, double discountRate)
        {
            _discountRate = discountRate;
            Dictionary<int,Results> baseYearResults = _currentYear.Compute(rp, iterations);//this is a list of impact area-specific ead
            Dictionary<int, Results> mlfYearResults = _futureYear.Compute(rp, iterations);

            Dictionary<int, Dictionary<string, Histogram>> keyValuePairs = new Dictionary<int, Dictionary<string, Histogram>>();

            foreach (int impactAreaID in baseYearResults.Keys)
            {
                Dictionary<string, Histogram> AaeqDamageHistogramDictionary = new Dictionary<string, Histogram>();
                foreach (string damageCategory in baseYearResults[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs.Keys)
                {
                    double min = 0;
                    double binWidth = 1; //guessing
                    Histogram histogram = new Histogram(min,binWidth);
                    for (int i = 0; i < iterations; i++)
                    {
                        double eadSampledBaseYear = baseYearResults[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs[damageCategory].InverseCDF(rp.NextRandom());
                        double eadSampledFutureYear = mlfYearResults[impactAreaID].ExpectedAnnualDamageResults.HistogramsOfEADs[damageCategory].InverseCDF(rp.NextRandom());
                        double aaeqDamage = ComputeEEAD(eadSampledBaseYear, eadSampledFutureYear);
                        histogram.AddObservationToHistogram(aaeqDamage);
                    }
                    AaeqDamageHistogramDictionary.Add(damageCategory, histogram);
                }
                keyValuePairs.Add(impactAreaID, AaeqDamageHistogramDictionary);
            }
            return keyValuePairs;
        }

        public double ComputeEEAD(double baseEAD, double mlfEAD){

            //probably instantiate a rng to seed each impact area differently

            double[] interpolatedEADs = Interpolate(baseEAD, mlfEAD, _currentYear.Year, _futureYear.Year, _periodOfAnalysis);
            double sumPresentValueEAD = PresentValueCompute(interpolatedEADs, _discountRate);
            double averageAnnualEquivalentDamage = IntoAverageAnnualEquivalentTerms(sumPresentValueEAD, _periodOfAnalysis, _discountRate);
            return averageAnnualEquivalentDamage;
        }
        private double IntoAverageAnnualEquivalentTerms(double sumPresentValueEAD, Int64 periodOfAnalysis, double discountRate)
        {
            double presentValueInterestFactorOfAnnuity = (1 - (1 / Math.Pow(1 + discountRate, periodOfAnalysis))) / discountRate;
            double averageAnnualEquivalentDamage = sumPresentValueEAD / presentValueInterestFactorOfAnnuity;
            return averageAnnualEquivalentDamage;
        }
        private double PresentValueCompute(double[] interpolatedEADs, double discountRate)
        {
            Int64 periodOfAnalysis = interpolatedEADs.Length;
            double[] presentValueInterestFactor = new double[periodOfAnalysis];
            double sumPresentValueEAD = 0;
            for (int i=0; i<periodOfAnalysis; i++)
            {
                presentValueInterestFactor[i] = 1 / Math.Pow(1 + discountRate, i+1);
                sumPresentValueEAD += interpolatedEADs[i] * presentValueInterestFactor[i];
            }
            return sumPresentValueEAD;
        }
        private double[] Interpolate(double baseEAD, double mlfEAD, Int64 baseYear, Int64 mlfYear, Int64 periodOfAnalysis)
        {
            Int64 yearsBetweenBaseAndMLFInclusive = mlfYear - baseYear;
            Int64 yearsAfterMLF = periodOfAnalysis - mlfYear;
            double[] interpolatedEADs = new double[periodOfAnalysis];
            for (Int64 i =0; i<yearsBetweenBaseAndMLFInclusive; i++)
            {
                interpolatedEADs[i] = baseEAD + (i / yearsBetweenBaseAndMLFInclusive) * (baseEAD - mlfEAD);
            }
            for (Int64 i = yearsBetweenBaseAndMLFInclusive; i<periodOfAnalysis; i++)
            {
                interpolatedEADs[i] = mlfEAD;
            }
            return interpolatedEADs;
        }



    }
}