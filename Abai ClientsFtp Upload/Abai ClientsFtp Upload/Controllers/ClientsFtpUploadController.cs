using Core.Models;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Abai_ClientsFtp_Upload.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientsFtpUploadController : ControllerBase
    {
        

        private readonly ILogger<ClientsFtpUploadController> _logger;
        private readonly ClientsFtpUploadAppService _clientsFtpUploadAppService;
        public ClientsFtpUploadController(ILogger<ClientsFtpUploadController> logger, ClientsFtpUploadAppService clientsFtpUploadAppService)
        {
            _logger = logger;
            _clientsFtpUploadAppService = clientsFtpUploadAppService;
        }

        [HttpGet("test")]
        public Response<bool> TmaReport()
        {
            try
            {
                var respuesta = _clientsFtpUploadAppService.upload();

                return respuesta;
            }
            catch (Exception ex)
            {
                _logger.LogError("TmaReport controller: exception = ");
                _logger.LogError(ex.Message, ex);
                return new(false, "TmaReport controller: exception = " + ex.Message);
            }
        }
    }
}