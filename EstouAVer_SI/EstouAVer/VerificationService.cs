using EstouAVer.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace EstouAVer
{
    partial class VerificationService : ServiceBase
    {
        private readonly Timer timer;
        private string mode;
        private string key;
        private string username;
        private string password;
        private string dataBasePassword;
        private Dir dir;

        public VerificationService()
        {
            InitializeComponent();

            timer = new Timer(1000) { AutoReset = true };
            timer.Elapsed += VerifyDirectory;
        }

        private void VerifyDirectory(object sender, ElapsedEventArgs e)
        {
            // O ficheiro ------------- foi alterado em ------------
            string[] lines = null;
            if (mode.Equals("sha256"))
                lines = VerificarIntegridadeSHA256();
            else if (mode.Equals("hmac"))
                lines = VerificarIntegridadeHMAC();

            File.AppendAllLines(Directories.servicePath, lines);
        }

        public string[] VerificarIntegridadeSHA256()
        {
            var currentfiles = SHA256Code.GenerateFromDir(dir.path);
            var databaseFiles = AjudanteParaBD.SelectFilesWithDir(dir.path);

            var lines = new List<string>();

            foreach (TFile f in databaseFiles)
            {
                //Se foi eliminado
                if (!currentfiles.Keys.Contains(f.path))
                {
                    lines.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - Removido o ficheiro \'" + f.path + "\' da base de dados.");
                    AjudanteParaBD.DeleteFile(f);

                    continue;
                }

                // Se existir vamos ver se foi alterado
                if (!currentfiles[f.path].Equals(f.sha256))
                {
                    lines.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+ " - O ficheiro \'" + f.path + "\' sofreu alteracoes.");
                    AjudanteParaBD.UpdateFile(new TFile(f.path, currentfiles[f.path], dir.path));
                }

                currentfiles.Remove(f.path);
            }

            // Se ainda houver ficheiros, são os que foram adicionados
            foreach (string file in currentfiles.Keys)
            {
                lines.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - Adicionado o ficheiro \'" + file + "\' a base de dados.");
                AjudanteParaBD.InsertFile(new TFile(file, currentfiles[file], dir.path));
            }

            return lines.ToArray();
        }
        
        public string[] VerificarIntegridadeHMAC()
        {
            var currentfiles = HMac.hmac(dir.path, key);
            var databaseFiles = AjudanteParaBD.SelectFileHMACWithDir(dir.path);

            var lines = new List<string>();

            foreach (var f in databaseFiles)
            {
                //Se foi eliminado
                if (!currentfiles.Keys.Contains(f.path))
                {
                    lines.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - Removido o ficheiro \'" + f.path + "\' da base de dados.");
                    AjudanteParaBD.DeleteFileHMAC(f);

                    continue;
                }

                // Se existir vamos ver se foi alterado
                if (!currentfiles[f.path].Equals(f.hmac))
                {
                    lines.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - O ficheiro \'" + f.path + "\' sofreu alteracoes.");
                    AjudanteParaBD.UpdateFileHMAC(new FileHmac(f.path, currentfiles[f.path], dir.path));
                }

                currentfiles.Remove(f.path);
            }

            // Se ainda houver ficheiros, são os que foram adicionados
            foreach (string file in currentfiles.Keys)
            {
                lines.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " - Adicionado o ficheiro \'" + file + "\' a base de dados.");
                AjudanteParaBD.InsertFileHMAC(new FileHmac(file, currentfiles[file], dir.path));
            }

            return lines.ToArray();
        }

        private bool Login(string username, string password)
        {
            User user = AjudanteParaBD.SelectUserWithUsername(username);
            string insertedRep = SHA256Code.GenerateFromText(SHA256Code.GenerateFromText(password) + user.salt);

            return user.rep.Equals(insertedRep);
        }

        public void EncryptFileBD()
        {
            byte[] bytesToBeEncrypted = File.ReadAllBytes(Directories.database);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(dataBasePassword);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES.AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            File.WriteAllBytes(Directories.databaseAES, bytesEncrypted);

            FileInfo f = new FileInfo(Directories.database);

            try
            {
                f.Delete();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void DecryptFileBD()
        {
            byte[] bytesToBeDecrypted = File.ReadAllBytes(Directories.databaseAES);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(dataBasePassword);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES.AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            File.WriteAllBytes(Directories.database, bytesDecrypted);
        }

        protected override void OnStart(string[] args)
        {
            mode = string.Empty;
            key = string.Empty;
            string directory = string.Empty;
            username = string.Empty;
            password = string.Empty;
            dataBasePassword = string.Empty;

            if (args.Length != 5 && args.Length != 6)
            {
                OnStop();
                return;
            }

            mode = args[0].ToLower();

            if (mode.Equals("sha256"))
            {
                username  = args[1];
                password  = args[2];
                directory = args[3];
                dataBasePassword = args[4];
            }
            else if(mode.Equals("hmac"))
            {
                key       = args[1];
                username  = args[2];
                password  = args[3];
                directory = args[4];
                dataBasePassword = args[5];
            }
            else
            {
                OnStop();
                return;
            }
            
            if(!File.Exists(Directories.databaseAES))
            {
                OnStop();
                return;
            }

            DecryptFileBD();

            if (!Login(username, password))
            {
                OnStop();
                return;
            }
                
            var dirs = AjudanteParaBD.SelectDirsWithUsername(username);

            foreach(Dir d in dirs)
                if(d.path.Equals(directory))
                    dir = d;
            
            if(dir == null)
            {
                OnStop();
                return;
            }
            
            timer.Start();
        }

        protected override void OnStop()
        {
            if(File.Exists(Directories.databaseAES))
                EncryptFileBD();

            timer.Stop();
        }
    }
}
