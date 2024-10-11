using Microsoft.EntityFrameworkCore.Storage;
using Northwind.Dal.Abstractions;

namespace Northwind.Dal
{
    public class Transaction : ITransaction
    {
        private readonly IDbContextTransaction _efTransaction;

        public Transaction(IDbContextTransaction efTransaction)
        {
            _efTransaction = efTransaction;
        }

        public Task CommitAsync()
        {
            return _efTransaction.CommitAsync();
        }

        public Task RollbackAsync()
        {
            return _efTransaction.RollbackAsync();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            _efTransaction.Dispose();
        }

    }
}