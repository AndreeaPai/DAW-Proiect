using System.ComponentModel.DataAnnotations;

namespace ArtShop.Models
{
    public class OrderItemModel
    {
        public int Id { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public int ProductId { get; set; }  //Cand vom insera items vom avea grija ca product id sa fie specificat sa nea dea celalalte prop

        // prefixam aceste prop cu sufic entitatii din care vin: Product
        public string ProductCategory { get; set; }
        public string ProductSize { get; set; }
        public string ProductTitle { get; set; }
        public string ProductArtist { get; set; }
        public string ProductArtId { get; set; }

    }
}