using Core.Models;
using Core.Models.Configuration;
using Infraestructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class ClientsFtpUploadAppService
    {
        private readonly ILogger<ClientsFtpUploadAppService> _logger;
        private readonly AppConfiguration _config;
        private readonly BQService _BQ;
        private readonly FtpService _ftpService;
        public ClientsFtpUploadAppService(ILogger<ClientsFtpUploadAppService> logger, AppConfiguration config, BQService BQ, FtpService ftpService)
        {
            _logger = logger;
            _config = config;
            _BQ = BQ;
            _ftpService = ftpService;
        }
        public Response<bool> upload() {
            bool respuesta = true;
            try
            {
                string sql = "SELECT Name,Host,PublicHost,Port,User,Password,Fingerprint FROM `operation-datalake.Abai_Aplications_Configurations.FtpConfigurations` WHERE Enabled = true";
                var ftps = _BQ.GetData<FtpConfiguration>("operation-datalake", sql, out string errorBQ);
                if(!ftps.Any())
                {
                    return new(respuesta, "");
                }
                foreach (var ftp in ftps)
                {
                    var ftpAntiguo = _config.FtpConfigs.FirstOrDefault(x => x.Name == ftp.Name);
                    if(ftpAntiguo != null)
                    {
                        ftpAntiguo.PublicHost = ftp.PublicHost;
                        ftpAntiguo.Port = ftp.Port;
                        ftpAntiguo.User = ftp.User;
                        ftpAntiguo.Host = ftp.Host;
                        ftpAntiguo.Password = ftp.Password;
                        ftpAntiguo.Fingerprint= ftp.Fingerprint;
                    }
                    else
                    {
                        _config.FtpConfigs.Add(ftp);
                    }
                }
            }
            catch (Exception)
            {

            }
            return new ( respuesta,"" );
        }
    }
}
