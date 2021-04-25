using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace chatClient
{
    class ClientMain
    {
        private const int port = 8888;
        private const string server = "127.0.0.1";
        static TcpClient client = null;
        static NetworkStream stream = null;
        static void Main(string[] args)
        {

            try
            {
                client = new TcpClient();
                client.Connect(server, port);
                stream = client.GetStream();

                Console.WriteLine("Введите ваше имя: ");
                string userName = Console.ReadLine();

                byte[] data = Encoding.UTF8.GetBytes(userName);
                stream.Write(data);

                Console.WriteLine("Добро пожаловать, {0}", userName);
                Thread thread = new Thread(new ThreadStart(ReceiveMessage));
                thread.Start();

                SendMessage(userName);

                //byte ch;
                //do
                //{
                //    Console.WriteLine("1. Групповой чат");
                //    Console.WriteLine("2. Личное сообщение");
                //    Console.WriteLine("3. Выход");

                //    //добавить форматный ввод
                //    ch = Convert.ToByte(Console.ReadLine());

                //    switch (ch)
                //    {
                //        case 1: 
                //            SendMessage(userName); break;
                //        case 2:
                //            Console.WriteLine("Кому?");
                //            SendMessage(userName, Console.ReadLine()); break;
                //    }
                //    Console.Clear();
                //} while (ch != 3);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
                stream.Close();
            }
        }

        static void ReceiveMessage()
        {

            while (true)
            {
                try
                {
                    StringBuilder builder = new StringBuilder();
                    byte[] data = new byte[256];
                    do
                    {
                        int bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string msg = builder.ToString();
                    msg = Aes.Cryptography.Decrypt(msg, "pass");
                    Console.WriteLine(msg);
                }
                catch
                {
                    Console.WriteLine("Подключение прервано![ReceiveMessage]");
                    client.Close();
                    stream.Close();
                    break;
                }
            }
        }

        static void SendMessage(string from, string to = null)
        {
            byte[] data = null;
            while (true)
            {
                try
                {
                    string msg = Console.ReadLine();

                    if (msg == "/online")
                        data = Encoding.UTF8.GetBytes("-2");    //список людей онлайн

                    if (msg == "/exit")
                        return;

                    if (data == null)
                    {
                        to ??= "-1";
                        data = Encoding.UTF8.GetBytes(to);
                    }

                    stream.Write(data, 0, data.Length);

                    msg = from + ": " + msg;
                    //AES шифрование
                    msg = Aes.Cryptography.Encrypt(msg, "pass");
                    data = Encoding.UTF8.GetBytes(msg);
                    stream.Write(data, 0, data.Length);
                    data = null;
                }
                catch
                {
                    Console.WriteLine("Подключение прервано![SendMessage]");
                    client.Close();
                    stream.Close();
                    break;
                }
            }
        }
    }
}