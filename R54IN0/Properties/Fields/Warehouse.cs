﻿namespace R54IN0
{
    /// <summary>
    /// 보관 장소 및 창고
    /// </summary>
    public class Warehouse : IField
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public Warehouse()
        {

        }

        public Warehouse(Warehouse thiz)
        {
            UUID = thiz.UUID;
            Name = thiz.Name;
            IsDeleted = thiz.IsDeleted;
        }
    }
}