using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArtShop.Models
{
    public class OrderModel
    {
        //in principiu sa aiba acelasi prop ca Order,dar voi folosi la unele nume diferite
        public int OrderId { get; set; } //in loc de id
        public DateTime OrderDate { get; set; }
        [Required]//cer consumatorului sa introduca orderNumber
        [MinLength(4)]
        public string OrderNumber { get; set; }

        public ICollection<OrderItemModel> Items { get; set; }
    }
}
