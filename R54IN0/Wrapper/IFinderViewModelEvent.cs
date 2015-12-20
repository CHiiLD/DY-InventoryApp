using System;

namespace R54IN0
{
    public interface IFinderViewModelEvent
    {
        void OnFinderViewSelectItemChanged(object sender, EventArgs e);
    }
}