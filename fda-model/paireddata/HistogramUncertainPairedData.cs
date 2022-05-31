using Statistics;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using paireddata;

namespace fda_model.paireddata
{
    internal class HistogramUncertainPairedData : UncertainPairedData
    {
        private ThreadsafeInlineHistogram[] _histograms;

        public HistogramUncertainPairedData(double[] xs, CurveMetaData metaData) : base(xs,null,metaData)
        {

        }
    }
}
