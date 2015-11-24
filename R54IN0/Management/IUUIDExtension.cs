using System;
using System.Diagnostics;
using Lex.Db;

namespace R54IN0.Lib
{
    public static class IUUIDExtension
    {
        public static ClassT Save<ClassT>(this IUUID iuuid) where ClassT : class
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                if (string.IsNullOrEmpty(iuuid.UUID))
                    iuuid.UUID = Guid.NewGuid().ToString();
                db.Save(iuuid as ClassT);
            }
            return iuuid as ClassT;
        }

        public static void Delete<ClassT>(this IUUID iuuid) where ClassT : class
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                if (string.IsNullOrEmpty(iuuid.UUID))
                    Debug.Assert(false);
                db.Delete(iuuid as ClassT);
            }
        }
    }
}