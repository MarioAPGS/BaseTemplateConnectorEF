using Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Security.DbItems
{
    public class Log : IDbItem
    {
        public static string INFO = "INFO";
        public static string WARN = "WARN";
        public static string ERROR = "ERROR";

        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        [NotMapped]
        public DateTime? Deleted { get; set; }
        public string Tool { get; set; }
        public string Table { get; set; }
        public string Method { get; set; }
        public string Type { get; set; }
        public int DataChanges { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }

        public static Log Info(string tool, string table, string method, string ipAddress, int dataChanges = 0, string description = null)
        {
            Log log = new();
            log.Tool = tool;
            log.Table = table;
            log.Method = method;
            log.Type = INFO;
            log.DataChanges = dataChanges;
            log.Description = description;
            log.IpAddress = ipAddress;

            return log;
        }

        public static Log Warn(string tool, string table, string method, string ipAddress, int dataChanges = 0, string description = null)
        {
            Log log = new();
            log.Tool = tool;
            log.Table = table;
            log.Method = method;
            log.Type = WARN;
            log.DataChanges = dataChanges;
            log.Description = description;
            log.IpAddress = ipAddress;

            return log;
        }

        public static Log Error(string tool, string table, string method, string ipAddress, string description = null)
        {
            Log log = new();
            log.Tool = tool;
            log.Table = table;
            log.Method = method;
            log.Type = ERROR;
            log.DataChanges = 0;
            log.Description = description;
            log.IpAddress = ipAddress;

            return log;
        }

    }
}
