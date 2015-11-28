namespace R54IN0
{
    /// <summary>
    /// 단위
    /// </summary>
    public class Measure : IField
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public Measure()
        {

        }

        public Measure(Measure thiz)
        {
            UUID = thiz.UUID;
            Name = thiz.Name;
            IsDeleted = thiz.IsDeleted;
        }
    }
}