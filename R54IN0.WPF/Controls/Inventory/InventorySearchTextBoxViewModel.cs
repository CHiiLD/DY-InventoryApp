using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class InventorySearchTextBoxViewModel
    {
        /// <summary>
        /// TextBox Search Button Command Binding
        /// </summary>
        public ICommand SearchCommand { get; set; }

        /// <summary>
        /// TextBox binding
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 검색 실행
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ObservableInventory> Search()
        {
            var fwd = ObservableInventoryDirector.GetInstance();
            var list = fwd.CreateList();
            if (string.IsNullOrEmpty(Text))
                return list;
            string[] keywords = Text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var result = keywords.SelectMany(word => list.Where(inven => inven.Specification.Contains(word) || inven.Product.Name.Contains(word)));
            return result;
        }
    }
}