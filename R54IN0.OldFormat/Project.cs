using System;

namespace R54IN0
{
    /// <summary>
    /// 프로젝트명
    /// </summary>
    public class Project : IField
    {
        public Project()
        {
        }
        public bool IsDeleted { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
    }
}