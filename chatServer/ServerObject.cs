using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace ChatServer
{
    class ServerObject
    {
        private TcpListener tcpListener;
        static private List<ClientObject> ClientList = new List<ClientObject>();
        public void Listen()
        {
            Console.WriteLine("Ожидание подключений...");
            TcpClient client = null;
            ClientObject clientObject = null;
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                while (true)
                {
                    client = tcpListener.AcceptTcpClient();
                    clientObject = new ClientObject(client, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Procces));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                DelConnection(clientObject.ID);
                Console.WriteLine(ex.Message);
                tcpListener.Stop();
                client.Close();
               
            }
        }
        static protected internal void AddConection(ClientObject clientObj)
        {
            ClientList.Add(clientObj);
        }

        static protected internal void DelConnection(string id)
        {
            int count = 0;
            foreach (ClientObject tmp in ClientList)
            {
                if (tmp.ID == id)
                {
                    ClientList.RemoveAt(count);
                    break;
                }
                count++;
            }
        }
        internal void SendToAll(string msg, string id)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            foreach (ClientObject tmp in ClientList)
            {
                if (tmp.ID != id)
                    tmp.stream.Write(data, 0, data.Length);
            }
        }
    }
}