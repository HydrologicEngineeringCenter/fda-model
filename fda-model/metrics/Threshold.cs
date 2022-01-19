﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using paireddata;

namespace metrics
{
    public class Threshold
    {
        public ThresholdEnum ThresholdType { get; set; }
        public double ThresholdValue { get; set; }
        public ProjectPerformanceResults ProjectPerformanceResults { get; set; }
        /// <summary>
        /// Threshold ID should be an integer greater than or equal to 1. 
        /// The threshold ID = 0 is reserved for the default threshold.
        /// </summary>
        public int ThresholdID { get; }

        public Threshold(int thresholdID, ThresholdEnum thresholdType=0, double thresholdValue=0)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            ProjectPerformanceResults = new ProjectPerformanceResults(thresholdType, thresholdValue); 
            ThresholdID = thresholdID;
        }

        public Threshold(int thresholdID, UncertainPairedData leveeCurve, ThresholdEnum thresholdType = 0, double thresholdValue = 0)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            ProjectPerformanceResults = new ProjectPerformanceResults(thresholdType, thresholdValue, leveeCurve);
            ThresholdID = thresholdID;
        }


    }
}