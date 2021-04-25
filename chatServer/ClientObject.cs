using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ChatServer
{
    class ClientObject
    {
        internal bool active;
        internal NetworkStream stream;
        private ServerObject server;
        internal string userName { get; set; }
        internal string id;
        private TcpClient tcp;

        public ClientObject(TcpClient clientObj, ServerObject serverObj)
        {
            active = true;
            tcp = clientObj;
            stream = clientObj.GetStream();
            server = serverObj;
            id = Guid.NewGuid().ToString();
            ServerObject.AddConection(this);
        }

        public void Procces()
        {
            try
            {
                userName = GetMessege();
                Console.WriteLine(userName);
                server.SendToAll(userName + " вошел в чат", this.id);

                string msg_info;
                string msg;
                while (true)
                {
                    try
                    {
                        msg_info = Response();
                        msg = GetMessege();

                        Console.WriteLine(msg);

                        if (msg_info == "-1")
                        {
                            server.SendToAll(msg, this.id);
                            continue;
                        }
                        else if (msg_info == "-2")
                        {
                            foreach (byte[] clientdata in ServerObject.GetList())
                                stream.Write(clientdata);
                            continue;
                        }

                        ClientObject tmp = ServerObject.getClientFromNAME(msg_info);
                        if (tmp != null && tmp.active)
                        {
                           ServerObject.AddConectionWith(this, tmp);
                           server.SendTo(msg, this.id, msg_info);
                        }
                        else
                            Console.WriteLine("пользователя не существует");

                    }
                    catch
                    {
                        Console.WriteLine("-" + userName);
                        server.SendToAll(userName + " покинул чат", this.id);
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ServerObject.DelConnection(id);
                stream.Close();
                tcp.Close();
            }
        }

        private string Response()
        {
            byte[] data = new byte[512];
            StringBuilder builder = new StringBuilder();

            int bytes = stream.Read(data, 0, data.Length);
            builder.Append(Encoding.UTF8.GetString(data, 0, bytes));

            return builder.ToString();
        }
        private string GetMessege()
        {
            byte[] data = new byte[512];
            StringBuilder builder = new StringBuilder();
            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            return builder.ToString();
        }


    }
}