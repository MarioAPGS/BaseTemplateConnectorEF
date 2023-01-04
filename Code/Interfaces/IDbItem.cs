using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Interfaces
{
    public interface IDbItem : IDbDates
    {
        [Key]
        int Id { get; set; }
    }
}
