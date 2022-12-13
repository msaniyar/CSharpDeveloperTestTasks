using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IStringReverseService
    {
        /// <summary>
        /// Reversing incoming string and returns.
        /// </summary>
        /// <param name="initialString"></param>
        /// <returns></returns>
        string ReverseString(string initialString);
    }
}
