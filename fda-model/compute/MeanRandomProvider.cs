using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using interfaces;

namespace compute
{
    public class MeanRandomProvider : IProvideRandomNumbers
    {
        public int Seed
        {
            get
            {
                return 0;
            }
        }
        public double NextRandom()
        {
            return .5;
        }

        public double[] NextRandomSequence(int size)
        {
            double[] randyPacket = new double[size];//needs to be initialized with a set of random nubmers between 0 and 1;
            for (int i = 0; i < size; i++)
            {
                randyPacket[i] = (((double)i) +.5)/ (double)size;
            }
            return randyPacket;
        }
    }
}
