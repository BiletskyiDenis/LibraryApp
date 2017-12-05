namespace LibraryWebApi.Extentions
{
    public class HttpPostedFile
    {
        public HttpPostedFile(string name, string filename, byte[] file)
        {
            Name = name;
            FileName = filename;
            File = file;
        }

        public string Name { get; private set; }
        public string FileName { get; private set; }
        public byte[] File { get; private set; }
    }
}