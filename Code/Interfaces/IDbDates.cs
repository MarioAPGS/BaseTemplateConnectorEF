using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Interfaces
{
    public interface IDbDates
    {
        DateTime Created { get; set; }
        DateTime? Modified {get; set; }
        DateTime? Deleted { get; set; }
        string UserId { get; set; }
    }
}
