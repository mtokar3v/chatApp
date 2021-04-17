using System;
using System.Threading;
using System.Text;

namespace ChatServer
{
    class ServerMain
    {
        static ServerObject server = null;
        static void Main(string[] args)
        {
            try
            {
                server = new ServerObject();
                Thread thread = new Thread(new ThreadStart(server.Listen));
                thread.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}