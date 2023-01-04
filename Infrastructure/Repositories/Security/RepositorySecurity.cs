using Core.Models;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.IO;
using System;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Core.Models.Security.DbItems;

namespace Infrastructure.Repositories.Security
{
    public class RepositorySecurity : IRepositorySecurity
    {
        private static readonly log4net.ILog Logging = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly string email = "adsumo@adsumo.com";
        public readonly string password = "Secreto@123";
        private DateTime TimeToOverdueToken;
        private string Token, Tool, Endpoint, EpValidateToken, EpInfo, EpWarn, EpError, EpTool, EpLogin;
        private bool IsAuthAvailable, IsLogsAvailable;

        public void LoadSettings()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            IConfiguration Configuration = builder.Build();
            var Security = Configuration.GetSection("Security");
            Tool = Configuration.GetSection("Tool").Value;
            Endpoint = Security.GetSection("Endpoint").Value;
            EpValidateToken = Security.GetSection("Token").Value;
            EpInfo = Security.GetSection("Info").Value;
            EpWarn = Security.GetSection("Warn").Value;
            EpError = Security.GetSection("Error").Value;
            EpTool = Security.GetSection("EpTool").Value;
            EpLogin = Security.GetSection("EpLogin").Value;
            IsAuthAvailable = Convert.ToBoolean(Configuration.GetSection("AdsumoModule").Value);
            IsLogsAvailable = Convert.ToBoolean(Configuration.GetSection("Logs").Value);
            if(IsAuthAvailable)
                GetToken();
        }

        public Response<Log> LogInfo(Log log)
        {
            try {
                if (Tool == null || DateTime.Now >= TimeToOverdueToken)
                    LoadSettings();
                if (IsLogsAvailable)
                {    
                    var client = new RestClient();
                    var request = new RestRequest(new Uri(Endpoint + "/" + EpInfo), Method.Post)
                    {
                        RequestFormat = DataFormat.Json
                    };
                    request.AddJsonBody(log);

                    var response = client.Execute<Response<Log>>(request);
                    if (!string.IsNullOrEmpty(response.Content))
                        return response.Data;
                    else
                        throw new Exception("Error reading reponse -> " + response.Content);
                }
                else
                    return new Response<Log>(true, new List<Log>() { log }, "Logs are disabled", HttpStatusCode.Continue);
            }
            catch (Exception ex)
            {
                Logging.Info("LogWarn error");
                Logging.Error(ex.Message);

                return new Response<Log>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response<Log> LogWarn(Log log)
        {

            try {
                

                if (Tool == null || DateTime.Now >= TimeToOverdueToken)
                    LoadSettings();
                if (IsLogsAvailable)
                {
                    var client = new RestClient();
                    var request = new RestRequest(new Uri(Endpoint + "/" + EpWarn), Method.Post)
                    {
                        RequestFormat = DataFormat.Json
                    };
                    request.AddJsonBody(log);

                    var response = client.Execute<Response<Log>>(request);
                    if (!string.IsNullOrEmpty(response.Content))
                        return response.Data;
                    else
                        throw new Exception("Error reading reponse -> " + response.Content);
                }
                else
                    return new Response<Log>(true, new List<Log>() { log }, "Logs are disabled", HttpStatusCode.Continue);
            }
            catch (Exception ex)
            {
                Logging.Info("LogWarn error");
                Logging.Error(ex.Message);

                return new Response<Log>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response<Log> LogError(Log log)
        {
            try
            {
                if (Tool == null || DateTime.Now >= TimeToOverdueToken)
                    LoadSettings();
                if (IsLogsAvailable)
                {
                    var client = new RestClient();
                    var request = new RestRequest(new Uri(Endpoint + "/" + EpError), Method.Post)
                    {
                        RequestFormat = DataFormat.Json
                    };
                    request.AddJsonBody(log);

                    var response = client.Execute<Response<Log>>(request);
                    if (!string.IsNullOrEmpty(response.Content))
                        return response.Data;
                    else
                        throw new Exception("Error reading reponse -> " + response.Content);
                }
                else
                    return new Response<Log>(true, new List<Log>() { log }, "Logs are disabled", HttpStatusCode.Continue);
            }
            catch (Exception ex)
            {
                Logging.Info("LogError error");
                Logging.Error(ex.Message);

                return new Response<Log>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response ValidateToken(string token, string table, string method)
        {
            try
            {
                if (Tool == null)
                    LoadSettings();

                if (IsAuthAvailable)
                {
                    var client = new RestClient();
                    var request = new RestRequest(Endpoint + "/" + EpValidateToken, Method.Post);

                    request.AddHeader("token", token);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("tool", Tool);
                    request.AddHeader("table", CheckTable(table));
                    request.AddHeader("method", method);

                    var response = client.Execute<Response>(request);
                    if (!string.IsNullOrEmpty(response.Content))
                        return response.Data;
                    else
                        throw new Exception("Error reading reponse -> " + response.Content);
                }
                else
                    return new Response(true, "OK", HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                Logging.Info("LogInfo error");
                Logging.Error(ex.Message);

                return new Response(false, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response<AuthToken> LogIn(string email, string password)
        {
            try
            {
                if (Tool == null)
                    LoadSettings();

                var client = new RestClient();
                var request = new RestRequest(Endpoint + "/" + EpLogin, Method.Get);

                request.AddParameter("email", email);
                request.AddParameter("password", password);
                request.AddHeader("token", "-");
                //request.AddJsonBody(new Log() { Tool = EpTool, Table = table });

                var response = client.Execute(request);
                if (!string.IsNullOrEmpty(response.Content))
                {
                    return JsonConvert.DeserializeObject<Response<AuthToken>>(response.Content);
                }
                else
                    throw new Exception("Error reading reponse -> " + response.Content);

            }
            catch (Exception ex)
            {
                Logging.Info("LogIn error");
                Logging.Error(ex.Message);

                return new Response<AuthToken>(false,0 , ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response ValidateLogin(string token, bool meesageOverdue)
        {
            try
            {
                if (Tool == null)
                    LoadSettings();

                var client = new RestClient();
                var url = Endpoint + "/" + EpValidateToken;
                if (meesageOverdue)
                    url += "/overdue";
                var request = new RestRequest(url, Method.Post);

                request.AddHeader("token", token);
                //request.AddJsonBody(new Log() { Tool = EpTool, Table = table });

                var response = client.Execute(request);
                if (!string.IsNullOrEmpty(response.Content))
                {
                    return JsonConvert.DeserializeObject<Response>(response.Content);
                }
                else
                    throw new Exception("Error reading reponse -> " + response.Content);

            }
            catch (Exception ex)
            {
                Logging.Info("ValidateLogin error");
                Logging.Error(ex.Message);

                return new Response(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response<Tool> CreateTool(string token, string toolJson)
        {
            try
            {
                if (Tool == null)
                    LoadSettings();

                var client = new RestClient();
                var request = new RestRequest(Endpoint + "/" + EpTool, Method.Post);

                request.AddHeader("token", token);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("toolJson", toolJson, ParameterType.QueryString);
                var response = client.Execute(request);
                var result = JsonConvert.DeserializeObject<Response<Tool>>(response.Content);
                if (result != null)
                    return result;
                else
                    return new Response<Tool>(false, 0, response.Content, HttpStatusCode.InternalServerError);

            }
            catch (Exception ex)
            {
                Logging.Info("CreateTool error");
                Logging.Error(ex.Message);

                return new Response<Tool>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response<Tool> GetTools(string token)
        {
            try
            {
                if (Tool == null)
                    LoadSettings();
                var client = new RestClient();
                var request = new RestRequest(Endpoint + "/" + EpTool, Method.Get);

                request.AddHeader("token", token);
                request.RequestFormat = DataFormat.Json;

                var response = client.Execute(request);
                var result = JsonConvert.DeserializeObject<Response<Tool>>(response.Content);
                if (result != null)
                    return result;
                else
                    throw new Exception("Error reading reponse -> " + response.Content);

            }
            catch (Exception ex)
            {
                Logging.Info("CreateTool error");
                Logging.Error(ex.Message);

                return new Response<Tool>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response<Tool> DeleteTool(string token, int id)
        {
            try
            {
                var client = new RestClient();
                var request = new RestRequest(Endpoint + "/" + EpTool+"?id="+ id, Method.Delete);

                request.AddHeader("token", token);
                request.RequestFormat = DataFormat.Json;

                var response = client.Execute(request);
                var result = JsonConvert.DeserializeObject<Response<Tool>>(response.Content);
                if (result != null)
                    return result;
                else
                    throw new Exception("Error reading reponse -> " + response.Content);

            }
            catch (Exception ex)
            {
                Logging.Info("CreateTool error");
                Logging.Error(ex.Message);

                return new Response<Tool>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        #region Internal Funtions
        private string GetToken()
        {
            try
            {
                var loginResponse = LogIn(email, password);
                if (loginResponse.Success)
                {
                    Token = loginResponse.Data.FirstOrDefault().Value;
                    TimeToOverdueToken = TimeToOverdueToken = DateTime.Now.AddMinutes(loginResponse.Data.FirstOrDefault().OverdueTime - 10);
                }
            }
            catch (Exception ex)
            {
                Logging.Info("LogError error");
                Logging.Error(ex.Message);
            }
            // If token was sent but is user is not loged, Security return forbbiden code
            

            return Token;
        }
        private string CheckTable(string table)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json");
                IConfiguration Configuration = builder.Build();
                var tables = Configuration.GetSection("Tables").GetChildren().Where(x => x.Value.ToUpper().Trim() == table.ToUpper().Trim());
                if (tables.Any())
                    return tables.First().Value;
                else
                    throw new Exception("Table must be included in the table's section of appsettings.json");
            }
            catch (Exception ex)
            {
                Logging.Error("CheckTable error");
                Logging.Error(ex);
                throw new Exception("Unable verify table '" + table + "'");
            }
        }

        #endregion
    }
}
