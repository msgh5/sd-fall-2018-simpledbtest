using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbPerformanceTest.Domain
{
    [Table("Production.Product")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string ProductNumber { get; set; }

        public bool MakeFlag { get; set; }

        public DateTime SellStartDate { get; set; }

        public virtual List<WorkOrder> WorkOrders { get; set; }

        public Product()
        {
            WorkOrders = new List<WorkOrder>();
        }
    }
}
