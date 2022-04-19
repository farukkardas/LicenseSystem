using System;
using Core.Entities.Abstract;

namespace Entities.Concrete
{
    public class Log : IEntity
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public int? OwnerId { get; set; }
        public DateTime? Date { get; set; }
        public bool Success { get; set; }
    }
}