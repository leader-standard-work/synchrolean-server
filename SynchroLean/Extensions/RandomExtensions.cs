using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynchroLean.Extensions
{
    public static class RandomExtensions
    {
        public static T SampleFrom<T>(this Random rng, IEnumerable<T> possibilities)
        {
            var outcome = rng.NextDouble() * possibilities.Count();
            return possibilities.ElementAt((int)Math.Floor(outcome));
        }
    }
}
