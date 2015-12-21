using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public abstract class AFieldViewModelMediatorColleague<T> : IViewModelMediatorColleaugue where T : class, IField, new()
    {
        ViewModelMediator _mediator;

        public ObservableCollection<FieldPipe<T>> Items { get; set; }

        public AFieldViewModelMediatorColleague(ViewModelMediator mediator)
        {
            _mediator = mediator;
            _mediator.register(this);
        }

        public void OnFieldAdded(object parameter)
        {
            _mediator.OnFieldAdded(this, parameter);
        }

        public void OnFieldChanged(object parameter)
        {
            _mediator.OnFieldChanged(this, parameter);
        }

        public void OnFieldRemoved(object parameter)
        {
            _mediator.OnFieldRemoved(this, parameter);
        }
    }
}
