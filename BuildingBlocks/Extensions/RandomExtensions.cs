using System.Globalization;
using System.Linq;
using BuildingBlocks.Common;

namespace BuildingBlocks.Extensions
{
    public static class RandomExtensions
    {
        public static double GenerateRandomBetween(this Random random, double min, double max)
        {
            Ensure.That(min <= max, $"min: {min.ToString(CultureInfo.InvariantCulture)} should be less than max: {max.ToString(CultureInfo.InvariantCulture)}");
            return random.NextDouble() * (max - min) + min;
        }

        public static int[] GenerateRandomSequence(this Random random, int count, int min, int max)
        {
            if (max <= min || count < 0 || count > max - min)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count), 
                    $"The given range of {min} to {max} ({max - min} values), with the count of {count} is invalid."
                );
            }

            var numbers = new HashSet<int>();
            for (int i = max - count; i < max; i++)
            {
                int next;
                do
                {
                    next = random.Next(min, i + 1);
                } while (!numbers.Add(next));
                
                if (!numbers.Add(i))
                    numbers.Add(next);
            }

            return numbers.OrderBy(_ => random.Next()).ToArray();
        }
    }
}