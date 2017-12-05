using System.ComponentModel.DataAnnotations;

namespace Library.Data.Models
{
    public class Journal : LibraryAsset
    {
        [Required]
        public string Frequency { get; set; }
    }
}
