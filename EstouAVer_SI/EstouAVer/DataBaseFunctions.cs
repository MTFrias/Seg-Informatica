using System;
using EstouAVer.Tables;

namespace EstouAVer
{
    public static class DataBaseFunctions
    {
        public static string userLog;

        public static bool Register(User user)
        {
            if (AjudanteParaBD.SelectUserWithUsername(user.username) != null)
            {
                Console.WriteLine("\nErro! O username \"" + user.username + "\" não está disponivel!");
                Console.WriteLine("Por-favor introduza outro username!");

                return false;
            }

            if (AjudanteParaBD.InsertUser(user) != -1)
            {
                Console.WriteLine("Registo efetuado com sucesso!");
                return true;
            }

            Console.WriteLine("Registo efetuado sem sucesso!");
            return false;
        }

        public static bool Login(string username, string password)
        {
            string op = string.Empty;

            User user = AjudanteParaBD.SelectUserWithUsername(username);

            Console.Clear();
            if (user == null)
            {
                Console.WriteLine("\nErro! O utilizador inserido nao existe.");
          
                return false;
            }

            //calcula o rep da palavra passe introduzida
            string insertedRep = SHA256Code.GenerateFromText(SHA256Code.GenerateFromText(password) + user.salt);

            if (user.rep.Equals(insertedRep))
            {
                Console.WriteLine("\nLogin efetuado com sucesso!\n");
                userLog = username;
                return true;
            }
            else
            {
                Console.WriteLine("\nDados invalidos! Tente novamente.\n");
                return false;
            }
        }

        public static void VerificarIntegridade(Dir dir)
        {
            var currentfiles = SHA256Code.GenerateFromDir(dir.path);
            var databaseFiles = AjudanteParaBD.SelectFilesWithDir(dir.path);


            foreach (TFile f in databaseFiles)
            {
                //Se foi eliminado
                if (!currentfiles.Keys.Contains(f.path))
                {
                    Console.WriteLine("Removido o ficheiro \'" + f.path + "\' da base de dados.");
                    AjudanteParaBD.DeleteFile(f);

                    continue;
                }

                // Se existir vamos ver se foi alterado
                if (!currentfiles[f.path].Equals(f.sha256))
                {
                    Console.WriteLine("O ficheiro \'" + f.path + "\'  sofreu alteracoes.");
                    AjudanteParaBD.UpdateFile(new TFile(f.path, currentfiles[f.path], dir.path));
                }
                else
                {
                    Console.WriteLine("O ficheiro \'" + f.path + "\' não sofreu alteracoes.");
                }

                currentfiles.Remove(f.path);
            }

            // Se ainda houver ficheiros, são os que foram adicionados
            foreach (string file in currentfiles.Keys)
            {
                Console.WriteLine("Adicionado o ficheiro \'" + file + "\' a base de dados.");
                AjudanteParaBD.InsertFile(new TFile(file, currentfiles[file], dir.path));
            }
        }

        public static void VerificarIntegridadeHMAC(Dir dir, string key)
        {
            var currentfiles = HMac.hmac(dir.path, key);
            var databaseFiles = AjudanteParaBD.SelectFileHMACWithDir(dir.path);


            foreach (var f in databaseFiles)
            {
                //Se foi eliminado
                if (!currentfiles.Keys.Contains(f.path))
                {
                    Console.WriteLine("Removido o ficheiro \'" + f.path + "\' da base de dados.");
                    AjudanteParaBD.DeleteFileHMAC(f);

                    continue;
                }

                // Se existir vamos ver se foi alterado
                if (!currentfiles[f.path].Equals(f.hmac))
                {
                    Console.WriteLine("O ficheiro \'" + f.path + "\'  sofreu alteracoes.");
                    AjudanteParaBD.UpdateFileHMAC(new FileHmac(f.path, currentfiles[f.path], dir.path));
                }
                else
                {
                    Console.WriteLine("O ficheiro \'" + f.path + "\' não sofreu alteracoes.");
                }

                currentfiles.Remove(f.path);
            }

            // Se ainda houver ficheiros, são os que foram adicionados
            foreach (string file in currentfiles.Keys)
            {
                Console.WriteLine("Adicionado o ficheiro \'" + file + "\' a base de dados.");
                AjudanteParaBD.InsertFileHMAC(new FileHmac(file, currentfiles[file], dir.path));
            }
        }
    }
}