using System;

namespace R54IN0
{
    /// <summary>
    /// 프로젝트명
    /// </summary>
    public class Project : IField
    {
        public const string HEADER = "프로젝트";

        public Project()
        {
        }

        public Project(string name)
        {
            Name = name;
            ID = Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string Name { get; set; }
    }
}