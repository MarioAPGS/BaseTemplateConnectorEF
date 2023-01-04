using Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Core.Models.Security.DbItems
{
    public class User
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public IEnumerable<Grant> Grants { get; set; }
        public AuthToken AuthToken { get; set; }
    }
}
