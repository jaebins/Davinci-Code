using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DaVinci_Code
{
    class Handler
    {
        public delegate void ReceiveMessageDeputy(string Message, string senderUser);
        public event ReceiveMessageDeputy ReceiveMessageEvent;

        public delegate void DisconnectServerDeputy(Socket client);
        public event DisconnectServerDeputy DisconnectServerEvent;

        Socket client;

        public void ThreadStart(Socket client)
        {
            this.client = client;

            Thread t1 = new Thread(HandlerStart);
            t1.IsBackground = true;
            t1.Start();
        }

        private void HandlerStart()
        {
            int length = 0;
            byte[] buffer = new byte[1024];
            string msg = String.Empty;

            while (true)
            {
                try
                {
                    length = client.Receive(buffer);
                    msg = Encoding.UTF8.GetString(buffer, 0, length);
                    ReceiveMessageEvent(msg, client.RemoteEndPoint.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine(se.Message);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }

            DisconnectServerEvent(client);
            client.Close();
        }
    }
}
