namespace R54IN0
{
    /// <summary>
    /// 보관 장소 및 창고
    /// </summary>
    public class Warehouse : IBasic
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}