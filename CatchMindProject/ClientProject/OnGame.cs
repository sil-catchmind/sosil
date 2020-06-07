using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CatchMindLibrary;//라이브러리 추가
using System.Net.Sockets;//IPAddress변수때문에 넣음
using System.Net;//IPAddress변수때문에 넣음
using System.Threading;
using System.IO;

namespace ClientProject
{
    public partial class formOnGame : Form///쓰레드로 리시브 하는거 해야함 아직안함
    {
        formStartGame formstart;

        public formOnGame()
        {
            InitializeComponent();
        }

        public formOnGame(formStartGame formstart)
        {
            InitializeComponent();
            this.formstart = formstart;
        }

       

        TcpClient m_Client;
        bool m_bConnect = false;
        const int PORT = 8000;//포트값
        IPAddress ipAddr = IPAddress.Parse("127.0.0.1");//아이피 주소값 저장
        NetworkStream m_stream;
        private byte[] sendBuffer = new byte[1024 * 4];
        private byte[] readBuffer = new byte[1024 * 4];
        Thread m_ThReader;


       

        Player_light_Info me = new Player_light_Info();
        string my_nickname;
        int my_place = 0;



        public AppDomainInitializer m_initializeClass;//없애도댈듯
        public Join_data m_loginClass;//없애도될듯

        const int PLAYERNUMBER = 4;//경기 플레이어 수
        List<Player_light_Info> arrClient = new List<Player_light_Info>();





        public PictureBox[] pic = new PictureBox[4];
        public Label[] lblName = new Label[4];
        public Label[] lblScore = new Label[4];
        public Label[] lblHistory = new Label[4];
        public GroupBox[] gbProfile = new GroupBox[4];


        private void formOnGame_Load(object sender, EventArgs e)
        {//스트림 어케써야되나 부모스트림 그대로쓰나


            m_stream = formstart.m_stream;
            m_Client = formstart.m_Client;

            //d왜 바깥으로 못내보내지? 
            me.nickname = formstart.my_nickname;

            arrClient = formstart.arrClient;//플레이어들 정보 얻어옴

           

           

           

            Message("게임에 참여하였습니다");

            pic[0] = pictureBox1;
            pic[1] = pictureBox2;
            pic[2] = pictureBox3;
            pic[3] = pictureBox4;

            
            lblName[0] = lblPlayer1Name;
            lblName[1] = lblPlayer2Name;
            lblName[2] = lblPlayer3Name;
            lblName[3] = lblPlayer4Name;
          
            
            lblScore[0] = lblPlayer1Score;
            lblScore[1] = lblPlayer2Score;
            lblScore[2] = lblPlayer3Score;
            lblScore[3] = lblPlayer4Score;

            lblHistory[0] = lblHistory1;
            lblHistory[1] = lblHistory2;
            lblHistory[2] = lblHistory3;
            lblHistory[3] = lblHistory4;

            gbProfile[0] = groupBox1;
            gbProfile[1] = groupBox2;
            gbProfile[2] = groupBox3;
            gbProfile[3] = groupBox4;

            LoadScreen();



            m_ThReader = new Thread(new ThreadStart(Receive)); //접속 받아들이는 thread 시작
            m_ThReader.Start();

            
        }

     
       

        void Receive()
        {
            int nRead = 0;//이거몬지 선영이한테 물어보기
            try
            {
                m_bConnect = true;
                while (m_bConnect && m_stream != null)
                {
                    Array.Clear(readBuffer, 0, 1024 * 4);
                    if (m_Client.Connected)
                    {
                        try
                        {
                            nRead = m_stream.Read(readBuffer, 0, 1024 * 4);
                        }
                        catch
                        {
                            Message("error at Receive -read");
                            m_stream.Close();
                        }

                        Packet packet = (Packet)Packet.Desserialize(readBuffer);

                        if ((int)packet.Type == (int)PacketType.채팅)
                        {
                            Chat_data chat = (Chat_data)Packet.Desserialize(readBuffer);
                            Message(chat.nickname + ":" + chat.data);
                        }
                        else if ((int)packet.Type == (int)PacketType.그림)
                        {
                            //그림일 때 추가
                        }
                        else if ((int)packet.Type == (int)PacketType.정답)
                        {
                            Correct_data correct = (Correct_data)Packet.Desserialize(readBuffer);
                            if (correct.nickname == me.nickname)
                            {
                                Message("정답입니다");//본인이 맞췄을 때는 정답입니다로 출력
                            }
                            else
                            {
                                Message(correct.nickname + "님이 정답을 맞추셨습니다."); //messagebox할라다 별로길래 냅둠
                                Message(correct.nickname + "님이 그리는 중입니다");
                            }
                            lblKeyword.Text = "";
                            //클라이언트가 내부적으로 걔점수라벨만 바꾸는게 낫나???아니면 서버가 플레이어정보배열 보내서 일괄 관리!?
                            for (int i = 0; i < arrClient.Count; i++)
                            {
                                if (arrClient[i].nickname == correct.nickname)
                                {
                                    arrClient[i].score += 1;
                                    lblScore[i].Text = arrClient[i].score.ToString();
                                }
                            }
                        }
                        else if ((int)packet.Type == (int)PacketType.다음정답)
                        {
                            Answer ans = (Answer)Packet.Desserialize(readBuffer);
                            Message("제시어는 " + ans.answer + "입니다");
                            Message("제시어를 그려주세요");
                            lblKeyword.Text = ans.answer;//퇴장 재참가할때 이거띄 우는거에는 상관없는지 고민

                        }

                        else if ((int)packet.Type == (int)PacketType.참가)
                        {
                            New_Player new_player = (New_Player)Packet.Desserialize(readBuffer);

                            Message(new_player.ligthInfo.nickname + "님이 입장하셨습니다");
                            //Player_light_Info light_info = new Player_light_Info();//가벼운정보들 저장용 

                            arrClient.Add(new_player.ligthInfo);
                   

                            LoadScreen();
                        }

                        else if ((int)packet.Type == (int)PacketType.게임종료)
                        {
                            End_Packet end = (End_Packet)Packet.Desserialize(readBuffer);
                            Message(end.winner_name + "님이 " + end.winner_score + "로 승리하셨습니다");


                        }

                        else if ((int)packet.Type == (int)PacketType.나가기)
                        {
                            Exit_data exit = (Exit_data)Packet.Desserialize(readBuffer);
                            Player_light_Info exit_member = new Player_light_Info();
                            exit_member.nickname = exit.nickname;
                            Message(exit_member.nickname + "님이 나갔습니다");
                            arrClient.Remove(exit_member);
                            LoadScreen();


                        }
                       
                    }
                }
            }
            catch
            {
                Message("client : error at Receive");
            }

            
        }

        void LoadScreen()//화면 로딩
        {
            my_place = arrClient.IndexOf(me);

            for (int i =0;i<4;i++)
            { 

            pic[i].Visible = false;
            lblName[i].Text = "";
            //점수설정
            lblScore[i].Text = "";
            lblHistory[i].Text = "";
                gbProfile[i].BackColor = Color.White;
            }


            for (int i = 0; i < arrClient.Count; i++)
            {
                if(i==my_place)
                {
                    gbProfile[i].BackColor = Color.LemonChiffon;

                }

                   // 플레이어들 이미지 배치     
                    if (arrClient[i].img_num == 1)
                    { pic[i].Image = Properties.Resources.무지; }
                    else if (arrClient[i].img_num == 2)
                    { pic[i].Image = Properties.Resources.네오; }
                    else if (arrClient[i].img_num == 3)
                    { pic[i].Image = Properties.Resources.어피치; }
                    else if (arrClient[i].img_num == 4)
                    { pic[i].Image = Properties.Resources.라이언; }

                
                    //이름설정
                    lblName[i].Text = arrClient[i].nickname;
                    //점수설정
                    lblScore[i].Text = Convert.ToString(arrClient[i].score);
                    //그림 보이게
                    string match = Convert.ToString(arrClient[i].match);
                    string win= Convert.ToString(arrClient[i].win);
                    string lose = Convert.ToString(arrClient[i].lose);
                    //전적 설정
                    lblHistory[i].Text = (match + "전 " + win + "승 " + lose + "패");

                    pic[i].Visible = true;



                
             
            }







        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Send_Chat();

        }
        private void txtSendline_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Send_Chat();
        }

        void Send_Chat()
        {
            Message(me.nickname + "(ME) : " + txtSendline.Text);
            try
            {
                if (m_Client.Connected)
                {
                    Chat_data chat = new Chat_data();
                    chat.Type = (int)PacketType.채팅;
                    chat.nickname = me.nickname;
                    chat.data = txtSendline.Text;

                    Packet.Serialize(chat).CopyTo(this.sendBuffer, 0);//메세지 패킷 보내기
                    Send();

                    txtSendline.Text = "";

                }
            }
            catch
            {
                Message("채팅데이터 전송 실패");
            }

        }


        void Send()
        {
            this.m_stream.Write(this.sendBuffer, 0, this.sendBuffer.Length);
            this.m_stream.Flush();

            for (int i = 0; i < 1024 * 4; i++)
            {
                this.sendBuffer[i] = 0;
            }
        }

        void Disconnect()
        {
            Exit_data exit = new Exit_data();
            exit.Type = (int)PacketType.나가기;
            exit.nickname = me.nickname;
            Packet.Serialize(exit).CopyTo(this.sendBuffer, 0);//exit 패킷 보내기
            Send();



         
        }
        
        public void Message(string msg)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                txtAll.AppendText(msg + "\r\n");
                txtAll.Focus();
                txtAll.ScrollToCaret();
                txtSendline.Focus();
            }));
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Disconnect();
            //스트림 닫기, 쓰레드종료 
           Application.Exit();//종료 함수 적절한지 확인더 필요
            
           
        }

        private void formOnGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit(); 
        }

        
    }
}
