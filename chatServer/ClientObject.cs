using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ChatServer
{
    class ClientObject
    {
        internal NetworkStream stream;
        ServerObject server;
        internal string userName { get; set; }
        internal string ID;
        TcpClient tcp;

        public ClientObject(TcpClient clientObj, ServerObject serverObj)
        {
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
                string id = GetMessege();
                string msg;
                while (true)
                {
                    try
                    {
                        msg = this.userName + ": " + GetMessege();
                        Console.WriteLine(msg);

                        if (id == "-1")
                            server.SendToAll(msg, this.ID);
                        else
                        {
                            ClientObject tmp = ServerObject.getClientFromID(id);
                            if (tmp != null)
                            {
                                ServerObject.AddConectionWith(this, tmp);
                                server.SendTo(msg, this.ID, id);
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

        //private int GetID()
        //{
        //    byte[] data = new byte[256];
        //    StringBuilder builder = new StringBuilder();

        //    do
        //    {
        //        int bytes = stream.Read(data, 0, data.Length);
        //        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
        //    }
        //    while (stream.DataAvailable);

        //    return Convert.ToInt32(builder.ToString());
        //}
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