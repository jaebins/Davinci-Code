using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DaVinci_Code
{
    class Server
    {
        string myIP;
        int playerCounter = 0;
        int allPlayerCounter = 0;
        
        public Dictionary<Socket, string> clientList = new Dictionary<Socket, string>();

        public Server(string myIP, string allPlayerCounter)
        {
            this.myIP = myIP;
            this.allPlayerCounter = Int32.Parse(allPlayerCounter);
        }

        public void ServerStart()
        {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Parse(myIP), 8000));
            server.Listen(10);
            Console.WriteLine("Server : 서버 오픈");

            while (true)
            {
                try
                {
                    Socket client = server.Accept();
                    clientList.Add(client, playerCounter.ToString());
                    Console.WriteLine("Server : " + client.RemoteEndPoint.ToString() + " 입장");

                    // 0 : 현재 플레이어 수, 1 : 총 받아야할 플레이어 수
                    SendMessage(playerCounter.ToString() + '+' + allPlayerCounter, playerCounter.ToString(), true);
                    playerCounter++;

                    Handler handler = new Handler();
                    handler.ReceiveMessageEvent += new Handler.ReceiveMessageDeputy(ReceiveMessage);
                    handler.DisconnectServerEvent += new Handler.DisconnectServerDeputy(OnDisconnect);
                    handler.ThreadStart(client);

                    
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

            server.Close();
        }

        private void ReceiveMessage(string msg, string senderUser)
        {
            SendMessage(msg, senderUser, false);
        }

        private void SendMessage(string msg, string senderUser, bool isFlag)
        {
            foreach (var pair in clientList)
            {
                Socket client = pair.Key;
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                client.Send(buffer);
            }
        }

        private void OnDisconnect(Socket client)
        {
            if (clientList.ContainsKey(client))
            {
                allPlayerCounter--;
                clientList.Remove(client);
            }
        }
    }
}
