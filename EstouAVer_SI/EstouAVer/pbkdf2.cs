using System;
using System.Linq;
using System.Security.Cryptography;

namespace EstouAVer
{
    class pbkdf2
    {
        private const int saltByteSize     = 64;
        private const int hashByteSize     = 64;
        private const int pbkdf2Iterations = 10000;

        // cria um hash pbkdf2 da password com salt
        public static passwordHashContainer CreateHash(string pass)
        {

            using (var csprng = new RNGCryptoServiceProvider())
            {

                //cria um salt único para cada pass
                var salt = new byte[saltByteSize];
                csprng.GetBytes(salt);

                //criar hash da pass e codificar os parâmetros
                var hash = Pbkdf2(pass, salt, pbkdf2Iterations, hashByteSize);

                return new passwordHashContainer(hash, salt);
            }
        }

        //cria o hash baseado na password recebida e o salt guardado
        public static byte[] CreateHash(string pass, byte[] salt)
        {

            //extrai os parâmetros do hash
            return Pbkdf2(pass, salt, pbkdf2Iterations, hashByteSize);
        }

        //valida a pass se o hash estiver correto
        //public static bool validatePass(string pass, byte[] salt, byte[] correctHash)
        //{

        //    //extrai os parâmetros do hash
        //    byte[] testHash = Pbkdf2(pass, salt, pbkdf2Iterations, hashByteSize);
        //    return compareHashes(correctHash, testHash);
        //}

        public static string validatePass(string pass, byte[] salt, byte[] correctHash)
        {
            bool value;
            //extrai os parâmetros do hash
            byte[] testHash = Pbkdf2(pass, salt, pbkdf2Iterations, hashByteSize);
           value =  compareHashes(correctHash, testHash);

            if(value == true)
            {

                return BitConverter.ToString(correctHash).Replace("-","");
            }
            else
            {

                return "erro";
            }
        
                
        }


        //compara dois hashes
        public static bool compareHashes(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            return !array1.Where((t, i) => t != array2[i]).Any();
        }

        //cria o hash PBKDF2-SHA256 de uma password
        private static byte[] Pbkdf2(string pass, byte[] salt, int pbkdf2Iterations, int outBytes)
        {

            using (var pbkdf2 = new Rfc2898DeriveBytes(pass, salt))
            {
                pbkdf2.IterationCount = pbkdf2Iterations;
          
                return pbkdf2.GetBytes(outBytes);
            }
        }

        //contentor para o hash, salt e iterações da pass
        public sealed class passwordHashContainer
        {

            public passwordHashContainer(byte[] hashedPassword, byte[] salt)
            {
                this.hashedPassword = hashedPassword;
                this.salt = salt;
            }

            //vai buscar a pass
            public byte[] hashedPassword
            {
                get; private set;
            }

            //vai buscar o salt
            public byte[] salt
            {
                get; private set;
            }
        }

        //métodos para converter entre strings hexadecimais e byte arrays
        public static class byteConverter
        {
            //converte hex string para byte array
            public static byte[] getHexBytes(string hexString)
            {

                var bytes = new byte[hexString.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {

                    var strPos = i * 2;
                    var chars = hexString.Substring(strPos, 2);
                    bytes[i] = Convert.ToByte(chars, 16);
                }

                return bytes;
            }

            //converte de byte array para hex string
            public static string getHexString(byte[] bytes)
            {
                return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
            }

        }
    }

}




/* 
 * Password Hashing With PBKDF2 (http://crackstation.net/hashing-security.htm).
 * Copyright (c) 2013, Taylor Hornby
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 */
