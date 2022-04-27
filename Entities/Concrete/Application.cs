using System;
using Core.Entities.Abstract;

namespace Entities.Concrete
{
    public class Application : IEntity
    {
        public int Id { get; set; }
        public string? ApplicationName { get; set; }
        public int? OwnerId { get; set; }
        public DateTime? CreationTime { get; set; }
        public bool? Status { get; set; }
    }
}