using System;
using System.Diagnostics;

namespace R54IN0.Test
{
    public static class IIDExtension
    {
        public static T Save<T>(this IID id) where T : class, IID
        {
            if (string.IsNullOrEmpty(id.ID))
                id.ID = Guid.NewGuid().ToString();
            using (var db = LexDb.GetDbInstance())
                db.Save(id as T);
            return id as T;
        }

        public static void Delete<T>(this IID id) where T : class, IID
        {
            if (string.IsNullOrEmpty(id.ID))
                Debug.Assert(false);
            using (var db = LexDb.GetDbInstance())
                db.Delete(id as T);
        }
    }
}