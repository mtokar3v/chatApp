//#define DEBUG
#define RELEASE
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
#if RELEASE
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Procces));
                    clientThread.Start();
#elif DEBUG
                    clientObject.Procces();
#endif
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

        static protected internal void DelConnection(string id)
        {
            int count = 0;
            foreach (ClientObject tmp in ClientList)
            {
                if (tmp.ID == id)
                {
                    tmp.active = false;
                    ClientList.RemoveAt(count);
                    break;
                }
                count++;
            }

            //чистка баз данных

            //count = 0;
            //foreach (List<ClientObject> i in PrivateMessages)
            //{
            //    if (i[0].ID == id)
            //    {
            //        count = 0;
            //        foreach (ClientObject j in i)
            //        {
            //            i.RemoveAt(count);
            //            count++;
            //        }
            //    }
            //    else
            //    {
            //        count = 0;
            //        foreach (ClientObject j in i)
            //        {
            //            if (j.ID == id)
            //            {
            //                i.RemoveAt(count);
            //                break;
            //            }
            //            count++;
            //        }
            //    }
            //}
        }

        internal void SendTo(string msg, string idFrom, string nameTO)
        {
           // byte[] nameData = Encoding.UTF8.GetBytes(name+": ");
            byte[] data = Encoding.UTF8.GetBytes(msg);

            try
            {
                foreach (List<ClientObject> i in PrivateMessages)
                {
                    if (i[0].ID == idFrom)
                    {
                        foreach (ClientObject j in i)
                        {
                            if (j.userName == nameTO)
                            {
                               // j.stream.Write(nameData, 0, nameData.Length);
                                j.stream.Write(data, 0, data.Length);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Пользователя не существует");
            }
        }
        internal void SendToAll(string msg, string id)
        {
            //byte[] nameData = Encoding.UTF8.GetBytes(name+": ");
            byte[] data = Encoding.UTF8.GetBytes(msg);
            foreach (ClientObject tmp in ClientList)
            {
                if (tmp.ID != id && tmp.active)
                {
                   // tmp.stream.Write(nameData, 0, nameData.Length);
                    tmp.stream.Write(data, 0, data.Length);
                }
            }
        }

        public void disconnect(ServerObject server)
        {
            DelConnection(server.clientObject.ID);
            server.tcpListener.Stop();
            server.client.Close();
        }

        static public ClientObject getClientFromNAME(string name)
        {
            foreach (ClientObject tmp in ClientList)
                if (tmp.userName == name)
                    return tmp;
            return null;
        }

        static public List<byte[]> GetList()
        {
            List<byte[]> datalist = new List<byte[]>();
            foreach (ClientObject i in ClientList)
                if (i.active)
                    datalist.Add(Encoding.UTF8.GetBytes(i.userName + '\n'));
            return datalist;
        }
    }
}