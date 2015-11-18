namespace DY.Inven
{
    /// <summary>
    /// 거래처
    /// </summary>
    public class Seller : IBasic
    {
        public string Name { get; set; }
        public string UUID { get; set; }
        public bool IsDeleted { get; set; }
        public string Delegator { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
    }
}