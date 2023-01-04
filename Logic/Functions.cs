using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Logic
{
    public class Functions
    {
        private static readonly log4net.ILog Logging = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string GetTool()
        {
            try
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json");
                IConfiguration Configuration = builder.Build();
                var LogAndAuth = Configuration.GetSection("LogAndAuth");
                return Configuration.GetSection("Tool").Value;
            }
            catch (Exception ex)
            {
                Logging.Error("GetTool error");
                Logging.Error(ex);

                return null;
            }
        }

        public static string RequestToString(HttpRequest request)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[" + request.Method + "] ");

            if (request.IsHttps)
                sb.Append("https://");
            else
                sb.Append("http://");

            sb.Append(request.Host + request.Path);

            return sb.ToString();
        }

        public static string FixUrl(string url)
        {
            url = url.Replace('/', '\\');
            url = url.Replace("//", "\\");
            url = url.Replace("\\\\", "\\");
            if (!url.EndsWith('\\'))
                url += '\\';
            return url;
        }

        public static JsonSerializerSettings NoLoop()
        {
            return new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        }
    }
}
