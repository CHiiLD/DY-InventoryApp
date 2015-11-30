using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public abstract class AViewModelMediatorColleague : IViewModelMediatorColleaugue
    {
        ViewModelMediator _mediator;

        public AViewModelMediatorColleague(ViewModelMediator mediator)
        {
            _mediator = mediator;
            _mediator.register(this);
        }

        public void Changed()
        {
            _mediator.OnViewModelChanged(this, null);
        }

        public void Changed(object args)
        {
            _mediator.OnViewModelChanged(this, args);
        }
    }
}
