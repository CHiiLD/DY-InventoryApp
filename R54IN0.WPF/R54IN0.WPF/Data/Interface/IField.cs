namespace R54IN0
{
    public interface IField : IID, IName
    {
        bool IsDeleted { get; set; }
    }
}