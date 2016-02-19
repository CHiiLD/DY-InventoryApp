﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MySQL.Test
{
    public static class LinqRandom
    {
        private static Random random = new Random();

        public static T Random<T>(this IEnumerable<T> items)
        {
            return items.ElementAt(random.Next(items.Count() - 1));
        }
    }
}