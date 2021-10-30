﻿using System.Threading.Tasks;

namespace Cko.PaymentGateway.Infrastructure.Interfaces
{
    public interface IDocumentPersistance<T>
    {
        Task<T> RetrieveDocumentByKeyAsync(string key);
        Task StoreDocumentAsync(T obj);
    }
}
