using Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Core.Models.Security.DbItems
{
    public class Table : IDbItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Deleted { get; set; }

        public string Name { get; set; }
        public int ToolId { get; set; }
        public Tool Tool { get; set; }
        public IEnumerable<Grant> Grants { get; set; }

        [JsonConstructor]
        public Table(string name)
        {
            Name = name;
        }

        public Table(string name, int toolId) : this(name)
        {
            ToolId = toolId;
        }

        public Table(string name, int toolId, string userId) : this(name, toolId)
        {
            UserId = userId;
        }
    }
}
