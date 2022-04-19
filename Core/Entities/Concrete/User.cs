using System;
using Core.Entities.Abstract;

namespace Core.Entities.Concrete
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public byte[]? PasswordHash { get; set; }
        public bool Status { get; set; }
        public double Balance { get; set; }
        public string? SecurityKey { get; set; }
        public DateTime SecurityKeyExpiration { get; set; }
    }
}