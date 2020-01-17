

namespace EstouAVer.Tables
{
    public class Dir
    {        
        public string path { get; }

        public string username { get; }

        public Dir(string path, string username)
        {
            this.path = path;
            this.username = username;
        }
    }
}
