using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EstouAVer
{
    public class SHA256Code
    {
        public static IDictionary<string, string> GenerateFromDir(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Console.WriteLine("The directory specified could not be found.");
                return null;
            }

            var dir = new DirectoryInfo(directory);
            var files = dir.GetFiles();
            IDictionary<string, string> hashValues = new Dictionary<string, string>();

            using (var mySHA256 = SHA256.Create())
            {
                foreach (var fInfo in files)
                {
                    try
                    {
                        var fileStream = fInfo.Open(FileMode.Open);
                        var hash = mySHA256.ComputeHash(fileStream);

                        hashValues.Add(fInfo.FullName, BitConverter.ToString(hash).Replace("-", ""));
                        fileStream.Close();
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine($"I/O Exception: {e.Message}");
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine($"Access Exception: {e.Message}");
                    }
                }
            }
            return hashValues;
        }

        public static string GenerateFromText(string text)
        {
            using (var sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

                var builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));

                return builder.ToString();
            }
        }

        public static byte[] GenerateSalt()
        {
            Random number = new Random();
            byte[] salt = new byte[32];

            number.NextBytes(salt);
            return salt;
        }
    }
}