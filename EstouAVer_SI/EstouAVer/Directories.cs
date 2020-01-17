using System;

namespace EstouAVer
{
    class Directories
    {
        public static readonly string database = AppDomain.CurrentDomain.BaseDirectory    + "\\EstouAVer.sqlite";
        public static readonly string databaseAES = AppDomain.CurrentDomain.BaseDirectory + "\\EstouAVer.aes";
        public static readonly string servicePath = AppDomain.CurrentDomain.BaseDirectory + "\\EstouAVerlog.txt";
    }
}
