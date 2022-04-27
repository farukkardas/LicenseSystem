using Core.Entities.Abstract;
using Core.Entities.Concrete;

namespace Entities.Concrete
{
    public  class Panel : IEntity
    {
        public int Id { get; set; }
        public int PanelOwnerId { get; set; }
        public int? PanelSellerId { get; set; }
        public bool? IsActive { get; set; }
        public double Balance { get; set; }
        public int CreatedLicense { get; set; }
        public int ApplicationId {get;set;}
    }
}