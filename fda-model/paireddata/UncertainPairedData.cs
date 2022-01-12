using System.Collections.Generic;
using Statistics;
using interfaces;
using System.Linq;
using System.Xml.Linq;
using System;
using Statistics.Distributions;

namespace paireddata
{
    public class UncertainPairedData: IPairedDataProducer, ICategory, ICanBeNull
    {
        #region Fields 
        private double[] _xvals;
        private IDistribution[] _yvals;
        #endregion

        #region Properties 
        public string XLabel { get; }
        public string YLabel { get; }
        public string Name { get; }
        public string Description { get; }
        public int ID { get; }
        public string Category {get;}
        public bool IsNull { get; }
        public double[] xs(){
            return _xvals;
        }
        public IDistribution[] ys(){
            return _yvals;
        }
        #endregion

        #region Constructors 
        public UncertainPairedData()
        {
            IsNull = true;
        }
        //, string xlabel, string ylabel, string name, string description, int ID
        public UncertainPairedData(double[] xs, IDistribution[] ys, string xlabel, string ylabel, string name, string description, int id)
        {
            _xvals = xs;
            _yvals = MakeMeMonotonic(ys);
            Category = "Default";
            IsNull = false;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            Description = description;
            ID = id;
        }
        public UncertainPairedData(double[] xs, IDistribution[] ys, string xlabel, string ylabel, string name, string description, int id, string category){
            _xvals = xs;
            _yvals = MakeMeMonotonic(ys);
            Category = category;
            IsNull = false;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            Description = description;
            ID = id;
        }
        #endregion

        #region Methods 
        /// <summary>
        /// This function should generally given all recurrence intervals are represented as non-exceedance probabilities 
        /// as is required by Simulation. This method assumes that the same distribution type is used for all y. 
        /// This is not the complete solution. 
        /// /// </summary>
        /// <param name="ys"></param> The array of IDistributions 
        private IDistribution[] MakeMeMonotonic(IDistribution[] ys)
        {
            //I think we need to check for strict increasing at the top and strict increasing at the bottom 
            //why?
            //it is possible that the relative increase in standard error is so large that the .05 quantile for non-exceedance prob .05 is less than the .05 quantile 
            //for non-exceedance prob .01 but the .95 quantile for the non-exceedance prob .05 is greater than the .95 quantile for non-exceedance prob .01
            //it is also possible that the decrease in standard error is so large that the converse is true 
            //so this has to be handled whether standard error increased or decreased, but in either case, the standard error will be held constant which will 
            //ensure strict increasing monotonic 
            //so really we can just compare the .05 and .95 quantiles 
            //I don't think we need to handle truncated distributions differently 
            //as long as the second min is greater than the first min,

            IDistribution[] retval = new IDistribution[ys.Length];
            switch (ys[0].Type)
            {
                case IDistributionEnum.Normal:
                    {
                        retval = IAmNormalMakeMeMonotonic(ys);
                        break;
                    }
                case IDistributionEnum.LogNormal:
                    {
                        retval = IAmNormalMakeMeMonotonic(ys);
                        break;
                    }
                case IDistributionEnum.NotSupported:
                    {
                        throw new Exception("This distribution type is not supported");
                    }
                case IDistributionEnum.Triangular:
                    {
                        retval = IAmTriangularMakeMeMonotonic(ys);
                        break;
                    }
                case IDistributionEnum.TruncatedNormal:
                    {
                        retval = IAmNormalMakeMeMonotonic(ys);
                        break;  
                    } 
                case IDistributionEnum.TruncatedTriangular:
                    {
                        retval = IAmTriangularMakeMeMonotonic(ys);
                        break;
                    }
                case IDistributionEnum.TruncatedUniform:
                    {
                        retval = IAmUniformMakeMeMonotonic(ys);
                        break;
                    }
                case IDistributionEnum.Uniform:
                    {
                        retval = IAmUniformMakeMeMonotonic(ys);
                        break;
                    }
            }
            return retval;
        }

        public IDistribution[] IAmNormalMakeMeMonotonic(IDistribution[] distributionArray)
        {
            IDistribution[] monotonicDistributionArray = new IDistribution[distributionArray.Length];
            double lowerBoundNonExceedanceProbability = 0.05;
            double upperBoundNonExceedanceProbability = 0.95;
            double lowerBoundFirstDistribution;
            double lowerBoundSecondDistribution;
            double upperBoundFirstDistribution;
            double upperBoundSecondDistribution;
            bool lowerBoundIsDecreasing;
            bool upperBoundIsDecreasing;


            monotonicDistributionArray[0] = distributionArray[0];
            for (int i = 1; i < distributionArray.Length; i++)
            {
                lowerBoundFirstDistribution = distributionArray[i-1].InverseCDF(lowerBoundNonExceedanceProbability);
                lowerBoundSecondDistribution = distributionArray[i].InverseCDF(lowerBoundNonExceedanceProbability);
                upperBoundFirstDistribution = distributionArray[i - 1].InverseCDF(upperBoundNonExceedanceProbability);
                upperBoundSecondDistribution = distributionArray[i].InverseCDF(upperBoundNonExceedanceProbability);

                lowerBoundIsDecreasing = lowerBoundSecondDistribution < lowerBoundFirstDistribution;
                upperBoundIsDecreasing = upperBoundSecondDistribution < upperBoundFirstDistribution;

                if (lowerBoundIsDecreasing || upperBoundIsDecreasing)
                {
                    double meanSecondDistribution = ((Normal)distributionArray[i]).Mean;
                    int sampleSizeSecondDistribution = ((Normal)distributionArray[i]).SampleSize;
                    double standardDeviationFirstDistribution = ((Normal)distributionArray[i - 1]).StandardDeviation;

                    if (distributionArray[i].Truncated)
                    {
                        double firstMin = ((Normal)distributionArray[i - 1]).Min;
                        double secondMin = ((Normal)distributionArray[i]).Min;
                        double firstMax = ((Normal)distributionArray[i - 1]).Max;
                        double secondMax = ((Normal)distributionArray[i]).Max;
                        bool minIsDecreasing = secondMin < firstMin;
                        bool maxIsDecreasing = secondMax < firstMax;
                        double epsilon = .0001;

                        if (minIsDecreasing && !maxIsDecreasing)
                        {
                            monotonicDistributionArray[i] = IDistributionFactory.FactoryTruncatedNormal(meanSecondDistribution, standardDeviationFirstDistribution, firstMin + epsilon, secondMax,sampleSizeSecondDistribution);
                        } else if (!minIsDecreasing && maxIsDecreasing)
                        {
                            monotonicDistributionArray[i] = IDistributionFactory.FactoryTruncatedNormal(meanSecondDistribution, standardDeviationFirstDistribution, secondMin, firstMax + epsilon, sampleSizeSecondDistribution);
                        } else if(minIsDecreasing && maxIsDecreasing)
                        {
                            monotonicDistributionArray[i] = IDistributionFactory.FactoryTruncatedNormal(meanSecondDistribution, standardDeviationFirstDistribution, firstMin + epsilon, firstMax + epsilon, sampleSizeSecondDistribution);
                        }
                    } else
                    {

                        monotonicDistributionArray[i] = IDistributionFactory.FactoryNormal(meanSecondDistribution, standardDeviationFirstDistribution, sampleSizeSecondDistribution);
                    }
                }
                else
                {
                    monotonicDistributionArray[i] = distributionArray[i];
                }
            }
            return monotonicDistributionArray;
        }
        /// <summary>
        /// We should not ever need to handle monotonicity for triangular or uniform so this code might add time without benefit 
        /// However, if we want to let  users decide their distribution type for the 
        /// aggregated stage-damage compute then we take the responsibility of coming up with the 
        /// uncertain paired data specification and there is a chance our code produces something 
        /// non monotonic
        /// </summary>
        /// <param name="distributionArray"></param>
        /// <returns></returns>
        public IDistribution[] IAmTriangularMakeMeMonotonic(IDistribution[] distributionArray)
        {
            IDistribution[] monotonicDistributionArray = new IDistribution[distributionArray.Length];
            double minFirstDistribution;
            double minSecondDistribution;
            double maxFirstDistribution;
            double maxSecondDistribution;
            int sampleSizeSecondDistribution;
            double mostLikelySecondDistribution;
            double epsilon = 0.0001;
            monotonicDistributionArray[0] = distributionArray[0];
            for (int i = 0; i < distributionArray.Length; i++)
            {
                minFirstDistribution = ((Triangular)distributionArray[i - 1]).Min;
                minSecondDistribution = ((Triangular)distributionArray[i]).Min;
                maxFirstDistribution = ((Triangular)distributionArray[i - 1]).Min;
                maxSecondDistribution = ((Triangular)distributionArray[i]).Min;
                mostLikelySecondDistribution = ((Triangular)distributionArray[i]).MostLikely;
                sampleSizeSecondDistribution = ((Triangular)distributionArray[i]).SampleSize;
                bool minIsDecreasing = minSecondDistribution < minFirstDistribution;
                bool maxIsDecreasing = maxSecondDistribution < maxFirstDistribution;

                if(minIsDecreasing && !maxIsDecreasing)
                {
                    monotonicDistributionArray[i] = IDistributionFactory.FactoryTriangular(minFirstDistribution + epsilon, mostLikelySecondDistribution, maxSecondDistribution, sampleSizeSecondDistribution);
                } else if(!minIsDecreasing && maxIsDecreasing)
                {
                    monotonicDistributionArray[i] = IDistributionFactory.FactoryTriangular(minSecondDistribution, mostLikelySecondDistribution, maxFirstDistribution+epsilon, sampleSizeSecondDistribution);

                } else if(minIsDecreasing && maxIsDecreasing)
                {
                    monotonicDistributionArray[i] = IDistributionFactory.FactoryTriangular(minFirstDistribution + epsilon, mostLikelySecondDistribution, maxFirstDistribution + epsilon, sampleSizeSecondDistribution);

                }
                else
                {
                    monotonicDistributionArray[i] = distributionArray[i];
                }
                
            }
            return monotonicDistributionArray;

        }

        public IDistribution[] IAmUniformMakeMeMonotonic(IDistribution[] distributionArray)
        {
            IDistribution[] monotonicDistributionArray = new IDistribution[distributionArray.Length];
            double minFirstDistribution;
            double minSecondDistribution;
            double maxFirstDistribution;
            double maxSecondDistribution;
            int sampleSizeSecondDistribution;
            double epsilon = 0.0001;
            monotonicDistributionArray[0] = distributionArray[0];
            for (int i = 0; i < distributionArray.Length; i++)
            {
                minFirstDistribution = ((Uniform)distributionArray[i - 1]).Min;
                minSecondDistribution = ((Uniform)distributionArray[i]).Min;
                maxFirstDistribution = ((Uniform)distributionArray[i - 1]).Min;
                maxSecondDistribution = ((Uniform)distributionArray[i]).Min;
                sampleSizeSecondDistribution = ((Uniform)distributionArray[i]).SampleSize;
                bool minIsDecreasing = minSecondDistribution < minFirstDistribution;
                bool maxIsDecreasing = maxSecondDistribution < maxFirstDistribution;

                if (minIsDecreasing && !maxIsDecreasing)
                {
                    monotonicDistributionArray[i] = IDistributionFactory.FactoryUniform(minFirstDistribution + epsilon, maxSecondDistribution, sampleSizeSecondDistribution);
                }
                else if (!minIsDecreasing && maxIsDecreasing)
                {
                    monotonicDistributionArray[i] = IDistributionFactory.FactoryUniform(minSecondDistribution, maxFirstDistribution + epsilon, sampleSizeSecondDistribution);

                }
                else if (minIsDecreasing && maxIsDecreasing)
                {
                    monotonicDistributionArray[i] = IDistributionFactory.FactoryUniform(minFirstDistribution + epsilon, maxFirstDistribution + epsilon, sampleSizeSecondDistribution);

                }
                else
                {
                    monotonicDistributionArray[i] = distributionArray[i];
                }

            }
            return monotonicDistributionArray;
        }


        public IPairedData SamplePairedData(double probability){
            double[] y = new double[_yvals.Length];
            for (int i=0;i<_xvals.Length; i++){
                y[i] = ys()[i].InverseCDF(probability);
            }
            return new PairedData(xs(), y, Category);
        }

        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("UncertainPairedData");
            masterElement.SetAttributeValue("Category", Category);
            masterElement.SetAttributeValue("XLabel", XLabel);
            masterElement.SetAttributeValue("YLabel", YLabel);
            masterElement.SetAttributeValue("Name", Name);
            masterElement.SetAttributeValue("Description", Description);
            masterElement.SetAttributeValue("ID", ID);
            masterElement.SetAttributeValue("Ordinate_Count", _xvals.Length);
            for (int i=0; i<_xvals.Length; i++)
            {
                XElement rowElement = new XElement("Coordinate");
                XElement xElement = new XElement("X");
                xElement.SetAttributeValue("Value", _xvals[i]);
                XElement yElement = _yvals[i].ToXML();
                rowElement.Add(xElement);
                rowElement.Add(yElement);
                masterElement.Add(rowElement);
            }
            return masterElement;
        }

        public static UncertainPairedData ReadFromXML(XElement element)
        {
            string category = element.Attribute("Category").Value;
            string xLabel = element.Attribute("XLabel").Value;
            string yLabel = element.Attribute("YLabel").Value;
            string name = element.Attribute("Name").Value;
            string description = element.Attribute("Description").Value;
            int id = Convert.ToInt32(element.Attribute("ID").Value);
            int size = Convert.ToInt32(element.Attribute("Ordinate_Count").Value);
            double[] xValues = new double[size];
            IDistribution[] yValues = new IDistribution[size];
            int i = 0;
            foreach(XElement coordinateElement in element.Elements())
            {
                foreach(XElement ordinateElements in coordinateElement.Elements())
                {
                    if (ordinateElements.Name.ToString().Equals("X"))
                    {
                        xValues[i] = Convert.ToDouble(ordinateElements.Attribute("Value").Value);
                    }
                    else
                    {
                        yValues[i] = Statistics.IDistributionExtensions.FromXML(ordinateElements);
                    }
                }
                i++;
            }
            return new UncertainPairedData(xValues,yValues,xLabel,yLabel,name,description,id,category);
        }
        #endregion
    }
}