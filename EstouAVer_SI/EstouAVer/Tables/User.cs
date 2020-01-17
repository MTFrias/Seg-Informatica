using System;
using System.Collections.Generic;
using System.Text;

namespace EstouAVer.Tables
{
    public class User
    {
        public string username { get; }
        public string rep { get; }
        public string salt { get;  }

        public User(string username, string rep, string salt)
        {
            this.username = username;
            this.rep = rep;
            this.salt = salt;
        }
    }
}
