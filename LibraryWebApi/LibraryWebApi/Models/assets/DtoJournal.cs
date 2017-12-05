using System.ComponentModel.DataAnnotations;

namespace LibraryWebApi.Models
{
    public class DtoJournal : DtoLibraryAsset
    {
        public string Frequency { get; set; }
    }
}
