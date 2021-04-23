//#define DEBUG
#define RELEASE
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
#if RELEASE
                server = new ServerObject();
                Thread thread = new Thread(new ThreadStart(server.Listen));
                thread.Start();
#elif DEBUG
                server.Listen();
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                server.disconnect(server);
            }
        }
    }
}