namespace R54IN0
{
    /// <summary>
    /// 자사원
    /// </summary>
    public class Employee : IField
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public Employee()
        {

        }

        public Employee(Employee thiz)
        {
            UUID = thiz.UUID;
            Name = thiz.Name;
            IsDeleted = thiz.IsDeleted;
        }
    }
}