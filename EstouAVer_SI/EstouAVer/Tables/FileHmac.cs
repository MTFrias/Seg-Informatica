namespace EstouAVer.Tables
{
    public class FileHmac
    {

        public string path { get; set; }
        public string hmac { get; set; }
        public string dir { get; set; }

        public FileHmac(string path, string hmac, string dir)
        {
            this.path = path;
            this.hmac = hmac;
            this.dir = dir;
        }
    }
}
