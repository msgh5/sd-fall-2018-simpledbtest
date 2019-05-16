using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbPerformanceTest.ViewModel
{
    public class WorkOrderViewModel
    {
        public int WorkOrderId { get; set; }
        public int ProductId { get; set; }
        public int OrderQty { get; set; }
        public DateTime StartDate { get; set; }

        public ProductViewModel Product { get; set; }
    }
}
