using System;

namespace R54IN0
{
    public interface IFinderViewModelCallback
    {
        void OnFinderViewSelectItemChanged(object sender, EventArgs e);
    }
}