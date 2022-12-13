using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class StringReverseService : IStringReverseService
    {

        /// <summary>
        /// Reversing incoming string and returns.
        /// </summary>
        /// <param name="initialString"></param>
        /// <returns></returns>
        public string ReverseString(string initialString)
        {
            var charArray = initialString.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
