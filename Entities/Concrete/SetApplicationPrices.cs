using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class SetApplicationPrices
    {
        public double DailyPrice { get; set; }
        public double WeeklyPrice { get; set; }
        public double MonthlyPrice { get; set; }
        public int ApplicationId { get; set; }
    }
}
