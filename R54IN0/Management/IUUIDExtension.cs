using System;
using System.Diagnostics;
using Lex.Db;

namespace DY.Inven
{
    public static class IUUIDExtension
    {
        private static DbInstance GetDbInstance(IUUID iuuid)
        {
            DbInstance db = null;
            if (iuuid is IBasic)
                db = DatabaseDirector.GetBase().GetDbInstance();
            else //if (iuuid is IInOutStock || iuuid is StockItem)
                db = DatabaseDirector.GetStock().GetDbInstance();
            return db;
        }

        public static ClassT Save<ClassT>(this IUUID iuuid) where ClassT : class
        {
            using (var db = GetDbInstance(iuuid))
            {
                if (string.IsNullOrEmpty(iuuid.UUID))
                    iuuid.UUID = Guid.NewGuid().ToString();
                db.Save(iuuid as ClassT);
            }
            return iuuid as ClassT;
        }

        public static void Delete<ClassT>(this IUUID iuuid) where ClassT : class
        {
            using (var db = GetDbInstance(iuuid))
            {
                if (string.IsNullOrEmpty(iuuid.UUID))
                    Debug.Assert(false);
                db.Delete(iuuid as ClassT);
            }
        }
    }
}