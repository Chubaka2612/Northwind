using System;
using System.Threading.Tasks;

namespace Northwind.Dal.Abstractions
{
    public interface ITransaction : IDisposable
    {
        Task CommitAsync();

        Task RollbackAsync();
    }
}