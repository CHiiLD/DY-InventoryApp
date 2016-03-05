using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public static class IIDExtension
    {
        public static string CutString(this IID thiz, string str, int limitSize)
        {
            if (str != null)
            {
                int count = Encoding.UTF8.GetByteCount(str);
                if (limitSize < count)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(str);
                    str = Encoding.UTF8.GetString(bytes, 0, limitSize);
                }
            }
            return str;
        }
    }
}