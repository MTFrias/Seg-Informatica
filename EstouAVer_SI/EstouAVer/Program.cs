using EstouAVer.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;

namespace EstouAVer
{
    class Program
    {
        private static void Main(string[] args)
        {
            StartService();

            if (!File.Exists(Directories.databaseAES))
            {
                CreateDataBase();
            }
            else
            {
                DecryptFileBD();
            }
            
            FirstMenu();
        }

        public static void EncryptFileBD()
        {
            Console.WriteLine("Introduza uma password para encriptar a base de dados");

            string password = Console.ReadLine();

            byte[] bytesToBeEncrypted = File.ReadAllBytes(Directories.database);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hashes the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES.AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            File.WriteAllBytes(Directories.databaseAES, bytesEncrypted);

            FileInfo f = new FileInfo(Directories.database);

            try
            {
                f.Delete();
            }
            catch(IOException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public static void DecryptFileBD()
        {
            Console.WriteLine("Introduza a password para desencriptar a base de dados.");

            string password = Console.ReadLine();

            byte[] bytesToBeDecrypted = File.ReadAllBytes(Directories.databaseAES);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES.AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            File.WriteAllBytes(Directories.database, bytesDecrypted);
        }

        private static void StartService()
        {
            ServiceBase.Run(new VerificationService());
            Console.Clear();
        }

        private static bool CreateDataBase()
        {
            if (!File.Exists(Directories.database))
            {
                AjudanteParaBD.OnCreate();
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void FirstMenu()
        {
            bool tmp;
            string opcao;

            //verificação da introdução do user
            do
            {
                tmp = true;

                Console.WriteLine("+----------------------------------------------------------+");
                Console.WriteLine("| Estou-a-Ver: um Monitor para Integridade para Diretorias |");
                Console.WriteLine("+----------------------------------------------------------+");
                Console.WriteLine("\n1 - Entrar");
                Console.WriteLine("2 - Registar");
                Console.WriteLine("0 - Sair");
                Console.Write("\nOpcao escolhida: ");
                opcao = Console.ReadLine();

                if (!opcao.Equals("1") && !opcao.Equals("2") && !opcao.Equals("0"))
                {
                    Console.Clear();
                    Console.WriteLine("\nOpcao invalida. Introduza uma das opcoes enunciadas!");
                    tmp = false;
                }
            } while (!tmp);

            //opções depois do user escolher no meu
            switch (int.Parse(opcao))
            {
                case 0:
                    Console.WriteLine("Programa terminado.");
                    EncryptFileBD();
                    Environment.Exit(1);
                    break;
                case 1:
                    LoginMenu();
                    break;
                case 2:
                    RegistoMenu();
                    break;
            }
        }

        private static void LoginMenu()
        {
            string username = string.Empty;
            string password = string.Empty;

            bool value;
            string SN = string.Empty;

            do
            {
                Console.WriteLine("+----------------------------------------------------------+");
                Console.WriteLine("|                        Login                             |");
                Console.WriteLine("+----------------------------------------------------------+");

                Console.WriteLine("\nIntroduza os dados solicitados!");
                Console.Write("Username: ");
                username = Console.ReadLine();
                Console.Write("Password: ");
                password = Console.ReadLine();

                value = DataBaseFunctions.Login(username, password);

                if (value == false)
                {
                    Console.WriteLine("Deseja registar um novo utilizador? Sim(s) Não(n)");
                    SN = Console.ReadLine().ToLower();

                    if (SN.Equals("s"))
                    {
                        Console.Clear();
                        RegistoMenu();
                    }

                }

            } while (value == false);

            Console.WriteLine("Bem vindo " + username + "!");
            MainMenu(username);
        }

        public static void RegistoMenu()
        {
            string username = string.Empty;
            string password = string.Empty;
            string rep = string.Empty;
            string salt = string.Empty;

            User user = null;

            do
            {
                Console.WriteLine("+----------------------------------------------------------+");
                Console.WriteLine("|                       Registo                            |");
                Console.WriteLine("+----------------------------------------------------------+");
                Console.WriteLine("\nIntroduza os dados solicitados!");
                Console.Write("Username:");

                username = Console.ReadLine();

                //verificação da password
                do
                {
                    Console.Write("Password:");
                    password = Console.ReadLine();

                    if (password.Length < 4)
                        Console.WriteLine("Erro! A password deve conter no minimo 4 caracteres.");

                } while (password.Length < 4);

                //Calcula o SALT
                salt = BitConverter.ToString(SHA256Code.GenerateSalt()).Replace("-", "");

                //calcula o REP
                rep = SHA256Code.GenerateFromText(SHA256Code.GenerateFromText(password) + salt);

                user = new User(username, rep, salt);
            } while (!DataBaseFunctions.Register(user));

            LoginMenu();
        }

        private static void MainMenu(string username)
        {
            bool b;
            string opcao;
            int result_b;

            //verificação da introdução do user
            do
            {
                b = true;
                Console.WriteLine("+----------------------------------------------------------+");
                Console.WriteLine("|                         MENU                             |");
                Console.WriteLine("+----------------------------------------------------------+");
                Console.WriteLine("");
                Console.WriteLine("1 - Adicionar Diretoria De Verificação");
                Console.WriteLine("2 - Calcular valores de hash");
                Console.WriteLine("3 - Verifcar Registos");
                Console.WriteLine("4 - Ajuda");
                Console.WriteLine("5 - Logout");
                Console.WriteLine("");
                //  Console.WriteLine("0 - Sair");
                Console.Write("\nOpcao escolhida: ");
                opcao = Console.ReadLine();

                if (opcao != "1" && opcao != "2" && opcao != "3" && opcao != "4" && opcao != "5" && opcao != "0")
                {
                    Console.Clear();
                    Console.WriteLine("\nOpcao invalida. Introduza uma das opcoes enunciadas!");
                    b = false;
                }
            } while (b != true);

            //opções depois do user escolher no meu
            result_b = int.Parse(opcao);
            switch (result_b)
            {
                case 0:
                    Console.Clear();
                    EncryptFileBD();
                    Console.WriteLine("Adeus!");
                    Environment.Exit(1);
                    break;
                case 1:
                    ReceberDiretoria();
                    MainMenu(username);
                    break;
                case 2:
                    EscolherVerificação(username);
                    MainMenu(username);
                    break;
                case 3:
                    TipoVerificacao(username);
                    MainMenu(username);
                    break;
                case 4:
                    Console.Clear();
                    HelpMenu();
                    MainMenu(username);
                    break;
                case 5:
                    //Console.WriteLine("5 - Logout");
                    FirstMenu();
                    break;
            }
        }

        public static void TipoVerificacao(string username)
        {
            bool b;
            string opcao;
            int result_b;

            do
            {
                b = true;
                Console.WriteLine("+----------------------------------------------------------+");
                Console.WriteLine("|                         MENU                             |");
                Console.WriteLine("+----------------------------------------------------------+\n");
                Console.WriteLine("1 - Verificar com SHA256");
                Console.WriteLine("2 - Verificar com HMAC\n");
                Console.WriteLine("0 - Sair");
                Console.Write("\nOpcao escolhida: ");
                opcao = Console.ReadLine();

                if (opcao != "1" && opcao != "2" && opcao != "0")
                {
                    Console.Clear();
                    Console.WriteLine("\nOpcao invalida. Introduza uma das opcoes enunciadas!");
                    b = false;
                }
            } while (b != true);

            result_b = int.Parse(opcao);
            switch (result_b)
            {
                case 0:
                    break;
                case 1:
                    VerificarDiretoria();
                    MainMenu(username);
                    break;
                case 2:
                    Verificarhmac();
                    MainMenu(username);
                    break;
            }
        }

        private static string ReceberDiretoria()
        {
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("|               Ler Directoria                             |");
            Console.WriteLine("+----------------------------------------------------------+");

            string mainPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string sql = string.Empty;
            string option = string.Empty;
            string relativePath = string.Empty;

            do
            {
                Console.WriteLine("Deseja criar uma pasta na raiz do programa sim(S) não(N)");
                option = Console.ReadLine().ToLower();

                if (option.Equals("s"))
                {
                    Console.WriteLine("Qual será o nome da pasta que ira conter os ficheiros a verificar?");
                    relativePath = Console.ReadLine();

                    while (Directory.Exists(relativePath))
                    {
                        Console.WriteLine("Já existe uma pasta com esse nome, por favor dê outro nome!");
                        relativePath = Console.ReadLine();
                    }

                    Console.WriteLine("A sua pasta foi criada em: ");
                    Console.WriteLine(mainPath + "\\" + relativePath);

                    // Adicionar esta directoria à base de dados
                    Dir dir = new Dir(mainPath + "\\" + relativePath, DataBaseFunctions.userLog);

                    if (AjudanteParaBD.InsertDirectory(dir) == -1)
                    {
                        Console.WriteLine("Erro ao criar a diretoria");
                        return string.Empty;
                    }

                    Directory.CreateDirectory(relativePath);
                    return mainPath + "\\" + relativePath;
                }

                else if (option.Equals("n"))
                {

                    Console.WriteLine("Introduza o caminho absoluto da diretoria que pretende verificar.");
                    Console.WriteLine("Ex.: C:/Caminho/Completo/Ate/A/Diretoria");
                    Console.Write("Introduzido:");

                    mainPath = Console.ReadLine();

                    while (!Directory.Exists(mainPath))
                    {
                        Console.WriteLine("O caminho introduzido não existe na sua maquina");
                        Console.WriteLine("Introduza o caminho absoluto da pasta que você deseja verificar");
                        Console.Write("Introduzido:");
                        mainPath = Console.ReadLine();
                    }

                    // Adicionar esta directoria à base de dados
                    Dir dir = new Dir(mainPath, DataBaseFunctions.userLog);

                    if (AjudanteParaBD.InsertDirectory(dir) == -1)
                    {
                        Console.WriteLine("Erro ao adicionar a diretoria na base de dados.");
                        return string.Empty;
                    }

                    return mainPath;
                }

            } while (true);
        }

        public static void EscolherVerificação(string username)
        {

            string opcao;
            bool flag = true;
            int result_b;
            do
            {

                Console.WriteLine("+----------------------------------------------------------+");
                Console.WriteLine("|               Tipo de Directoria                         |");
                Console.WriteLine("+----------------------------------------------------------+");

                Console.WriteLine("Deverá escolher uma opeção para verificar os ficheiros!");
                Console.WriteLine("1 - Criar o SHA256 dos ficheiros que estão na sua directoria.");
                Console.WriteLine("2 - Criar um HMAC dos ficheiros que estão na sua directoria.");
                Console.WriteLine("0 - Voltar para traz");
                Console.WriteLine("");
                Console.Write("\nOpcao escolhida: ");
                opcao = Console.ReadLine();

                if (opcao != "1" && opcao != "2" && opcao != "0")
                {
                    Console.Clear();
                    Console.WriteLine("\nOpcao invalida. Introduza uma das opcões enunciadas!");
                    flag = false;
                }


            } while (flag != true);

            //opções depois do user escolher no meu
            result_b = int.Parse(opcao);
            switch (result_b)
            {
                case 0:
                    Console.Clear();
                    MainMenu(username);
                    break;
                case 1:  
                    VerificarDiretoria();
                    MainMenu(username);
                    break;
                case 2:
                    FilesHmac();
                    MainMenu(username);
                    break;
            }
        }

        public static void FilesHmac()
        {
            string passwd;
            string option;
            int rdopiton;
            int i;
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("|      Gerar Hmac dos ficheriros da directoria             |");
            Console.WriteLine("+----------------------------------------------------------+");

            List<Dir> directories = AjudanteParaBD.SelectDirsWithUsername(DataBaseFunctions.userLog);

            if (directories == null)
            {
                Console.WriteLine("Erro durante a leitura da base de dados.");
                return;
            }
            if (directories.Count == 0)
            {
                Console.WriteLine("Erro! Voce nao tem nenhuma diretoria adicionada.");
                return;
            }

            for (i = 0; i < directories.Count; i++)
                Console.WriteLine(i + " - " + directories[i].path);
            do
            {
                Console.WriteLine("Escolha uma direcotria!");
                option = Console.ReadLine();


                rdopiton = int.Parse(option);

            } while (rdopiton > i);

            Console.WriteLine("Introduza a password com que pretende gerar os HMAC");

            passwd = Console.ReadLine();

            var hmacs = HMac.hmac(directories[rdopiton].path, passwd);

            foreach (var hmac in hmacs)
            {
                Console.WriteLine(hmac);
                AjudanteParaBD.InsertFileHMAC(new FileHmac(hmac.Key, hmac.Value, directories[rdopiton].path));
            }
        }

        private static void VerificarDiretoria()
        {
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("|               Ler Directoria                             |");
            Console.WriteLine("+----------------------------------------------------------+");

            List<Dir> directories = AjudanteParaBD.SelectDirsWithUsername(DataBaseFunctions.userLog);
            int index = 0;

            if (directories == null)
            {
                Console.WriteLine("Erro durante a leitura da base de dados.");
                return;
            }
            if (directories.Count == 0)
            {
                Console.WriteLine("Erro! Voce nao tem nenhuma diretoria adicionada.");
                return;
            }

            for (int i = 0; i < directories.Count; i++)
                Console.WriteLine(i + " - " + directories[i].path);

            do
            {
                Console.Write("Diretoria a verificar: ");
                index = int.Parse(Console.ReadLine());

                if (index > directories.Count - 1 || index < 0)
                    Console.WriteLine("Erro! Escolha uma opcao valida.");

            } while (index > directories.Count - 1 || index < 0);

            Console.WriteLine(string.Empty);
            Console.WriteLine("Diretoria escolhida: " + directories[index].path);

            DataBaseFunctions.VerificarIntegridade(directories[index]);
        }

        public static void Verificarhmac()
        {

            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("|               Ler Directoria                             |");
            Console.WriteLine("+----------------------------------------------------------+");

            List<Dir> directories = AjudanteParaBD.SelectDirsWithUsername(DataBaseFunctions.userLog);
            int index = 0;

            if (directories == null)
            {
                Console.WriteLine("Erro durante a leitura da base de dados.");
                return;
            }
            if (directories.Count == 0)
            {
                Console.WriteLine("Erro! Voce nao tem nenhuma diretoria adicionada.");
                return;
            }

            for (int i = 0; i < directories.Count; i++)
                Console.WriteLine(i + " - " + directories[i].path);

            do
            {
                Console.Write("Diretoria a verificar: ");
                index = int.Parse(Console.ReadLine());

                if (index > directories.Count - 1 || index < 0)
                    Console.WriteLine("Erro! Escolha uma opcao valida.");

            } while (index > directories.Count - 1 || index < 0);

            Console.WriteLine(string.Empty);
            Console.WriteLine("Diretoria escolhida: " + directories[index].path.ToString());

            Console.WriteLine("Introduza a sua palavra pass com que gerou os HMAC");
            string pass = Console.ReadLine();

            DataBaseFunctions.VerificarIntegridadeHMAC(directories[index], pass);
        }

        private static void HelpMenu()
        {
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("|                         AJUDA                            |");
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("*    1 - Ler Diretoria                                     *");
            Console.WriteLine("*        -> Pede ao utilizador para introduzir a diretoria *");
            Console.WriteLine("*        absoluta onde têm os documentos.                  *");
            Console.WriteLine("*        -> Pede ao utilizador para criar uma pasta numa   *");
            Console.WriteLine("*        diretoria definida pelo programa.                 *");
            Console.WriteLine("*        -> Mostra a diretoria onde foi criada a pasta e   *");
            Console.WriteLine("*        onde o utilizador têm que colocar os documentos.  *");
            Console.WriteLine("*    2 - Verificar Diretoria                               *");
            Console.WriteLine("*        -> Verifica se a diretoria especificada no po     *");
            Console.WriteLine("*    3 - Apagar Registos                                   *");
            Console.WriteLine("*        -> Apaga o registo na base de dados do utilizador *");
            Console.WriteLine("*        que estiver logado.                               *");
            Console.WriteLine("*    4 - Ajuda                                             *");
            Console.WriteLine("*        -> Lista as opções do programa.                   *");
            Console.WriteLine("*    5 - Logout                                            *");
            Console.WriteLine("*        -> Termina a sessão do utilizador logado.         *");
            Console.WriteLine("*    0 - SAIR                                              *");
            Console.WriteLine("*        -> Sai do programa.                               *");
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.Write("Pressione qualquer tecla para continuar...");
            Console.ReadLine();
        }
    }
}
