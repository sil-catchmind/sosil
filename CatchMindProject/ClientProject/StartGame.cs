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
    public partial class formStartGame : Form
    {
        public formStartGame()
        {
            InitializeComponent();
        }
         
        public TcpClient m_Client;
        bool m_bConnect = false;
        const int PORT = 8000;//포트값
        IPAddress ipAddr=IPAddress.Parse("127.0.0.1");//아이피 주소값 저장
        public NetworkStream m_stream;
        private byte[] sendBuffer = new byte[1024 * 4];
        private byte[] readBuffer = new byte[1024 * 4];
        Thread m_ThReader;
        bool it; //본인이 술래인지 아닌지 지정
        public string my_nickname;
        public List<Player_light_Info> arrClient = new List<Player_light_Info>();

        
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (txtNickname.Text == "")
            {
                lblAlert.Text = "닉네임을 입력하세요.";
                return;
            }

          
            else if ((!rdoImage1.Checked)&&(!rdoImage2.Checked) && (!rdoImage3.Checked) && (!rdoImage4.Checked))
            {
                lblAlert.Text = "이미지를 선택하세요.";
                return;
            }
            login();

        }
        void login()//서버에게 로그인 정보 패킷 전달 후 승인 패킷 받음
        {
            this.m_Client = new TcpClient();

            //서버에 접속
            try
            {
                this.m_Client.Connect(ipAddr, PORT);
            }
            catch
            {
                MessageBox.Show("접속 에러");
                m_bConnect = false;
                return;
            }

            this.m_bConnect = true;//서버에 연결되었습니다 메세지띄우기 굳이 해야되나?
            this.m_stream = this.m_Client.GetStream();

            //로그인 정보 전송
            if (!this.m_bConnect)
            {
                return;
            }

            Join_data join = new Join_data();
            join.Type = (int)PacketType.연결;
            join.nickname = txtNickname.Text;
            

            //이미지 번호 설정
            if (rdoImage1.Checked == true)
            {
                join.img_num = 1;
            }
            else if (rdoImage2.Checked == true)
            {
                join.img_num = 2;
            }
            else if (rdoImage3.Checked == true)
            {
                join.img_num = 3;
            }
            else if (rdoImage4.Checked == true)
            {
                join.img_num = 4;
            }


            Packet.Serialize(join).CopyTo(this.sendBuffer, 0);
            Send();


            m_stream.Read(readBuffer, 0, 1024 * 4);//이거 받는거는 쓰레드 안필요???

            Join_result joinResult = (Join_result)Packet.Desserialize(readBuffer);

            

            if (!joinResult.success)//승인되지않을때
            {
                //lblAlert.Text = "닉네임과 비밀번호를 다시한번 확인해주세요";
                //사람다찼을때도 추가해야할듯

                if (joinResult.reason == "인원초과")
                    MessageBox.Show("인원이 초과되어 입장할 수 없습니다");
                else if (joinResult.reason == "이름중복")
                    MessageBox.Show("이미 사용중인 닉네임입니다");
            }
            else//승인되면 게임폼열음
            {
                arrClient = joinResult.arrClient;//플레이어들 정보 얻음
                my_nickname = txtNickname.Text;//이렇게 해도 되려나
                formOnGame ongame = new formOnGame(this);

                if (joinResult.it)
                {
                    it = true;
                    ongame.lblKeyword.Text = joinResult.first_answer;
                    //술래 채팅창, 버튼 제거? or enabled=false
                    this.Invoke(new MethodInvoker(delegate () {
                            ongame.txtSendline.Visible = false;
                            ongame.btnSend.Visible = false;
                    }));
                }


                ongame.Show();//게임 폼 열기
                ongame.FormClosed += new FormClosedEventHandler(game_close);
                this.Visible = false;
            }

            

        }

        public void game_close(object sender, FormClosedEventArgs e)
        {
            if (this.Visible == false)
            {
                this.Visible = true;
                this.Close();
            }
        }

        public void Send()
        {
            if(m_stream != null) { 
            this.m_stream.Write(this.sendBuffer, 0, this.sendBuffer.Length);
            this.m_stream.Flush();
            }

            for ( int i = 0; i<1024*4;i++)
            {
                this.sendBuffer[i] = 0;
            }

        }

        

        private void formStartGame_FormClosed(object sender, FormClosedEventArgs e)//////////이걸 이폼말고 다른게 닫힐떄 해야되나
        {
            /* this.m_client.Close();
             this.m_networkstream.Close();
             */
            
        }
    }
}
    


