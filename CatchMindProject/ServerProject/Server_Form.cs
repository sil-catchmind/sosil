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

//4인, 5판3선승 기준

namespace ServerProject {
    public partial class Server_Form : Form {
        const int PORT = 8000;//포트값
        IPAddress ipAddr = IPAddress.Parse("127.0.0.1");//아이피 주소값 저장
        Thread m_thServer;
        TcpListener m_listener;
        TcpClient hClient;
        NetworkStream m_stream;

        //필요한 배열들
        List<Player_Info> arrClient = new List<Player_Info>();
        List<Player_light_Info> arrClientlight = new List<Player_light_Info>();
        List<Thread> threadList = new List<Thread>();
        List<NetworkStream> streamList = new List<NetworkStream>();//안쓰면 나중에 삭제

        //게임진행 관련변수
        int count_client = 0;//현재 클라이언트 수
        int count_game = 0;//진행된 게임 수
        bool correct = false;//정답 여부 
        string answer = "Test";  //(임시) 정답 


        byte[] readBuffer = new byte[1024 * 4];
        byte[] sendBuffer = new byte[1024 * 4];


        public Server_Form() {
            InitializeComponent();

        }

        private void Server_Form_Load(object sender, EventArgs e) { //아니면 버튼 눌렀을때?

        }


        private void button1_Click(object sender, EventArgs e) {
            m_thServer = new Thread(new ThreadStart(receive_client)); //접속 받아들이는 thread 시작
            m_thServer.Start();
        }

        bool m_bStop = false;

        void receive_client()//클라이언트 접속 받아들임
        {
            try {
                m_listener = new TcpListener(ipAddr, PORT);
                m_listener.Start();
                Message("서버시작. 클라이언트 접속 대기 중");
                m_bStop = true;
                while (m_bStop) {
                    hClient = m_listener.AcceptTcpClient();
                    if (hClient.Connected) {

                        //여기서 join data 패킷 받고
                        m_stream = hClient.GetStream();
                        m_stream.Read(readBuffer, 0, 1024 * 4);
                        //Packet packet = (Packet)Packet.Desserialize(readBuffer);

                        Join_data join_data = (Join_data)Packet.Desserialize(readBuffer);
                        Message(join_data.nickname + "님이 입장했습니다.");

                        Join_result join_result = new Join_result();
                        join_result.Type = (int)PacketType.연결;

                        //name 중복검사
                        if (!checkName(join_data.nickname)) // => 클라이언트에게 name중복임을 알리고 처리하도록 (연결안됨-게임폼으로 못넘어가게)
                        {
                            //중복이면 연결해제하고 count--하고 관련된 것들 처리 필요
                            //connection=false전달
                            Message("이름 중복 발생. client연결 해제함");
                            join_result.success = false;
                            join_result.reason = "이름중복";
                            Packet.Serialize(join_result).CopyTo(sendBuffer, 0);
                            Send();
                            m_stream.Close();
                            Array.Clear(sendBuffer, 0, 1024 * 4);
                            hClient.Close();
                            continue;
                        }

                        //4명이상인지 검사
                        if (count_client < 4) count_client++;
                        else {
                            //클라이언트에게 연결실패 전달
                            Message("4명 넘음. 클라이언트 연결 해제");
                            join_result.success = false;
                            join_result.reason = "인원초과";
                            Packet.Serialize(join_result).CopyTo(sendBuffer, 0);
                            Send();
                            m_stream.Close();
                            Array.Clear(sendBuffer, 0, 1024 * 4);
                            hClient.Close();
                            continue;
                        }

                        Player_Info info = new Player_Info(); //Client_Info 멤버변수 초기화해서 배열에 넣는 과정
                        info.client = hClient;
                        if (count_client == 1) info.drawable = true; //최초 client에게 그리기 권한 - 클라이언트들이 중간에 나가지 않는다는 가정하에
                        else info.drawable = false;
                        info.score = 0;
                        info.nickname = join_data.nickname;
                        info.img_num = join_data.img_num;
                        info.pwd = join_data.pwd;

                        arrClient.Add(info);


                        Player_light_Info light_info = new Player_light_Info();//가벼운정보들 저장용 
                        light_info.score = 0;
                        light_info.nickname = join_data.nickname;
                        light_info.img_num = join_data.img_num;

                        arrClientlight.Add(light_info);

                        //연결문제없음. 성공 전달///////////////////////연결성공 전달할때 현재 플레이어 상태도 보내야할듯. 사진이랑 닉네임정보
                        join_result.success = true;
                        join_result.arrClient = arrClientlight.ToList();
                        if (count_client == 1) { join_result.it = true; join_result.first_answer = answer; }
                        else { join_result.it = false; join_result.first_answer = ""; }
                        Packet.Serialize(join_result).CopyTo(sendBuffer, 0);
                        Send();

                        New_Player player = new New_Player();
                        player.Type = (int)PacketType.참가;
                        player.ligthInfo = light_info;
                        sendToAll(player, hClient);


                        Client_handler();
                    }
                }
            }
            catch {
                Message("error at receive_client");
            }
        }


        void Client_handler() {
            Thread t = new Thread(new ThreadStart(receiveData));
            t.Start();
            threadList.Add(t);
        }

        public bool checkName(string name) {
            for (int i = 0; i < arrClient.Count; i++) {
                if (name == arrClient[i].nickname) return false;
            }
            return true;
        }

        Chat_data chat_data;

        void receiveData() {
            TcpClient client = hClient;
            bool m_bConnect;
            Array.Clear(sendBuffer, 0, 1024 * 4);

            NetworkStream stream = client.GetStream();
            streamList.Add(stream);
            if (stream != null) {
                m_bConnect = true;
                while (m_bConnect) {
                    try {
                        Array.Clear(readBuffer, 0, 1024 * 4);
                        stream.Read(readBuffer, 0, 1024 * 4);
                    }
                    catch {
                        Message("error발생"); return;
                        //stream.Close(); //need check
                    }

                    Packet packet = (Packet)Packet.Desserialize(readBuffer);

                    if ((int)packet.Type == (int)PacketType.채팅) {
                        chat_data = (Chat_data)Packet.Desserialize(readBuffer);
                        Message(chat_data.nickname + " : " + chat_data.data);//for test
                        sendToAll(packet, client);

                        if (answer == chat_data.data) //정답 처리하는 부분
                        {
                            Correct_data correct_data = new Correct_data(); //클라이언트들에게 correct발생했다고 알림(correct패킷전송)
                            correct_data.Type = (int)PacketType.정답;
                            correct_data.nickname = getName(client); //맞춘 애 이름 담음 - client에서 **님이 정답을 맞췄습니다. 라고 출력
                            sendToAll(correct_data, null); //서버가 보낼 땐 null로... need check 

                            for (int i = 0; i < arrClient.Count; i++) {
                                if (client == arrClient[i].client) {
                                    arrClient[i].drawable = true;
                                    arrClient[i].score++;
                                    if (arrClient[i].score == 3) { endGame(); break; }
                                }
                                else arrClient[i].drawable = false;
                            }

                            count_game++; //몇 판 했는지 처리
                            if (count_game == 6) endGame(); //5판끝났으면 endgame호출

                            getAnswer(); //answer갱신

                            //정답자에게만 새로운 정답 알려줘야됨
                            Answer ans = new Answer();
                            ans.Type = (int)PacketType.다음정답;
                            ans.answer = answer;
                            Packet.Serialize(ans).CopyTo(sendBuffer, 0);
                            stream.Write(sendBuffer, 0, sendBuffer.Length);
                            stream.Flush();
                            Array.Clear(sendBuffer, 0, 1024 * 4);
                        }
                    }
                    else if ((int)packet.Type == (int)PacketType.그림) {

                    }
                    else if ((int)packet.Type == (int)PacketType.나가기) { //나가기 패킷 받았으면 처리 - 일단 이러한 경우 없다고 가정
                                                                        //arrclient에서 삭제하고 얘가 나갔다고 클라이언트들에게 알린다
                    }

                }
                //stream.Close(); //need check
            }

        }

        string getName(TcpClient client) { //client이름 뭔지 찾아서 반환
            for (int i = 0; i < arrClient.Count; i++) {
                if (arrClient[i].client == client)
                    return arrClient[i].nickname;
            }
            return null;
        }


        /*void startGame()//게임 시작
         {
             //5판이하이고 맞추지 못한 동안 while

             //함수로 따로 처리할 게 아니라 다른 함수 곳곳에서 변수로 제어해야할듯..?
             //=>receive client에서 제어해야하나? count == 5이면 게임 끝내기
             //receive client아니고 count증가시킬때마다 검사해서 5이면 endGame호출 - 클라이언트에 게임끝났다고 알리고, 클라이언트들, 스트림들 싹다 정리
             //=>receive client알아서 종료될듯 m_bStop통해서..
         }*/

        void endGame() //게임 끝 - 강의자료의 serverstop 함수역할. 모두정리
        {
            try {
                Chat_data chat = new Chat_data();
                chat.data = "게임이 종료됩니다."; //승리자도 위에서 계산해서 출력하는 거 추가필요
                chat.nickname = "server";
                sendToAll(chat, null);

                for (int i = 0; i < threadList.Count; i++) {
                    if (threadList[i].IsAlive) threadList[i].Abort();
                }
                m_listener.Stop();
                m_thServer.Abort();
            }
            catch {
                Message("error at endGame");
            }
        }

        void sendToAll(Packet packet, TcpClient client)//인자로 받은 패킷을 모든클라이언트에게 전송
        {
            //인자로 받은 client(=보낸 애) 제외
            try {
                Packet.Serialize(packet).CopyTo(sendBuffer, 0);
                for (int i = 0; i < arrClient.Count; i++) {
                    if (client == arrClient[i].client) continue;
                    NetworkStream stream = arrClient[i].client.GetStream();
                    stream.Write(this.sendBuffer, 0, this.sendBuffer.Length);
                    stream.Flush();
                }
                for (int i = 0; i < 1024 * 4; i++) {
                    this.sendBuffer[i] = 0;
                }
            }
            catch {
                Message("error at sendToAll");
            }
        }

        public void Send() {
            if (m_stream != null) {
                m_stream.Write(this.sendBuffer, 0, this.sendBuffer.Length);
                m_stream.Flush();
            }
            for (int i = 0; i < 1024 * 4; i++) {
                this.sendBuffer[i] = 0;
            }
        }

        void getAnswer()//새로운 정답 가져오기
        {
            answer = "Test" + count_game; //임시

            //txt file에서 readline해서 answer변수 갱신
            //랜덤하게 얻어올 수 있는 방법으로 개선 필요 
        }

        public void Message(string msg)//확인용
        {
            this.Invoke(new MethodInvoker(delegate () {
                txt_all.AppendText(msg + "\r\n");
                txt_all.Focus();
                txt_all.ScrollToCaret();
            }));
        }


    }
}

