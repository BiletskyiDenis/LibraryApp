namespace LibraryWebApi.Extentions
{
    public class HttpPostedField
    {
        public HttpPostedField(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }
    }
}