using System;

namespace R54IN0
{
    /// <summary>
    /// 단위
    /// </summary>
    public class Measure : AField
    {
        public const string HEADER = "단위";

        public Measure() : base()
        {
        }

        public Measure(string name) : base(name)
        {
        }
    }
}