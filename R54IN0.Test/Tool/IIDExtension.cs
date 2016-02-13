using R54IN0.WPF;
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

            InventoryDataCommander.GetInstance().DB.Insert(iid);

            return iid as T;
        }

        public static void Delete<T>(this IID iid) where T : class, IID
        {
            if (string.IsNullOrEmpty(iid.ID))
                Debug.Assert(false);

            InventoryDataCommander.GetInstance().DB.Delete(iid);
        }
    }
}