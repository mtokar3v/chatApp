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

        public ClientObject(TcpClient clientObj, ServerObject serverObj)
        {
            stream = clientObj.GetStream();
            server = serverObj;
            ServerObject.AddConection(this);
            ID = Guid.NewGuid().ToString();
        }

       
        public void Procces()
        {
            //клиент в первую очередь отправляет свой ник-нейм
            userName = GetMessege();
            Console.WriteLine(userName);
            

            while(true)
            {
                string msg = this.userName + ": " + GetMessege();
                Console.WriteLine(msg);
                server.SendToAll(msg, this.ID);
            }
        }

        private string GetMessege()
        {
            byte[] data = new byte[256];
            StringBuilder builder = new StringBuilder();
            try
            {
                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return builder.ToString();
        }

       
    }
}
