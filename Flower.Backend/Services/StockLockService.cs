using Microsoft.Extensions.Caching.Memory;
using System;

namespace Flower.Backend.Services
{
    public class StockLockService
    {
        private readonly IMemoryCache _cache;
        private static readonly object LockObj = new();

        public StockLockService(IMemoryCache cache)
        {
            _cache = cache;
        }

        // Giữ kho tạm thời trong 15 phút
        public bool ReserveStock(int productId, int quantity, TimeSpan ttl)
        {
            lock (LockObj)
            {
                string key = $"stock_reserved:{productId}";
                int currentReserved = _cache.Get<int?>(key) ?? 0;
                _cache.Set(key, currentReserved + quantity, ttl);
                return true;
            }
        }

        public int GetReservedStock(int productId)
        {
            lock (LockObj)
            {
                return _cache.Get<int?>($"stock_reserved:{productId}") ?? 0;
            }
        }

        public void ReleaseReservedStock(int productId, int quantity)
        {
            lock (LockObj)
            {
                string key = $"stock_reserved:{productId}";
                int currentReserved = _cache.Get<int?>(key) ?? 0;
                int newReserved = Math.Max(0, currentReserved - quantity);
                
                if (newReserved == 0)
                    _cache.Remove(key);
                else
                    _cache.Set(key, newReserved, TimeSpan.FromMinutes(15));
            }
        }
    }
}
