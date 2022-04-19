using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class UserDetailsDto : IDto
    {
        public int UserId { get; set; }
        public int LicensesCount { get; set; }
        public int PanelsCount { get; set; }
        public int UsedKeys { get; set; }
        public int UnusedKeys { get; set; }
    }
}
