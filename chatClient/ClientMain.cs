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

                //Console.WriteLine("1. Групповой чат");
                //Console.WriteLine("2. Шептать");
                //string ch = Console.ReadLine();

                Console.WriteLine("Кому?");
                string username = Console.ReadLine();

                SendMessage(username);
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
                    Console.WriteLine(msg);

                }
                catch
                {
                    Console.WriteLine("Подключение прервано!");
                    client.Close();
                    stream.Close();
                    break;
                }
            }
        }

        static void SendMessage(string TO = null)
        {
            //предполагается, что TO - это ID
            while (true)
            {
                byte[] data;
                if (String.IsNullOrEmpty(TO))
                    data = Encoding.UTF8.GetBytes("-1");    //общий чат
                else
                    data = Encoding.UTF8.GetBytes(TO);

                stream.Write(data, 0, data.Length);

                string msg = Console.ReadLine();
                data = Encoding.UTF8.GetBytes(msg);
                stream.Write(data, 0, data.Length);
            }
        }

       
       
    }
}