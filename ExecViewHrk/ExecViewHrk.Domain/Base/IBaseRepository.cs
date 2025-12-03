using System;

namespace ExecViewHrk.Domain.Interface
{
    public interface IBaseRepository : IDisposable
    {
        bool CanDispose { get; set; }
        void Dispose(bool force);
    }
}
