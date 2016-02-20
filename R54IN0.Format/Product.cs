﻿namespace R54IN0
{
    /// <summary>
    /// 제품명
    /// </summary>
    public class Product : IField
    {
        public const string HEADER = "제품";

        public string ID { get; set; }
        public string Name { get; set; }
    }
}