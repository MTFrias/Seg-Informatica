namespace EstouAVer.Tables
{
    public class TFile
    {   
        public string path { get; }
        public string sha256 { get; }
        public string dir { get; }

        public TFile(string path, string sha256, string dir)
        {
            this.path = path;
            this.sha256 = sha256;
            this.dir = dir;
        }
    }
}
