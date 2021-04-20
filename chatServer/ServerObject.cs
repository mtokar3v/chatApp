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
        internal TcpListener tcpListener;
        static private List<ClientObject> ClientList = new List<ClientObject>();
        static private List<List<ClientObject>> PrivateMessages = new List<List<ClientObject>>();
        TcpClient client = null;
        ClientObject clientObject = null;
        public void Listen()
        {
            Console.WriteLine("Ожидание подключений...");
            
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
                Console.WriteLine(ex.Message);
                disconnect(this);
            }
        }
        static protected internal void AddConection(ClientObject clientObjFIRST)
        {
            ClientList.Add(clientObjFIRST);
            List<ClientObject> tmp = new List<ClientObject>();
            tmp.Add(clientObjFIRST);
            PrivateMessages.Add(tmp);
        }

        static protected internal void AddConectionWith(ClientObject clientObjFIRST, ClientObject clientObjSECOND)
        {
            foreach (List<ClientObject> i in PrivateMessages)
            {
                //0 элемент - сам клиент, последующие - его связи
                if (i[0] == clientObjFIRST)
                    i.Add(clientObjSECOND);
            }

            foreach (List<ClientObject> i in PrivateMessages)
            {
                //если начать чат с одним, то чат должен начаться и с другим
                if (i[0] == clientObjSECOND)
                    i.Add(clientObjFIRST);
            }
        }

        //fix id = name
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

        static protected internal void DelConnection(string idFrom, string idTO)
        {
            int count = 0;
            foreach (List<ClientObject> i in PrivateMessages)
            { 
                if (i[0].ID == idFrom)
                {
                    foreach (ClientObject j in i)
                    {
                        if (j.ID == idTO)
                        {
                            i.RemoveAt(count);
                            break;
                        }
                        count++;
                    }
                    break;
                }
            }
        }

        //предположим, что idTO - username
        internal void SendTo(string msg, string idFrom, string idTO)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);

            try
            {
                foreach (List<ClientObject> i in PrivateMessages)
                {
                    if (i[0].ID == idFrom)
                    {
                        foreach (ClientObject j in i)
                        {
                            if (j.userName == idTO)
                            {
                                j.stream.Write(data, 0, data.Length);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            catch(ArgumentOutOfRangeException)   
            {
                Console.WriteLine("Пользователя не существует");
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

        public void disconnect(ServerObject server)
        {
            DelConnection(server.clientObject.ID);
            server.tcpListener.Stop();
            server.client.Close();
        }

        //пока что id - это name
        static public ClientObject getClientFromID(string id)
        {
            foreach (ClientObject tmp in ClientList)
                if (tmp.userName == id)
                    return tmp;
            return null;
        }
    }
}