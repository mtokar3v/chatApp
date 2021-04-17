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

            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                while (true)
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Procces));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //нужно по-хорошему отключиться
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
                    ClientList.RemoveAt(count);
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