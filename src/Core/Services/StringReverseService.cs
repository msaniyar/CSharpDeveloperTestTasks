using Core.Interfaces;

namespace Core.Services
{
    public class StringReverseService : IStringReverseService
    {

        /// <inheritdoc />
        public string ReverseString(string initialString)
        {
            if(string.IsNullOrEmpty(initialString)) return string.Empty;
            var charArray = initialString.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
