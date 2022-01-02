using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI2.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }  //klucz główny 
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool HasDelivery { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }
        public int? CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }
        public int AddressId { get; set; }  //klucz obcy do tabeli z adresem
        public virtual Address Address { get; set; } //ułatwia posługiwaniem się obiektem typu restaurant kiedy pobieramy go z bazy danych
        public virtual List<Dish> Dishes { get; set; }
    }
}
