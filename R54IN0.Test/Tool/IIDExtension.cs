﻿using R54IN0.WPF;
using System;
using System.Diagnostics;

namespace R54IN0.Test
{
    public static class IIDExtension
    {
        public static T Save<T>(this IID iid) where T : class, IID
        {
            if (string.IsNullOrEmpty(iid.ID))
                iid.ID = Guid.NewGuid().ToString();

            DataDirector.GetInstance().DB.Insert(iid as T);

            return iid as T;
        }

        public static void Delete<T>(this IID iid) where T : class, IID
        {
            if (string.IsNullOrEmpty(iid.ID))
                Debug.Assert(false);

            DataDirector.GetInstance().DB.Delete(iid as T);
        }
    }
}