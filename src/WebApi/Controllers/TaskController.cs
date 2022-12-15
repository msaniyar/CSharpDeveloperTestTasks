using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class TaskController : ControllerBase
    {
        private readonly IStringReverseService _stringReverseService;
        private readonly INumberProcessorService _numberProcessorService;
        private readonly IFileHashService _fileHashService;
        private readonly IPriceGetterService _priceGetterService;

        public TaskController(IStringReverseService stringReverseService, 
            INumberProcessorService numberProcessorService,
            IFileHashService fileHashService,
            IPriceGetterService priceGetterService)
        {
            _stringReverseService = stringReverseService;
            _numberProcessorService = numberProcessorService;
            _fileHashService = fileHashService;
            _priceGetterService = priceGetterService;
        }

        /// <summary>
        /// Get Reversed string of input string.
        /// </summary>
        /// <param name="initialString"></param>
        /// <returns></returns>
        [HttpGet("GetReversedString")]
        public IActionResult GetReversedString(string initialString)
        {
            var result = _stringReverseService.ReverseString(initialString);
            return result == string.Empty ? BadRequest("String length should be more than 1") : Ok(result);
        }

        /// <summary>
        /// Run given number of processes async.
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        [HttpPost("ProcessNumbers")]
        public async Task<IActionResult> ProcessNumbers([FromBody] NumberProcessorRequestModel numbers)
        {
            var response = await _numberProcessorService.SendNumbers(numbers.Number);
            return response.Success == true ? Ok(response) : BadRequest(response);
        }


        /// <summary>
        /// Get file hash of given file in hex format.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpGet("FileHash")]
        public IActionResult GetFileHash(string filePath)
        {
            var result = _fileHashService.CalculateFileHash(filePath);
            return result == string.Empty ? NotFound("File does not exist.") : Ok(result);
        }


        /// <summary>
        /// Get current crypto prices of 100 assets.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPrices")]
        public async Task<IActionResult> GetPrices()
        {
            var response = await _priceGetterService.GetPrices();
            return response != null ? Ok(response) : NotFound("Prices cannot be fetched from external API.");
        }

    }
}
