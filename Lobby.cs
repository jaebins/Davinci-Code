using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DaVinci_Code
{
    public partial class Lobby : Form
    {
        public Dictionary<Socket, string> clientList = new Dictionary<Socket, string>();

        public Lobby()
        {
            InitializeComponent();
        }

        private void but_CreateRoom_Click(object sender, EventArgs e)
        {
            InputPlayerCount.Text = "2"; // 삭제하셔
            int count = 0;
            bool isNumCheck = Int32.TryParse(InputPlayerCount.Text, out count);
            if (isNumCheck)
            {
                if(count <= 4 && count > 1)
                {
                    Console.WriteLine(GetMyIP());
                    Server server = new Server(GetMyIP(), InputPlayerCount.Text);
                    Thread t1 = new Thread(server.ServerStart);
                    t1.IsBackground = true;
                    t1.Start();

                    InGame game = new InGame(InputAddress.Text, GetMyIP(), true);
                    game.Show();
                }
            }
            else
            {
                MessageBox.Show("올바른 숫자를 입력해주세요.");
            }
        }

        private void but_JoinRoom_Click(object sender, EventArgs e)
        {
            InputAddress.Text = "192.168.56.1"; //삭제하셔
            //InputAddress.Text = "10.0.2.15"; //삭제하셔
            InGame game = new InGame(InputAddress.Text, GetMyIP(), false);
            game.Show();
        }

        private string GetMyIP()
        {
            IPHostEntry host = Dns.GetHostByName(Dns.GetHostName());
            string myip = host.AddressList[0].ToString();
            return myip;
        }

        private void but_MyIPShow_Click(object sender, EventArgs e)
        {
            MessageBox.Show(GetMyIP());
        }
    }
}
