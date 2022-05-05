using System;
using Core.Entities.Abstract;

namespace Entities.Concrete
{
    public class KeyLicense : IEntity
    {
        public int Id { get; set; }
        public string? AuthKey { get; set; }
        public string? Hwid { get; set; }
        public int? OwnerId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsOwned { get; set; }
        public int ApplicationId {get;set;}
      
    }
}