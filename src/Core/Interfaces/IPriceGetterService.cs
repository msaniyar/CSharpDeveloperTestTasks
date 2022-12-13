using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
