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
    public partial class InGame : Form
    {
        // 기본 설정
        GameRule rule = new GameRule();
        PlayersBlockManage playerBlocks = new PlayersBlockManage();


        // 게임 관련 설정
        int[] player = new int[7];
        int labelCount = 0;
        int backupSender = 0;
        TextBox inputNum = new TextBox();
        Button but_GameStart = new Button();
        Label turnLabel = new Label();
        Color backupFontColor;
        Color backupFontBackgroundColor;

        bool isGiveRandomButActive = false;
        bool isGameStart = false;
        bool isReceiveBlockMode = false;
        bool isFillBlockCheck = false;

        // 플레이어 관련 설정
        int playersCount = -1;
        int playerNumber = -1;
        int playerBlockCount = 4; // 현재 블럭 갯수
        int targetPlayerNumber = 0;
        int nowPlayerCount = -1;
        int nowTurn = 0;

        // 블럭 관련 설정
        Label selectBlock = new Label();
        int selBlockLoc = 0;
        bool isClickCenter = false;

        //서버 관련 설정
        Socket client;
        string tarIP = string.Empty;
        string myIP = string.Empty;

        public InGame(string tarIP, string myIP, bool isServerOpen)
        {
            this.tarIP = tarIP;
            if (isServerOpen)
            {
                this.tarIP = myIP;
            }
            this.myIP = myIP;

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FillBlockSource();
            SetClientSizeCore(rule.gameScreenSize.X, rule.gameScreenSize.Y);
            Thread t1 = new Thread(ServerJoin);
            t1.IsBackground = true;
            t1.Start();
        }

        private void GameSetting() // 게임 설정
        {
            AllBlockOrganizeSet();
            FirstSetting(); // 화면에 표시
        }

        private void ServerJoin()
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(new IPEndPoint(IPAddress.Parse(tarIP), 8000));
            } catch(SocketException se)
            {
                MessageBox.Show(se.Message);
                Application.Exit();
            } catch(Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Exit();
            }

            byte[] buffer = new byte[1024];
            int length = 0;
            string msg = String.Empty;
            string[] cutMsg;

            while (true)
            {
                try
                {
                    length = client.Receive(buffer);
                    msg = Encoding.UTF8.GetString(buffer, 0, length);

                    if (msg.Contains("GameStart"))
                    {
                        foreach (var pair in playerBlocks.playerBlocks)
                        {
                            if (pair.Key == playerNumber) player = pair.Value;
                        }
                        isReceiveBlockMode = true;
                        isGameStart = true;
                        //MessageBox.Show("게임 시작");
                    }

                    if (isGameStart && isReceiveBlockMode && !msg.Contains("GameStart")) // 블럭 초기 세팅
                    {
                        if (playerNumber != 0)
                        {
                            cutMsg = msg.Split(':');
                            if (cutMsg[0].Equals("complete")) // 완료 됬다면
                            {
                                isReceiveBlockMode = false;
                                GameSetting();
                            }
                            else
                            {
                                string[] cutPlayer = msg.Split('/');
                                for(int i = 0; i < playersCount; i++)
                                {
                                    cutMsg = cutPlayer[i].Split(':');
                                    for(int j = 0; j < 4; j++)
                                    {
                                        playerBlocks.playerBlocks[i][j] = Int32.Parse(cutMsg[j]);
                                        rule.noSame[Int32.Parse(cutMsg[j])] = true;
                                    }
                                }
                            }
                        }
                    }

                    if (!isGameStart && !isReceiveBlockMode) // 게임 시작 이전이라면
                    {
                        cutMsg = msg.Split('+');

                        if (playersCount == -1 || playerNumber == -1 ) // 만약 설정이 되어있지 않다면
                        {
                            playerNumber = Int32.Parse(cutMsg[0]); // 현재 플레이어 넘버
                            playersCount = Int32.Parse(cutMsg[1]); // 총 플레이어 카운트
                        }

                        if(playerNumber == 0)
                        {
                            this.Invoke(new Action(delegate ()
                            {
                                StartButDraw();
                            }));
                        }
                        nowPlayerCount = Int32.Parse(cutMsg[0]);
                    }

                    if (isGameStart && !isReceiveBlockMode && msg.Contains("Send")) // 게임 시작 후라면
                    {
                        int sendPlayerNum = 0;
                        int cardNum = 0;
                        int cardLoc = 0;

                        cutMsg = msg.Split(':'); 
                        if(cutMsg.Length > 4) // 카드 확인
                        {
                            // 0 : 신호 1: 보내는 사람, 2 : 본래 카드 배열 위치, 3 : 보이는 값, 4 : 성공했는지 여부, 5 : 현재턴 (턴넘김)
                            sendPlayerNum = Int32.Parse(cutMsg[1]);
                            cardLoc = Int32.Parse(cutMsg[2]);
                            cardNum = Int32.Parse(cutMsg[3]); 
                            int isSuccess = Int32.Parse(cutMsg[4]);

                            if (isSuccess == 0)
                            {

                                MessageBox.Show(sendPlayerNum + "번 플레이어가 틀리셨습니다.");
                                nowTurn = Int32.Parse(cutMsg[5]);
                            }
                            else
                            {
                                MessageBox.Show(sendPlayerNum + "번 플레이어가 맞추셨습니다.");
                                playerBlocks.playerBlocksImage[sendPlayerNum][cardLoc].Text = cardNum.ToString();    
                            }

                            turnLabel.Text = "현재 턴 : " + nowTurn.ToString();
                        }
                        else // 새로운 카드 받기
                        {
                            // 0 확인 신호 1 플레이어 넘비 2 카드번호 3 인덱스 위치
                            sendPlayerNum = Int32.Parse(cutMsg[1]);
                            cardNum = Int32.Parse(cutMsg[2]);
                            cardLoc = Int32.Parse(cutMsg[3]);

                            rule.noSame[cardNum] = true;
                            playerBlocks.playerBlocks[sendPlayerNum][cardLoc] = cardNum;

                            //for (int i = 0; i < cardLoc + 1; i++)
                            //{
                            //    Controls.Remove(playerBlocks.playerBlocksImage[sendPlayerNum][i]);
                            //}

                            OrganizeBlock(playerBlocks.playerBlocks[sendPlayerNum]);
                            DrawBlockSetting(playerBlocks.playerBlocks[sendPlayerNum], sendPlayerNum, cardLoc + 1);
                        }
                    }
                }
                catch (SocketException se)
                {
                    MessageBox.Show(se.Message);
                    Application.Exit();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    Application.Exit();
                }
            }
        }

        private void but_GameStart_Click(object sender, EventArgs e)
        {
            if ((nowPlayerCount + 1 == playersCount) && !isGameStart && !isGiveRandomButActive) // 맨 마지막
            // 현재 플레이어와 수와 설정한 총 플레이어 값이 같다면
            {
                for (int i = 0; i < playersCount; i++)
                {
                    playerBlocks.playerBlocks[i] = new int[7];
                    playerBlocks.playerBlocksImage[i] = new Label[7];
                }
                foreach (var pair in playerBlocks.playerBlocks)
                {
                    if (pair.Key == playerNumber) player = pair.Value;
                }

                but_GameStart.Visible = false;
                isGiveRandomButActive = true;
                byte[] buffer = Encoding.UTF8.GetBytes("CommandGameStart");
                client.Send(buffer);

                GiveRandomBlock(); // 플레이어1의 클라이언트에서 다른 사용자들의 임의의 블럭을 설정
            }
            else if(nowPlayerCount + 1 != playersCount)
            {
                MessageBox.Show("현재 플레이어 수 : " + (nowPlayerCount + 1).ToString() + "/" + playersCount);
            }
        }

        private void FillBlockSource()
        {
            // 배경 선택 창 넣기
            this.BackgroundImage = Properties.Resources.InGameBackground;

            for (int i = 0; i < rule.block.Length; i++)
            {
                rule.block[i] = i;
                rule.noSame[i] = false;
            }

            for (int i = 0; i < 4; i++)
            {
                playerBlocks.playerBlocks[i] = new int[15];
                playerBlocks.playerBlocksImage[i] = new Label[15];
                for (int j = 0; j < 4; j++)
                {
                    playerBlocks.playerBlocksImage[i][j] = new Label();
                }
            }
        }

        private void GiveRandomBlock()
        {
            byte[] buffer = new byte[1024];

            string blockValues = string.Empty;

            if (playerNumber == 0)
            {
                for(int i = 0; i < playersCount; i++)
                {
                    int nowCount = 0;
                    Random ran = new Random();

                    while (true) // 중복 체크
                    {
                        int ranNum = ran.Next(0, 22);
                        bool isOverlap = OverlapCheck(ranNum);

                        if (!isOverlap)
                        {
                            playerBlocks.playerBlocks[i][nowCount] = ranNum;
                            rule.noSame[ranNum] = true;

                            blockValues += ranNum + ":";
                            Console.WriteLine(blockValues);

                            nowCount++;
                        }

                        if (nowCount == 4) break;
                    }

                    blockValues += "/";
                }

                // 완성된 블록 설정을 모든 클라이언트에게 보냄
                buffer = Encoding.UTF8.GetBytes(blockValues);
                client.Send(buffer);
                Thread.Sleep(1000);

                // 끝났다면
                buffer = Encoding.UTF8.GetBytes("complete:complete:complete");
                client.Send(buffer);
            }

            GameSetting();
        }

        private void FirstSetting()
        {
            DrawControl();

            for (int i = 0; i < playersCount; i++)
            {
                int[] targetPlayer = playerBlocks.playerBlocks[i];
                DrawBlockSetting(targetPlayer, i, playerBlockCount);
            }
        }

        private void AllBlockOrganizeSet()
        {
            for(int i = 0; i < playersCount; i++)
            {
                OrganizeBlock(playerBlocks.playerBlocks[i]);
            }
        }

        private bool OverlapCheck(int cardNum)
        {
            if (rule.noSame[cardNum])
            {
                return true;
            }

            return false;
        }

        private void OrganizeBlock(int[] playerBlocks) // 오름차순 정리
        {
            for(int i = 0; i < playerBlockCount; i++) // 검정색 카드를 고려한 새로운 알고리즘 짜기
            {
                int temp = playerBlocks[i];
                if (temp > 11) temp = (12 - temp) * -1;

                for(int j = 0; j < playerBlockCount; j++)
                {
                    int temp2 = playerBlocks[j];
                    if (temp2 > 11) temp2 = (12 - temp2) * -1;

                    if (temp < temp2) // temp2가 더 작다면
                    {
                        int backupBlocks = playerBlocks[i];
                        playerBlocks[i] = playerBlocks[j];
                        playerBlocks[j] = backupBlocks;
                    }
                    else if(temp == temp2)
                    {
                        if(playerBlocks[i] > playerBlocks[j])
                        {
                            int backupBlocks = playerBlocks[i];
                            playerBlocks[i] = playerBlocks[j];
                            playerBlocks[j] = backupBlocks;
                        }
                    }
                }
            }
        }

        private void DrawBlockSetting(int[] targetPlayer, int playersBlockNum, int targetLength)
        {
            labelCount = 0;

            for (int i = 0; i < targetLength; i++)
            {
                bool checkPlayer = false;
                int direction = 1;
                if (playersBlockNum == playerNumber) checkPlayer = true; // 만약 자기 참가번호랑 일치하면 숫자 보이기
                if (rule.blockPos[playersBlockNum].X > 1000) direction = -1; // 오른쪽에 있는 블럭들 방향 바꾸기

                int cardNum = targetPlayer[i];

                Point pos = new Point(rule.blockPos[playersBlockNum].X + (i * (rule.bs.X + 20) * direction), rule.blockPos[playersBlockNum].Y);
                DrawBlock(pos, cardNum, checkPlayer, playersBlockNum, i);
            }
        }

        private void DrawBlock(Point pos, int cardNum, bool checkPlayer, int sendPlayerNumber, int intBlockLoc) // 블록 그리기
        {
            // 만약 한 플레이어가 카드를 다 뽑았다면
            if (backupSender + 1 == sendPlayerNumber) 
            {
                labelCount = 0;
                backupSender++;
            }

            Color fontColor;
            Color fontBackgroundColor;
            if (cardNum> 11)
            {
                fontBackgroundColor = Color.Black;
                fontColor = Color.White;
            }
            else
            {
                fontBackgroundColor = Color.White;
                fontColor = Color.Black;
            }

            int orginalNum = cardNum;

            if (cardNum > 11)  cardNum = (12 - cardNum) * -1;

            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    Label numLabel = new Label
                    {
                        Size = new Size(rule.bs),
                        Location = pos,
                        Font = new Font("맑은 고딕", 16F, FontStyle.Bold),
                        ForeColor = fontColor,
                        BackColor = fontBackgroundColor
                    };

                    numLabel.Click += (sender, e) => { BlockClick(numLabel, orginalNum, sendPlayerNumber, intBlockLoc); };

                    playerBlocks.playerBlocksImage[sendPlayerNumber][labelCount] = numLabel;

                    if (checkPlayer)
                        numLabel.Text = cardNum.ToString();
                    else
                        numLabel.Text = "*";

                    Controls.Add(numLabel);
                });
            }
            else
            {
                Label numLabel = new Label
                {
                    Size = new Size(rule.bs),
                    Location = pos,
                    Font = new Font("맑은 고딕", 16F, FontStyle.Bold),
                    ForeColor = fontColor,
                    BackColor = fontBackgroundColor
                };

                numLabel.Click += (sender, e) => { BlockClick(numLabel, orginalNum, sendPlayerNumber, intBlockLoc); };

                MessageBox.Show(sendPlayerNumber + ":" + labelCount);
                playerBlocks.playerBlocksImage[sendPlayerNumber][labelCount] = numLabel;

                if (checkPlayer) 
                    numLabel.Text = cardNum.ToString();
                else
                    numLabel.Text = "*";

                Controls.Add(numLabel);
            }

            labelCount++;
        }

        private void DrawControl() // 컨트롤 그리기
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    turnLabel = new Label
                    {
                        BackColor = Color.LightGray,
                        Size = new Size(120, 30),
                        Location = new Point(rule.gameScreenSize.X / 2 - 60, rule.gameScreenSize.Y - 200),
                        Font = new Font("맑은 고딕", 12F, FontStyle.Bold),
                        Text = "현재 턴 : " + nowTurn.ToString()
                    };

                    Label numLabel = new Label
                    {
                        Size = new Size(rule.bs.X + 50, rule.bs.Y + 50),
                        Location = new Point(rule.gameScreenSize.X / 2 - (rule.bs.X), rule.gameScreenSize.Y / 2 - (rule.bs.Y)),
                        Font = new Font("맑은 고딕", 16F, FontStyle.Bold),
                        BackColor = Color.Aqua
                    };

                    inputNum = new TextBox
                    {
                        Size = new Size(200, 100),
                        Location = new Point(rule.gameScreenSize.X / 2 - 100, 80),
                        Font = new Font("맑은 고딕", 10F, FontStyle.Bold)
                    };

                    Button confirmBut = new Button
                    {
                        Size = new Size(60, 25),
                        Location = new Point(rule.gameScreenSize.X / 2 + 110, 80),
                        Text = "확인",
                    };

                    numLabel.Click += (sender, e) => { CenterBlockClick(); };
                    confirmBut.Click += (sender, e) => { CofirmTurn(); };

                    Controls.Add(turnLabel);
                    Controls.Add(numLabel);
                    Controls.Add(inputNum);
                    Controls.Add(confirmBut);
                });
            }
            else
            {
                Label turnLabel = new Label
                {
                    BackColor = Color.LightGray,
                    Size = new Size(120, 30),
                    Location = new Point(rule.gameScreenSize.X / 2 - 60, rule.gameScreenSize.Y - 200),
                    Font = new Font("맑은 고딕", 12F, FontStyle.Bold),
                    Text = "현재 턴 : " + nowTurn.ToString()
                };

                Label numLabel = new Label
                {
                    Size = new Size(rule.bs.X + 50, rule.bs.Y + 50),
                    Location = new Point(rule.gameScreenSize.X / 2 - (rule.bs.X), rule.gameScreenSize.Y / 2 - (rule.bs.Y)),
                    Font = new Font("맑은 고딕", 16F, FontStyle.Bold),
                    BackColor = Color.Aqua
                };

                inputNum = new TextBox
                {
                    Size = new Size(200, 100),
                    Location = new Point(rule.gameScreenSize.X / 2 - 100, 80),
                    Font = new Font("맑은 고딕", 10F, FontStyle.Bold)
                };

                Button confirmBut = new Button
                {
                    Size = new Size(60, 25),
                    Location = new Point(rule.gameScreenSize.X / 2 + 110, 80),
                    Text = "확인",
                };

                numLabel.Click += (sender, e) => { CenterBlockClick(); };
                confirmBut.Click += (sender, e) => { CofirmTurn(); };

                Controls.Add(turnLabel);
                Controls.Add(numLabel);
                Controls.Add(inputNum);
                Controls.Add(confirmBut);
            }

        }

        private void CofirmTurn()
        {
            int conNum = 0;
            if (isClickCenter && playerNumber == nowTurn)
            {
                if (int.TryParse(inputNum.Text, out conNum))
                {
                    if (conNum < 12)
                    {
                        string msg = string.Empty;
                        int[] targetPlayer = playerBlocks.playerBlocks[targetPlayerNumber];
                        int cardNum = 0;

                        if (backupFontBackgroundColor == Color.Black) // 선택한 블럭이 검은색이라면
                            cardNum = (12 - targetPlayer[selBlockLoc]) * -1;
                        else // 선택한 블럭이 하얀색이라면
                            cardNum = targetPlayer[selBlockLoc];

                        if (cardNum == conNum) // 만약 입력값과 카드 값이 일치하다면
                        {
                            MessageBox.Show("정답");
                            selectBlock.Text = cardNum.ToString();
                            msg = "Send:" + playerNumber.ToString() + ":" + selBlockLoc.ToString() + ":" + cardNum.ToString() + ":" + "1";
                        }
                        else { 
                            MessageBox.Show("오답");
                            isClickCenter = false;
                            
                            nowTurn++;
                            if (nowTurn == playersCount) nowTurn = 0;

                            // 0 : 신호 1: 보내는 사람, 2 : 본래 카드 배열 위치, 3 : 보이는 값, 4 : 성공했는지 여부, 5 : 현재턴 (턴넘김)
                            msg = "Send:" + playerNumber.ToString() + ":" + selBlockLoc.ToString() + ":" + cardNum.ToString() + ":" + "0" + ":" + nowTurn;
                        }

                        byte[] buffer = Encoding.UTF8.GetBytes(msg);
                        client.Send(buffer);

                        selectBlock.ForeColor = backupFontColor;
                        selectBlock.BackColor = backupFontBackgroundColor;
                        selectBlock = null;

                        turnLabel.Text = "현재 턴 : " + nowTurn.ToString();
                    }
                }
                else MessageBox.Show("숫자를 입력해주세요.");
            }
            else
            {
                MessageBox.Show("카드를 안 뽑았거나, 당신 턴이 아닙니다.");
            }
        }

        private void CenterBlockClick()
        {
            labelCount = 0;
            bool isCompleteSel = false;

            for (int i = 0; i < rule.noSame.Length; i++) // 멀티 구현하면 바꾸기 (카드 다 뽑았는지 확인)
            {
                if (!rule.noSame[i]) break;
                if (i == rule.noSame.Length - 1) isCompleteSel = true;
            }

            if (playerNumber == nowTurn && !isClickCenter && !isCompleteSel)
            {
                MessageBox.Show("뽑기");
                for(int i = 0; i < playerBlockCount; i++)
                {
                    Controls.Remove(playerBlocks.playerBlocksImage[playerNumber][i]);
                }
                Random ran = new Random();
                int ranNum = 0;

                while (true)
                {
                    ranNum = ran.Next(0, 22);
                    bool isOverlap = OverlapCheck(ranNum);
                    if (!isOverlap) break;
                }

                rule.noSame[ranNum] = true;
                playerBlocks.playerBlocks[playerNumber][playerBlockCount] = ranNum;
                // 0 확인 신호 1 플레이어 넘비 2 카드번호 3 인덱스 위치
                byte[] buffer = Encoding.UTF8.GetBytes("Send:" + playerNumber + ":" + ranNum + ":" + playerBlockCount);
                client.Send(buffer);

                playerBlockCount++;
                OrganizeBlock(playerBlocks.playerBlocks[playerNumber]);
                DrawBlockSetting(playerBlocks.playerBlocks[playerNumber], playerNumber, playerBlockCount);

                isClickCenter = true;
            }
        }

        private void BlockClick(Label numLabel, int cardNum, int sendPlayerNumber, int selBlockLoc) // 블록 클릭 이벤트
        {
            if (sendPlayerNumber != playerNumber && numLabel.Text.Contains("*")) // 만약 자기 블럭이 아니고 밝혀진게 아니라면
            {
                if(selectBlock != null)
                {
                    selectBlock.ForeColor = backupFontColor;
                    selectBlock.BackColor = backupFontBackgroundColor;
                }

                selectBlock = numLabel;
                backupFontColor = numLabel.ForeColor;
                backupFontBackgroundColor = numLabel.BackColor;

                numLabel.BackColor = Color.BlueViolet;
                targetPlayerNumber = sendPlayerNumber;
                this.selBlockLoc = selBlockLoc;
            }
        }

        private int GetBlockLocArr(int tarPlayerNum, int findNum)
        {
            int[] blocks = playerBlocks.playerBlocks[tarPlayerNum];
            for(int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == findNum)
                {
                    return i;
                }
            }
            return -1;
        }
        
        private void StartButDraw()
        {
            but_GameStart.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            but_GameStart.Location = new System.Drawing.Point(507, 278);
            but_GameStart.Size = new System.Drawing.Size(232, 79);
            but_GameStart.Text = "게임시작";
            but_GameStart.Visible = true;
            but_GameStart.Click += new System.EventHandler(this.but_GameStart_Click);
            Controls.Add(but_GameStart);
        }

        private void InGame_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }

    public class GameRule
    {
        public Point gameScreenSize = new Point(1280, 700);
        public Point bs = new Point(50, 100);
        public Point[] blockPos = new Point[4];

        public int[] block = new int[22]; // 12 이상부터는 하얀색의 블럭이다.
        public bool[] noSame = new bool[22];

        public GameRule()
        {
            blockPos[0] = new Point(20, 20); // 왼쪽 위
            blockPos[1] = new Point(gameScreenSize.X - (bs.X + 20), 20); // 오른쪽 위
            blockPos[2] = new Point(20, gameScreenSize.Y - (bs.Y + 20)); // 왼쪽 아래
            blockPos[3] = new Point(gameScreenSize.X - (bs.X + 20), gameScreenSize.Y - (bs.Y + 20)); // 왼쪽 아래
        }
    }

    public class PlayersBlockManage
    {
        public Dictionary<int, int[]> playerBlocks = new Dictionary<int, int[]>(); // key는 플레이어 value는 카드 넘버
        public Dictionary<int, Label[]> playerBlocksImage = new Dictionary<int, Label[]>();
    }
}
