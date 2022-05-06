using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Enumerations;

namespace metrics
{
    public class DamageResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields
        private List<DamageResult> _damageResultList;
        private int _impactAreaID;
        #endregion

        #region Properties 
        public List<DamageResult> DamageResultList
        {
            get
            {
                return _damageResultList;
            }
        }
        public int ImpactAreaID
        {
            get
            {
                return _impactAreaID;
            }
        }
        public event MessageReportedEventHandler MessageReport;

        #endregion
        #region Constructors
        public DamageResults(int impactAreaID){
            _damageResultList = new List<DamageResult>();
            _impactAreaID = impactAreaID;
        }
        private DamageResults(List<DamageResult> expectedAnnualDamageResults, int impactAreaID)
        {
            _damageResultList = expectedAnnualDamageResults;
            _impactAreaID = impactAreaID;
        }
        #endregion

        #region Methods 
        public void AddDamageResultObject(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
        {
            foreach (DamageResult damageResult in _damageResultList)
            {
                if (!damageResult.ImpactAreaID.Equals(impactAreaID))
                {
                    if (!damageResult.DamageCategory.Equals(damageCategory))
                    {
                        if (!damageResult.AssetCategory.Equals(assetCategory))
                        {
                            DamageResult newDamageResult = new DamageResult(damageCategory, assetCategory, convergenceCriteria, impactAreaID);
                            _damageResultList.Add(newDamageResult);
                        }
                    }
                }

            }
        }
        public void AddDamageRealization(double dammageEstimate, string damageCategory, string assetCategory, int impactAreaID, Int64 iteration)
        {
            DamageResult damageResult = GetDamageResult(damageCategory, assetCategory, impactAreaID);
            damageResult.AddDamageRealization(dammageEstimate, iteration);

        }
        public double MeanDamage(string damageCategory, string assetCategory, int impactAreaID)
        {
            DamageResult damageResult = GetDamageResult(damageCategory, assetCategory, impactAreaID);
            return damageResult.MeanDamage();
        }

        public double DamageExceededWithProbabilityQ(string damageCategory, double exceedanceProbability, string assetCategory, int impactAreaID)
        {
            DamageResult damageResult = GetDamageResult(damageCategory, assetCategory, impactAreaID);
            double quantileRequested = damageResult.DamageExceededWithProbabilityQ(exceedanceProbability);
            return quantileRequested;

        }
        public DamageResult GetDamageResult(string damageCategory, string assetCategory, int impactAreaID)
        {
            foreach (DamageResult damageResult in _damageResultList)
            {
                if (damageResult.ImpactAreaID.Equals(impactAreaID))
                {
                    if (damageResult.DamageCategory.Equals(damageCategory))
                    {
                        if (damageResult.AssetCategory.Equals(assetCategory))
                        {
                            return damageResult;
                        } 
                    }
                }
            }
            ReportMessage(this, new MessageEventArgs(new Message("The requested damage category - asset category combination could not be found. An arbitrary object is being returned.")));
            DamageResult dummyResult = new DamageResult();
            return dummyResult;
        }
        public bool Equals(DamageResults inputDamageResults)
        {
           foreach (DamageResult damageResult in _damageResultList)
           {
               DamageResult inputDamageResult = inputDamageResults.GetDamageResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.ImpactAreaID);
               bool resultsMatch = damageResult.Equals(inputDamageResult);
               if (!resultsMatch)
               {
                   return false;
               }
            }
            return true;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        public XElement WriteToXML()
        {
            XElement masterElem = new XElement("EAD_Results");
            foreach (DamageResult damageResult in _damageResultList)
            {
                XElement damageResultElement = damageResult.WriteToXML();
                damageResultElement.Name = $"{damageResult.DamageCategory}-{damageResult.AssetCategory}";
                masterElem.Add(damageResultElement);
            }
            masterElem.SetAttributeValue("ImpactAreaID", _impactAreaID);
            return masterElem;
        }

        public static DamageResults ReadFromXML(XElement xElement)
        {
            List<DamageResult> damageResults = new List<DamageResult>();
            foreach (XElement histogramElement in xElement.Elements())
            {
                damageResults.Add(DamageResult.ReadFromXML(histogramElement));
            }
            int impactAreaID = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new DamageResults(damageResults,impactAreaID);
        }

        #endregion
    }
}