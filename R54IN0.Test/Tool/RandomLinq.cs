using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Test
{
    public static class LinqRandom
    {
        static Random random = new Random();

        public static T Random<T>(this IList<T> items)
        {
            return items.ElementAt(random.Next(items.Count() - 1));
        }
    }
}
