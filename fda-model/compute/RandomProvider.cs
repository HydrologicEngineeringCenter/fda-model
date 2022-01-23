﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using interfaces;

namespace compute
{
    public class RandomProvider : IProvideRandomNumbers
    {
        private Random _rng;
        private object _sampleLock = new object();
        private object _sequenceLock = new object();
        public RandomProvider()
        {
            _rng = new Random();
        }
        public RandomProvider(int seed)
        {
            _rng = new Random(seed);
        }
        public double NextRandom()
        {
            lock (_sampleLock)
            {
                return _rng.NextDouble();
            }
            
        }
        public double[] NextRandomSequence(int size)
        {
            lock (_sequenceLock)
            {
                double[] randyPacket = new double[size];//needs to be initialized with a set of random nubmers between 0 and 1;
                for (int i = 0; i < size; i++)
                {
                    randyPacket[i] = _rng.NextDouble();
                }
                return randyPacket;
            }

        }
    }
}
