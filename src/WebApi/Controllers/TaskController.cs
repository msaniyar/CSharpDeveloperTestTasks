using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class TaskController : ControllerBase
    {
        private readonly IStringReverseService _stringReverseService;
        private readonly INumberProcessorService _numberProcessorService;
        private readonly IFileHashService _fileHashService;

        public TaskController(IStringReverseService stringReverseService, 
            INumberProcessorService numberProcessorService,
            IFileHashService fileHashService)
        {
            _stringReverseService = stringReverseService;
            _numberProcessorService = numberProcessorService;
            _fileHashService = fileHashService;
        }


        [HttpGet("GetReversedString")]
        public IActionResult GetReversedString(string initialString)
        {
            var result = _stringReverseService.ReverseString(initialString);
            return result == string.Empty ? BadRequest("String length should be more than 1") : Ok(result);
        }

        [HttpPost("ProcessNumbers")]
        public async Task<IActionResult> ProcessNumbers(int numbers)
        {
            var response = await _numberProcessorService.SendNumbers(numbers);
            return response.Success == true ? Ok(response) : BadRequest(response);
        }

        [HttpGet("FileHash")]
        public IActionResult GetFileHash(string filePath)
        {
            var result = _fileHashService.CalculateFileHash(filePath);
            return result == string.Empty ? BadRequest("File does not exist.") : Ok(result);
        }

    }
}
