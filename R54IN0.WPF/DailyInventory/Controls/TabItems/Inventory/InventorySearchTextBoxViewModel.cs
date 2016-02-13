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
            var inventories = InventoryDataCommander.GetInstance().CopyInventories();
            if (string.IsNullOrEmpty(Text))
                return inventories;
            string[] keywords = Text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var lowerKeywords = keywords.Select(x => x.ToLower());
            var result = lowerKeywords.SelectMany(word =>
            inventories.Where(inven => inven.Specification.ToLower().Contains(word) || inven.Product.Name.ToLower().Contains(word)));
            return result;
        }
    }
}