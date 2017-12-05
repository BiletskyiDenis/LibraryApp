namespace LibraryWebApi.Models
{
    public class DtoDetailModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }

        public string Type { get; set; }
        public string Genre{ get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }

        public int Year { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public int Pages { get; set; }

        public decimal Price { get; set; }

        public string Frequency { get; set; }
        public int NumberOfCopies { get; set; }
    }
}