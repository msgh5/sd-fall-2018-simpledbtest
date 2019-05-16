using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbPerformanceTest.ViewModel
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string ProductNumber { get; set; }

        public bool MakeFlag { get; set; }

        public DateTime SellStartDate { get; set; }
    }
}
