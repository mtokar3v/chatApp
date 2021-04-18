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
        string userName;
        internal string ID;
        TcpClient tcp;

        public ClientObject(TcpClient clientObj, ServerObject serverObj)
        {
            tcp = clientObj;
            stream = clientObj.GetStream();
            server = serverObj;
            ServerObject.AddConection(this);
            ID = Guid.NewGuid().ToString();
        }


        public void Procces()
        {
            try
            {
                //клиент в первую очередь отправляет свой ник-нейм
                userName = GetMessege();
                Console.WriteLine(userName);
                server.SendToAll(userName + " вошел в чат", this.ID);

                while (true)
                {
                    try
                    {
                        string msg = this.userName + ": " + GetMessege();
                        Console.WriteLine(msg);
                        server.SendToAll(msg, this.ID);
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