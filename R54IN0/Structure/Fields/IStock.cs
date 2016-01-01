using System;

namespace R54IN0
{
    public interface IStock : ICloneable
    {
        string ItemID { get; set; }
        string SpecificationID { get; set; }
        int Quantity { get; set; }
        string Remark { get; set; }
    }
}