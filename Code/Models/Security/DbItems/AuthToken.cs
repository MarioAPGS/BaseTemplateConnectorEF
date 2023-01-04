using Core.Interfaces;
using Newtonsoft.Json;
using System;

namespace Core.Models.Security.DbItems
{
    public class AuthToken : IDbItem
    {
        public int Id {get; set; }
        public string UserId {get; set; }
        public DateTime Created {get; set; }
        public DateTime? Modified {get; set; }
        public DateTime? Deleted {get; set; }
        public string Value {get; set; }
        public double OverdueTime { get; set; }
        public User User { get; set; }

        public AuthToken(string userId)
        {
            UserId = userId;
            Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            OverdueTime = 1440;
            Deleted = DateTimeOffset.UtcNow.AddMinutes(OverdueTime).DateTime;
        }

        public AuthToken(string userId, double overdueTime) : this(userId)
        {
            Deleted = DateTimeOffset.UtcNow.AddMinutes(OverdueTime).DateTime;
            OverdueTime = overdueTime;
        }

        public AuthToken(string userId, double overdueTime, string value) : this(userId, overdueTime)
        {
            Value = value;
        }

        [JsonConstructor]
        public AuthToken(int id, string userId, DateTime created, DateTime? modified, DateTime? deleted, string value, double overdueTime, User user)
        {
            Id = id;
            UserId = userId;
            Created = created;
            Modified = modified;
            Deleted = deleted;
            Value = value;
            OverdueTime = overdueTime;
            User = user;
        }
    }
}
