using Core.Models;

namespace Core.Interfaces
{
    public interface IPriceGetterService
    {
        /// <summary>
        /// Get Crypto prices of first 100 crypto markets.
        /// </summary>
        /// <returns></returns>
        Task<Prices> GetPrices();
    }
}
