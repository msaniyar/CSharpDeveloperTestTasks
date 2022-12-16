
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
