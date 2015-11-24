namespace R54IN0.Lib
{
    /// <summary>
    /// 화폐
    /// </summary>
    public class Currency : IBasic
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}