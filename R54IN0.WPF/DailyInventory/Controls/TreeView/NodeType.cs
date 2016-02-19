namespace R54IN0.WPF
{
    public enum NodeType
    {
        NONE = 0,     //NULL
        FOLDER = 1 << 0,   //폴더
        PRODUCT = 1 << 1,  //제품
        INVENTORY = 1 << 2 //규격
    }
}