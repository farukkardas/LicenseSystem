using Core.Entities.Abstract;

namespace Entities.Dto
{
    public class PanelRegisterDto : IDto
    {
        public string PanelMail { get; set; }
        public string PanelPassword { get; set; }
        public double Balance { get; set; }
        public int ApplicationId {get;set;}
    }
}