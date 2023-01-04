using Core.Models;
using Core.Models.Security.DbItems;
using Infrastructure.Repositories.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Logic.Managers
{
    public class SecurityManager
    {
        public IRepositorySecurity RepositorySecurity;

        public SecurityManager(IRepositorySecurity repositorySecurity)
        {
            RepositorySecurity = repositorySecurity;
        }
        public Response ValidateToken(HttpRequest request, string table, string method)
        {
            string token = null;
            if (request.Headers.ContainsKey("token"))
                token = request.Headers["token"].ToString();
            return RepositorySecurity.ValidateToken(token, table, method);
        }
        public Response<Log> LogInfo(HttpRequest request, string table, int datachange = 0, string description = null)
        {
            var tool = Functions.GetTool();
            var ipAdress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            return RepositorySecurity.LogInfo(Log.Info(tool, table, request.Method, ipAdress, datachange, description));
        }
        public Response<Log> LogWarn( HttpRequest request, string table, int datachange = 0, string description = null)
        {
            var tool = Functions.GetTool();
            var ipAdress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            return RepositorySecurity.LogWarn(Log.Warn(tool, table, request.Method, ipAdress, datachange, description));
        }
        public Response<Log> LogError( HttpRequest request, string table, string description = null)
        {
            var tool = Functions.GetTool();
            var ipAdress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            return RepositorySecurity.LogError(Log.Error(tool, table, request.Method, ipAdress, description));
        }
        public Response<AuthToken> LogIn(string email, string password)
        {
            return RepositorySecurity.LogIn(email, password);
        }
        public Response ValidateLogin(HttpRequest request, bool meesageOverdue = false)
        {
            string token = null;
            if (request.Headers.ContainsKey("token"))
                token = request.Headers["token"].ToString();
            return RepositorySecurity.ValidateLogin(token, meesageOverdue);
        }
        public Response<Tool> GetTools(HttpRequest request)
        {
            string token = null;
            if (request.Headers.ContainsKey("token"))
                token = request.Headers["token"].ToString();

            return RepositorySecurity.GetTools(token);
        }
        public Response<Tool> CreateTool(HttpRequest request)
        {
            string token = null;
            if (request.Headers.ContainsKey("token"))
                token = request.Headers["token"].ToString();

            return RepositorySecurity.CreateTool(token, AddOrExisistApplication());
        }
        public Response<Tool> DeleteToolByName(HttpRequest request, string name)
        {

            var resultGetTool = GetTools(request);
            if (resultGetTool.Success)
            {
                var tools = resultGetTool.Data.Where(x => x.Name == name);
                if (tools.Any())
                    return DeleteTool(request, tools.First().Id);
            }

            return resultGetTool;
        }
        public Response<Tool> DeleteTool(HttpRequest request, int id)
        {
            string token = null;
            if (request.Headers.ContainsKey("token"))
                token = request.Headers["token"].ToString();

            return RepositorySecurity.DeleteTool(token, id);
        }
        public string AddOrExisistApplication()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            IConfiguration Configuration = builder.Build();
            var toolName = Configuration.GetSection("Tool").Value;
            var tables = Configuration.GetSection("Tables").GetChildren().ToList();

            Tool tool = new(toolName);
            var tablesList = new List<Table>();
            foreach (var table in tables)
                tablesList.Add(new(table.Value));
            tool.Tables = tablesList;
            return JsonConvert.SerializeObject(tool, Functions.NoLoop());
        }
    }
}
