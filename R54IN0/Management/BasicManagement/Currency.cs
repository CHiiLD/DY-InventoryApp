namespace DY.Inven
{
    /// <summary>
    /// 화폐
    /// </summary>
    public class Currency : IBasic
    {
        public string Name { get; set; }
        public string UUID { get; set; }
        public bool IsDeleted { get; set; }
    }
}