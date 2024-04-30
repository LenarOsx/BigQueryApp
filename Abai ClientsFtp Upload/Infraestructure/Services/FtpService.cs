
using Core.Models.Configuration;
using Microsoft.Extensions.Logging;
using WinSCP;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;

namespace Infraestructure.Services
{
    public class FtpService
    {
        private readonly ILogger<FtpService> _logger;
        private Dictionary<string, Session> _sessions;

        public FtpService(ILogger<FtpService> logger)
        {


            _sessions = new Dictionary<string, Session>();
            _logger = logger;
        }



        private bool OpenSession(Session session, FtpConfiguration ftpConfiguration)
        {
            try
            {
                var sessionOptions = SetSessionOption(ftpConfiguration);
                var TryOpenSession = ftpConfiguration.TryOpenSession;
                var cont = 0;
                while (cont < TryOpenSession)
                {
                    try
                    {
                        session.Open(sessionOptions);
                        if (session.Opened)
                        {
                            _logger.LogInformation($"FTP session opened (try: {cont})");
                            return true;
                        }
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"FTP session failed. Next try ({cont})");
                    }
                    cont++;
                }
                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError("OpenSession error");
                _logger.LogError($"Error {ex.Message}");
                throw;
            }
        }

        private SessionOptions SetSessionOption(FtpConfiguration ftpConfiguration)
        {
            return new SessionOptions()
            {
                Protocol = Protocol.Sftp,
                HostName = ftpConfiguration.Host,
                UserName = ftpConfiguration.User,
                Password = ftpConfiguration.Password,
                SshHostKeyFingerprint = ftpConfiguration.Fingerprint,
                PortNumber = ftpConfiguration.Port
            };
        }
        public bool UploadFileSftp(string fromFilePath, string toFilePath, ReportFtpConfiguration reportFtpConfig, FtpConfiguration ftpConfiguration, out string error, bool removeFileAfterUpload = false)
        {
            try
            {
                Session session;
                var sessionDictionary = _sessions.FirstOrDefault(sd => sd.Key.ToLower() == ftpConfiguration.Name.ToLower());
                if (sessionDictionary.Key != null)
                {
                    session = sessionDictionary.Value;
                }
                else
                {
                    session = new Session() { Timeout = new TimeSpan(0, 0, 10) };
                    _sessions.Add(ftpConfiguration.Name, session);
                }
                if (!session.Opened)
                {
                    if (OpenSession(session, ftpConfiguration))
                    {
                        error = "";
                        var respuesta = UploadFileSftp(fromFilePath, toFilePath, reportFtpConfig, ftpConfiguration, out string errorx, removeFileAfterUpload);
                        error = errorx;
                        return respuesta;
                    }
                    _logger.LogInformation($"Could not open ftp session");
                    error = "Could not open ftp session";
                    return false;
                }

                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Binary;
                transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;
                transferOptions.OverwriteMode = OverwriteMode.Overwrite;
                //transferOptions.FilePermissions = null;
                //transferOptions.PreserveTimestamp = false;

                if (reportFtpConfig.CreateDirectory)
                {
                    bool esCreado = CreateDirectory(reportFtpConfig.FtpPath, session, ftpConfiguration);
                }
                var nombreArchivo = Path.GetFileName(fromFilePath);
                var nombreArchivoServer = reportFtpConfig.FtpPath + nombreArchivo;

                if (!nombreArchivo.Contains(".*") && session.FileExists(nombreArchivoServer) && nombreArchivo != "")
                {
                    error = $"El archivo [{nombreArchivoServer}] ya existe en este directorio [{reportFtpConfig.FtpPath}]";
                    FileInfo archivoExiste = new FileInfo(fromFilePath);
                    DirectoryInfo respaldo = new DirectoryInfo(archivoExiste.Directory.Parent.FullName + "Respaldo_Error\\");
                    if (!respaldo.Exists)
                    {
                        respaldo.Create();
                    }
                    archivoExiste.CopyTo(respaldo.FullName + archivoExiste.Name, true);
                    if (archivoExiste.Exists) archivoExiste.Delete();
                    _logger.LogInformation(error);
                    return false;
                }
                if (!nombreArchivo.Contains("*"))
                {
                    FileInfo archivoExisteTemp = new FileInfo(fromFilePath);
                    DirectoryInfo respaldoTemp = new DirectoryInfo(archivoExisteTemp.Directory.Parent.FullName + "Respaldo_Temporal\\");
                    if (!respaldoTemp.Exists)
                    {
                        respaldoTemp.Create();
                    }
                    archivoExisteTemp.CopyTo(respaldoTemp.FullName + archivoExisteTemp.Name, true);
                }
                else
                {
                    var path = Path.GetDirectoryName(fromFilePath);
                    DirectoryInfo directory = new DirectoryInfo(path);
                    DirectoryInfo respaldoTemp = new DirectoryInfo(directory.Parent.FullName + "Respaldo_Temporal\\");
                    if (!respaldoTemp.Exists)
                    {
                        respaldoTemp.Create();
                    }
                    foreach (var file in directory.GetFiles())
                    {
                        file.CopyTo(respaldoTemp.FullName + file.Name, true);
                    }

                }
                //if (session.FileExists(nombreArchivoServer))
                //{
                //    RemovalOperationResult removalResult = session.RemoveFiles(nombreArchivoServer);

                //    if (!removalResult.IsSuccess)
                //    {
                //        error = "Subir archivo SFTP: No se pudo borrar el archivo en el servidor ftp";
                //        return false;
                //    }
                //}

                TransferOperationResult transferOperationResult = session.PutFiles(fromFilePath, toFilePath, reportFtpConfig.DeleteFilesAfter, transferOptions);
                transferOperationResult.Check();
                if (reportFtpConfig.DeleteFilesAfter)
                {
                    foreach (TransferEventArgs transfer in transferOperationResult.Transfers)
                    {
                        RemovalOperationResult removalResult = session.RemoveFiles(session.EscapeFileMask(transfer.FileName));

                        if (!removalResult.IsSuccess)
                        {
                            //eventLogUtility.WriteToEventLog("There was an error removing the file: " + transfer.FileName + " from " + sessionOptions.HostName + ".", EventLogEntryType.LogError);
                        }
                    }
                }
                error = string.Empty;
                return true;
            }
            catch (SessionLocalException sle)
            {
                string errorDetail = "WinSCP: There was an error communicating with winscp process. winscp cannot be found or executed.";
                errorDetail += Environment.NewLine + "Message:" + sle.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sle.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sle.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sle.StackTrace;
                //eventLogUtility.WriteToEventLog(errorDetail, EventLogEntryType.LogError);
                error = errorDetail;
                return false;
            }
            catch (SessionRemoteException sre)
            {
                string errorDetail = "WinSCP: Error is reported by the remote server; Local error occurs in WinSCP console session, such as error reading local file.";
                errorDetail += Environment.NewLine + "Message:" + sre.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sre.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sre.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sre.StackTrace;
                error = errorDetail;
                return false;
                //eventLogUtility.WriteToEventLog(errorDetail, EventLogEntryType.LogError);
            }
            catch (Exception ex)
            {
                error = "Subir archivo SFTP : Ocurrio una excepcion " + ex.Message;
                return false;
                //eventLogUtility.WriteToEventLog("Error in ProcessEdi() while downloading EDI files via FTP: Message:" + ex.Message + "Stacktrace: " + ex.StackTrace, EventLogEntryType.LogError);
            }
        }

        public bool DownloadFileSftp(string fromFilePath, string toFilePath, ReportFtpConfiguration reportFtpConfig, FtpConfiguration ftpConfiguration, out string error, bool removeFileAfterUpload = false)
        {
            try
            {
                Session session;
                var sessionDictionary = _sessions.FirstOrDefault(sd => sd.Key.ToLower() == ftpConfiguration.Name.ToLower());
                if (sessionDictionary.Key != null)
                {
                    session = sessionDictionary.Value;
                }
                else
                {
                    session = new Session() { Timeout = new TimeSpan(0, 0, 10) };
                    _sessions.Add(ftpConfiguration.Name, session);
                }
                if (!session.Opened)
                {
                    if (OpenSession(session, ftpConfiguration))
                    {
                        error = "";
                        var respuesta = DownloadFileSftp(fromFilePath, toFilePath, reportFtpConfig, ftpConfiguration, out string errorx, removeFileAfterUpload);
                        error = errorx;
                        return respuesta;
                    }
                    _logger.LogInformation($"Could not open ftp session");
                    error = "Could not open ftp session";
                    return false;
                }

                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Binary;
                transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;
                transferOptions.OverwriteMode = OverwriteMode.Overwrite;

                string nombreArchivoServer = Path.GetFileName(fromFilePath);
                if (!session.FileExists(fromFilePath))
                {
                    error = $"El archivo [{nombreArchivoServer}] no se encontró en el ftp en el directorio [{reportFtpConfig.FtpPath}]";
                    _logger.LogInformation(error);
                    return false;
                }

                TransferOperationResult transferOperationResult = session.GetFiles(fromFilePath, toFilePath, false, transferOptions);
                transferOperationResult.Check();
                //if (reportFtpConfig.DeleteFilesAfter)
                //{
                //    foreach (TransferEventArgs transfer in transferOperationResult.Transfers)
                //    {
                //        RemovalOperationResult removalResult = session.RemoveFiles(session.EscapeFileMask(transfer.FileName));

                //        if (!removalResult.IsSuccess)
                //        {
                //            //eventLogUtility.WriteToEventLog("There was an error removing the file: " + transfer.FileName + " from " + sessionOptions.HostName + ".", EventLogEntryType.LogError);
                //        }
                //    }
                //}
                error = string.Empty;
                return true;
            }
            catch (SessionLocalException sle)
            {
                string errorDetail = "WinSCP: There was an error communicating with winscp process. winscp cannot be found or executed.";
                errorDetail += Environment.NewLine + "Message:" + sle.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sle.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sle.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sle.StackTrace;
                //eventLogUtility.WriteToEventLog(errorDetail, EventLogEntryType.LogError);
                error = errorDetail;
                return false;
            }
            catch (SessionRemoteException sre)
            {
                string errorDetail = "WinSCP: Error is reported by the remote server; Local error occurs in WinSCP console session, such as error reading local file.";
                errorDetail += Environment.NewLine + "Message:" + sre.Message;
                errorDetail += Environment.NewLine + "Target Site:" + sre.TargetSite;
                errorDetail += Environment.NewLine + "Inner Exception:" + sre.InnerException;
                errorDetail += Environment.NewLine + "Stacktrace: " + sre.StackTrace;
                error = errorDetail;
                return false;
                //eventLogUtility.WriteToEventLog(errorDetail, EventLogEntryType.LogError);
            }
            catch (Exception ex)
            {
                error = "Subir archivo SFTP : Ocurrio una excepcion " + ex.Message;
                return false;
                //eventLogUtility.WriteToEventLog("Error in ProcessEdi() while downloading EDI files via FTP: Message:" + ex.Message + "Stacktrace: " + ex.StackTrace, EventLogEntryType.LogError);
            }
        }

        public bool CreateDirectory(string path, Session session, FtpConfiguration ftpConfiguration)
        {
            try
            {
                path = path.Replace("\\", "/");
                path = path.Replace("\\\\", "/");

                if (!session.Opened)
                {
                    if (OpenSession(session, ftpConfiguration))
                        return CreateDirectory(path, session, ftpConfiguration);

                    _logger.LogInformation($"Could not open ftp session");
                    return false;
                }

                if (!session.FileExists(path))
                {
                    var parentPath = new DirectoryInfo(path);
                    var parent = parentPath.Parent.FullName.Replace("\\", "/");
                    parent = parent.Replace("\\\\", "/");
                    if (!session.FileExists(parent))
                    {
                        if (!CreateDirectory(parent, session, ftpConfiguration))
                        {
                            _logger.LogError($"Create directory: Error creando el directorio{parent}");
                            return false;
                        }
                    }

                    session.CreateDirectory(path);
                    _logger.LogInformation($"Create directory: {path}");
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateDirectory error");
                _logger.LogError($"Error {ex.Message}");
                return false;
            }
        }
        public bool CleanDirectory(string path, Session session, FtpConfiguration ftpConfiguration, bool isDirectory, bool isFile)
        {
            try
            {


                if (!session.Opened)
                {
                    if (OpenSession(session, ftpConfiguration))
                        return CleanDirectory(path, session, ftpConfiguration, isDirectory, isFile);

                    _logger.LogInformation($"Could not open ftp session");
                    return false;
                }

                if (session.FileExists(path))
                {
                    var files = session.EnumerateRemoteFiles(path, string.Empty, WinSCP.EnumerationOptions.MatchDirectories).ToList();
                    if (isDirectory)
                    {
                        foreach (var file in files.Where(f => f.IsDirectory == isDirectory))
                        {
                            session.RemoveFiles(RemotePath.EscapeFileMask(file.FullName));
                        }
                    }
                    if (isFile)
                    {
                        foreach (var file in files.Where(f => f.IsDirectory != isFile))
                        {
                            session.RemoveFile(RemotePath.EscapeFileMask(file.FullName));
                        }
                    }
                    _logger.LogInformation($"Clean directory: {path}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateDirectory error");
                _logger.LogError($"Error {ex.Message}");
                return false;
            }
        }

        public bool DeleteFile(string filePath, Session session, FtpConfiguration ftpConfiguration)
        {
            try
            {
                if (!session.Opened)
                {
                    if (OpenSession(session, ftpConfiguration))
                        return DeleteFile(filePath, session, ftpConfiguration);

                    _logger.LogInformation($"Could not open ftp session");
                    return false;
                }

                if (session.FileExists(filePath))
                {

                    session.RemoveFiles(RemotePath.EscapeFileMask(filePath));
                    _logger.LogInformation($"File deleted: {filePath}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("DeleteFile error");
                _logger.LogError($"Error {ex.Message}");
                return false;
            }
        }

        public string GetDirectoryCampaign(string path, Session session, FtpConfiguration ftpConfiguration)
        {
            var nombreCampania = string.Empty;
            try
            {
                if (!session.Opened)
                {
                    if (OpenSession(session, ftpConfiguration))
                        return GetDirectoryCampaign(path, session, ftpConfiguration);

                    _logger.LogInformation($"Could not open ftp session");
                    return "";
                }

                if (session.FileExists(path))
                {
                    var files = session.EnumerateRemoteFiles(path, string.Empty, WinSCP.EnumerationOptions.MatchDirectories).ToList();
                    var remoteFileInfo = files.Where(f => f.IsDirectory).FirstOrDefault();
                    if (remoteFileInfo != null)
                    {
                        nombreCampania = remoteFileInfo.Name;
                    }
                    _logger.LogInformation($"Clean directory: {path}");
                }

                return nombreCampania;
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateDirectory error");
                _logger.LogError($"Error {ex.Message}");
                return nombreCampania;
            }
        }

    }
}
