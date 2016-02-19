using System;

namespace R54IN0.WPF
{
    public class UpdateLocker : IDisposable
    {
        private IUpdateLock _lock;

        public UpdateLocker(IUpdateLock ulock)
        {
            _lock = ulock;
            _lock.UpdateLock = false;
        }

        public void Dispose()
        {
            _lock.UpdateLock = true;
        }
    }
}