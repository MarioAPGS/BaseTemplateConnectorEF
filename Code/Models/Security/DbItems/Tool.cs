using Core.Interfaces;
using Core.Models.Security.DbItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Core.Models.Security.DbItems
{
    public class Tool : IDbItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public string Name { get; set; }
        public IEnumerable<Table> Tables {get; set; }

        public Tool (string name)
        {
            Name = name;
        }
        
        public Tool(string name, IEnumerable<Table> tables) : this(name)
        {
            Tables = tables;
        }

        [JsonConstructor]
        public Tool(int id, DateTime created, DateTime? modified, DateTime? deleted, string name, IEnumerable<Table> tables)
        {
            Id = id;
            Created = created;
            Modified = modified;
            Deleted = deleted;
            Name = name;
            Tables = tables;
        }
    }
}
