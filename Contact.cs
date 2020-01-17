using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArtShop.Models
{
    public class Contact
    /*Folosim atribute sa validam date.*/
    {
        //Required - spune sistemului ca toate proprietatile de mai jos sunt obligatoriii cand userul cere date inapoi
        [Required]
        [MinLength(5)]
        public string Name { get; set; }
        [Required]
        [EmailAddress] //vf daca adr email primita e valida
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        [MaxLength(250, ErrorMessage = "Mesajul nu trebuie sa depaseasca lungimea de 250.")]
        public string Message { get; set; }
    }
}
