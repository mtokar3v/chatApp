using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace ChatServer
{
    class ClientObject
    {
        internal bool active;
        internal NetworkStream stream;
        private ServerObject server;
        internal string userName { get; set; }
        internal string ID;
        private TcpClient tcp;

        public ClientObject(TcpClient clientObj, ServerObject serverObj)
        {
            active = true;
            tcp = clientObj;
            stream = clientObj.GetStream();
            server = serverObj;
            ID = Guid.NewGuid().ToString();
            ServerObject.AddConection(this);
        }

        public void Procces()
        {
            try
            {
                //клиент в первую очередь отправляет свой ник-нейм
                userName = GetMessege();
                Console.WriteLine(userName);
                server.SendToAll(userName + " вошел в чат", this.ID);
                string friendName;
                string msg;
                while (true)
                {
                    try
                    {
                        friendName = GetID();
                        msg = this.userName + ": " + GetMessege();
                        Console.WriteLine(msg);

                        if (friendName == "-1")
                            server.SendToAll(msg, this.ID);
                        else if(friendName == "-2")
                        {

                            List<byte[]> tmp = ServerObject.GetList();
                            foreach(byte[] clientdata in tmp)
                                stream.Write(clientdata);
                        }
                        else
                        {
                            ClientObject tmp = ServerObject.getClientFromNAME(friendName);
                            if (tmp != null && tmp.active)
                            {
                                ServerObject.AddConectionWith(this, tmp);
                                server.SendTo(msg, this.ID, friendName);
                            }
                            else
                                Console.WriteLine("пользователя не существует");
                        }

                      
                    }
                    catch
                    {
                        Console.WriteLine("-" + userName);
                        server.SendToAll(userName + " покинул чат", this.ID);
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
                ServerObject.DelConnection(ID);
                stream.Close();
                tcp.Close();
            }
        }

        private string GetID()
        {
            byte[] data = new byte[256];
            StringBuilder builder = new StringBuilder();

            int bytes = stream.Read(data, 0, data.Length);
            builder.Append(Encoding.UTF8.GetString(data, 0, bytes));

            return builder.ToString();
        }
        private string GetMessege()
        {
            byte[] data = new byte[256];
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