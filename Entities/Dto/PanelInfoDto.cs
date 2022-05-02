using Core.Entities.Abstract;
namespace Entities.Dto
{
    public class PanelInfoDto : IDto
    {
        public int Id { get; set; }
       public int PanelOwnerId { get; set; }
        public int? PanelSellerId { get; set; }
        public bool? IsActive { get; set; }
        public double Balance { get; set; }
        public int CreatedLicense { get; set; }
        public int ApplicationId { get; set; }
        public string? PanelMail { get; set; }
    }
}
