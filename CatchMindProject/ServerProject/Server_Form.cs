﻿using System;
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
        List<Member_Info> arrMember = new List<Member_Info>();
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

        string winner_name = "";//게임종료후 승자 정보용 변수
        int winner_score = 0;


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
                        Message(join_data.nickname + "님이 로그인 시도");//입장했다는 메세지보다 더 명확한의미로 시도로 바꿈

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

                        Message(join_data.nickname + "님 로그인 성공");//위의 로그인 조건 확인 뒤 로그인 성공 띄움

                        Player_Info info = new Player_Info(); //Client_Info 멤버변수 초기화해서 배열에 넣는 과정
                        info.client = hClient;
                        if (count_client == 1) info.drawable = true; //최초 client에게 그리기 권한 - 클라이언트들이 중간에 나가지 않는다는 가정하에
                        else info.drawable = false;

                        bool is_exist=false;


                        Player_light_Info light_info = new Player_light_Info();//가벼운정보들 클라이언트에 전달용


                        for (int i =0; i < arrMember.Count; i++)
                        {
                            if(join_data.nickname == arrMember[i].nickname )//기존 존재하는 닉네임인 경우 저장되어있는 점수 불러옴
                            {
                                arrMember[i].img_num = join_data.img_num;//로그인할때 설정한 이미지 정보 회원정보에 저장
                                
                                
                                info.nickname = arrMember[i].nickname;
                                info.img_num = arrMember[i].img_num;
                                info.score = arrMember[i].score;//게임한판진행 중에 중간에 나갔다가 들어왔을때 점수 유지 

                                
                                arrClient.Add(info);//현재 플레이중 플레이어들 배열에 추가

                                is_exist = true;

                                light_info.score = arrMember[i].score;//판이 끝나기 전에 저장되어있던 점수
                                light_info.match = arrMember[i].match;
                                light_info.win = arrMember[i].win;
                                light_info.lose = arrMember[i].lose;
                                
                            }
                        }

                        if (!is_exist)//기존 존재하는 닉네임이 아닌경우 
                        {
                            Member_Info minfo = new Member_Info();
                            minfo.nickname = join_data.nickname;         
                            arrMember.Add(minfo);//회원정보리스트에 추가

                            info.nickname =join_data.nickname;
                            info.img_num = join_data.img_num;
                            arrClient.Add(info);//플레이어리스트에 추가

                            light_info.score = 0;//처음들어오는거면 점수 0
                            light_info.match = 0;
                            light_info.win = 0;
                            light_info.lose = 0;


                        }

 
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
            for(int i =0;i<arrClient.Count;i++)
            {
                if (arrClient[i].client == hClient)
                    arrClient[i].thread = t;//각 클라이언트랑 연결할때 실행되는 쓰레드를 클라이언트 정보에 저장 쓰레드 종료 회원나갈때 굳이 안해도되면 뺴기
            }
            t.Start();
            threadList.Add(t);
        }

        public bool checkName(string name) {
            for (int i = 0; i < arrClient.Count; i++) {
                if (name == arrClient[i].nickname) return false;
            }
            return true;
        }

        Chat_data chat_data;//이거 밖에있어야함?

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

                            string correct_player="";

                            bool end=false;
                            for (int i = 0; i < arrClient.Count; i++) {
                                if (client == arrClient[i].client) {
                                    arrClient[i].drawable = true;
                                    arrClient[i].score++;
                                    arrClientlight[i].score++;//중간에 나갔다 들어온사람한테 올바른 경기상태 전하려면 light정보 갱신
                                    correct_player = arrClient[i].nickname;//맞춘사람 이름
                                    if (arrClient[i].score == 3) end = true;
                                }
                                else arrClient[i].drawable = false;
                            }
                            foreach(Member_Info member in arrMember)
                            {
                                if(member.nickname == correct_player)
                                {
                                    member.score++;
                                }
                            }
                            if (end) { 
                                endGame();
                                Answer ans = new Answer();
                                ans.Type = (int)PacketType.다음정답;
                                ans.answer = answer;
                                Packet.Serialize(ans).CopyTo(sendBuffer, 0);
                                stream.Write(sendBuffer, 0, sendBuffer.Length);
                                stream.Flush();
                                Array.Clear(sendBuffer, 0, 1024 * 4);
                            }
                            else {
                                count_game++; //몇 판 했는지 처리
                                if (count_game == 5) endGame(); //5판끝났으면 endgame호출
                                else {
                                    getAnswer(); //answer갱신

                                    //정답자에게만 새로운 정답 알려줘야됨
                                    Answer ans = new Answer();
                                    ans.Type = (int)PacketType.다음정답;
                                    ans.answer = answer;
                                    Packet.Serialize(ans).CopyTo(sendBuffer, 0);
                                    stream.Write(sendBuffer, 0, sendBuffer.Length);
                                    stream.Flush();
                                    Array.Clear(sendBuffer, 0, 1024 * 4);//이건왜 sendToAll로 안함?
                                }
                            }
                        }
                    }
                    else if ((int)packet.Type == (int)PacketType.그림) {

                    }
                    else if ((int)packet.Type == (int)PacketType.나가기) {
                        //나가기 패킷 받았으면 처리
                        //arrclient에서 삭제하고 얘가 나갔다고 클라이언트들에게 알린다

                        Exit_data exit = (Exit_data)Packet.Desserialize(readBuffer);
                        Player_Info exit_member = new Player_Info();
                        exit_member.nickname = exit.nickname;

                        Message(exit_member.nickname + "님이 나갔습니다. ");
                        count_client--;//플레이어수 감소
                        sendToAll(exit, client);
                        int exit_index = arrClient.IndexOf(exit_member);//객체넣어서비교말고 string으로만 indexOf 쓰고 싶었는데 잘안됨
                        arrClient.RemoveAt(exit_index);
                        arrClientlight.RemoveAt(exit_index);


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


        void endGame() //게임 끝 - 강의자료의 serverstop 함수역할. 모두정리
        {
            //client한테 결과 보내고
            GameResult();
            End_Packet end = new End_Packet();
            end.Type = (int)PacketType.게임종료;
            end.winner_name = winner_name;
            end.winner_score = winner_score;
            sendToAll(end, null);
            
            //새 게임 준비
            count_game = 0;
            getAnswer();
        }

        void endServer() { //게임 아예 종료 - 언제? - 그냥 서버 창 닫을때만?
            try {
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


        void GameResult()
        {
            
            foreach(Player_Info player in arrClient)//이번판 승자 구하기
            {
                if(player.score>winner_score)
                {
                    winner_name = player.nickname;
                    winner_score = player.score;
                }
                player.score = 0; //초기화도 같이 해줌 - 클라이언트엔 반영이 안되는듯
            }

            foreach(Member_Info member in arrMember)//전적 저장,score정보는 0으로 초기화
            {
                member.score = 0;
                member.match += 1;

                if (member.nickname == winner_name)
                {
                    member.win += 1;
                  
                }
                else
                { 
                    member.lose += 1;
                }
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

