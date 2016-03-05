using System;

namespace R54IN0
{
    /// <summary>
    /// 프로젝트명
    /// </summary>
    public class Project : AField
    {
        public const string HEADER = "프로젝트";

        public Project() : base()
        {
        }

        public Project(string name) : base(name)
        {
        }
    }
}