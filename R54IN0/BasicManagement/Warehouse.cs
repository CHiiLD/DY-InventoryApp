namespace DY.Inven
{
    /// <summary>
    /// 보관 장소 및 창고
    /// </summary>
    public class Warehouse : IBasic
    {
        public string Name { get; set; }
        public string UUID { get; set; }
        public bool IsEnable { get; set; }
    }
}