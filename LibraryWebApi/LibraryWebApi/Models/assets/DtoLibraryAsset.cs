using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApi.Models
{
    public abstract class DtoLibraryAsset
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public decimal Price { get; set; }
        public virtual string Genre{ get; set; }
        public int NumbersOfCopies { get; set; }
    }
}