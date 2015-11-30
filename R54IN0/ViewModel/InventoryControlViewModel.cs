
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class InventoryControlViewModel
    {
        InventoryFinderViewModel _finder;
        InventoryDataGridViewModel _dataGrid;

        public InventoryControlViewModel(InventoryFinderViewModel finder, InventoryDataGridViewModel dataGrid)
        {
            _finder = finder;
            _dataGrid = dataGrid;
            _finder.SelectingAction = OnSelecting;
        }

        void OnSelecting(InventoryFinderViewModel finderViewModel)
        {
            IEnumerable<ItemNode> itemNodes = finderViewModel.SelectedNodes.SelectMany(x => x.Descendants().OfType<ItemNode>());
            itemNodes = itemNodes.Distinct();
            List<InventoryPipe> pipeList = new List<InventoryPipe>();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Inventory[] invens = db.LoadAll<Inventory>();
                foreach (var itemNode in itemNodes)
                {
                    IEnumerable<Inventory> inventory = invens.Where(x => x.ItemUUID == itemNode.ItemUUID);
                    pipeList.AddRange(inventory.Select(x => new InventoryPipe(x)));
                }
            }
            _dataGrid.ChangeInventoryItems(pipeList);
        }
    }
}