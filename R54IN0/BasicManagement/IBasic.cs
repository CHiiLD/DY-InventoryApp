using System;

namespace DY.Inven
{
    public interface IBasic
    {
        string Name { get; set; }
        string UUID { get; set; }
        bool IsEnable { get; set; }
    }

    public static class IBasicExtension
    {
        public static string NewUUID(this IBasic iBase)
        {
            return Guid.NewGuid().ToString();
        }
    }
}