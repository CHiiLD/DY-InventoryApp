using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public abstract class AFinderViewModelMediatorColleague
    {
        FinderViewModelMediator _mediator;

        public AFinderViewModelMediatorColleague(FinderViewModelMediator mediator)
        {
            _mediator = mediator;
            _mediator.register(this);
        }

        public void UpdateFinderItems(ItemPipe item, bool isAddAction)
        {
            _mediator.OnItemPipeCollectionChanged(item, isAddAction);
        }

        public void UpdateInventoryDataGridItems(InventoryFinderViewModel finderViewModel)
        {
            _mediator.OnFinderNodesSelected(finderViewModel);
        }
    }
}