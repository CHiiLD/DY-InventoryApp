namespace R54IN0
{
    /// <summary>
    /// 추가, 수정, 삭제 버튼 객체의 커맨드
    /// </summary>
    public interface IButtonCommands
    {
        RelayCommand<object> AddCommand { get; set; }
        RelayCommand<object> RemoveCommand { get; set; }
        RelayCommand<object> ModifyCommand { get; set; }

        bool CanAddNewItem(object parameter);

        bool CanModifySelectedItem(object parameter);

        bool CanRemoveSelectedItem(object parameter);

        void ExecuteAddCommand(object parameter);

        void ExecuteModifyCommand(object parameter);

        void ExecuteRemoveCommand(object parameter);
    }
}