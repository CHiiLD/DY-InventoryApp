namespace R54IN0.Lib
{
    /// <summary>
    /// 거래처
    /// </summary>
    public class Seller : IBasic
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string Delegator { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
    }
}