using System;

namespace CyclicDominoes
{
    public class RandomProvider : IRandomProvider
    {
        private readonly Random _random;
        
        public RandomProvider(int? seed = null)
        {
            _random = seed != null ? new Random(seed.Value) : new Random();
        }

        public int GetNextRandom(int minValue, int maxValue)
            => _random.Next(minValue, maxValue);
    }
}