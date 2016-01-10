namespace R54IN0
{
    /// <summary>
    /// 화폐
    /// </summary>
    public class Currency : IField
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}