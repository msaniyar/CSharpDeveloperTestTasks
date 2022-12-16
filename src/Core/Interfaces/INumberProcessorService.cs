using Core.Models;

namespace Core.Interfaces
{
    public interface INumberProcessorService
    {
        /// <summary>
        /// Takes the numbers and sends them to processor asynchronously.
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        Task<QueueServiceResponseModel> SendNumbers(int numbers);
    }
}
