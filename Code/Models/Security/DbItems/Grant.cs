using Core.Interfaces;
using System;

namespace Core.Models.Security.DbItems
{
    public class Grant : IDbItem
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int TableId { get; set; }
        public Table Table { get; set; }
        public string Method { get; set; }
        public int Privilege { get; set; }

        public Grant(string userId, int tableId)
        {
            UserId = userId;
            TableId = tableId;
            Method = "";
            Privilege = 1;
        }

        public Grant(string userId, int tableId, string method) : this(userId, tableId)
        {
            Method = method;
            Privilege = 1;
        }

        public Grant(string userId, int tableId, string method, int privilege) : this(userId, tableId, method)
        {
            Privilege = privilege;
        }
    }
}
