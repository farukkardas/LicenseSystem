using Core.Entities.Abstract;

namespace Entities.Dto
{
    public  class UserRegisterDto : IDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}