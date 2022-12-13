using System;

using Core.Models;

namespace Core.Interfaces
{
    public interface INumberProcessorService
    {
       Task<QueueServiceResponseModel> SendNumbers(int numbers);
    }
}
