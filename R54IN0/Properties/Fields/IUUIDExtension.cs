using System;
using System.Diagnostics;
using Lex.Db;
using System.Linq;
using System.Collections;

namespace R54IN0
{
    public static class IUUIDExtension
    {
        public static T Save<T>(this IUUID iuuid) where T : class, IUUID
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                if (string.IsNullOrEmpty(iuuid.UUID))
                    iuuid.UUID = Guid.NewGuid().ToString();
                db.Save(iuuid as T);
            }
            return iuuid as T;
        }

        public static void Delete<T>(this IUUID iuuid) where T : class, IUUID
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                if (string.IsNullOrEmpty(iuuid.UUID))
                    Debug.Assert(false);
                db.Delete(iuuid as T);
            }
        }
    }
}