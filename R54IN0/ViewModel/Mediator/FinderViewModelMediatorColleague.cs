using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public abstract class FinderViewModelMediatorColleague : IDisposable
    {
        FinderViewModelMediator _mediator;

        public FinderViewModelMediatorColleague(FinderViewModelMediator mediator)
        {
            if (mediator != null)
            {
                _mediator = mediator;
                _mediator.Register(this);
            }
        }

        ~FinderViewModelMediatorColleague()
        {
            Dispose();
        }

        public void Dispose()
        {
            _mediator.Cancellation(this);
            GC.SuppressFinalize(this);
        }

        public void UpdateItemPipeCollection(ItemPipe item, CollectionAction action)
        {
            if (_mediator != null)
                _mediator.OnItemPipeCollectionChanged(item, action);
        }

        public void ShowSelectedFinderNodes(InventoryFinderViewModel finderViewModel)
        {
            if (_mediator != null)
                _mediator.OnFinderNodesSelected(finderViewModel);
        }
    }
}