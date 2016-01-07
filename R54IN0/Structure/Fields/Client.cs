﻿namespace R54IN0
{
    /// <summary>
    /// 거래처
    /// </summary>
    public class Client : IField
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string Delegator { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
    }
}