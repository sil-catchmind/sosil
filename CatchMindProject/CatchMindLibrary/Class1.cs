using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;//Client_Info.hclient 용
using System.Net.Sockets;//Client_Info.hclient 용
using System.IO;//Packet용
using System.Runtime.Serialization.Formatters.Binary;//Packet용


namespace CatchMindLibrary
{
    public enum PacketType //패킷타입 구별. receive함수에서 int로 변환해서 사용
    {
        채팅 = 0, //chat_data
        그림, //draw_data
        나가기, //exit_data
        들어오기, //join_data
        연결, //join_result
        정답, //correct_data
        게임종료, //end
        다음정답, //Answer
        참가

    }

    public class Player_Info
    {
        public string nickname;//사용자 이름
        public string pwd;//사용자 비밀번호
        public int img_num;//사용자 프로필 이미지 번호
        public bool drawable;//그리기 권한 여부
        public int score;//맞춘 개수 점수
        public TcpClient client;//해당 client 누군지 구분
    }

    [Serializable]
    public class Player_light_Info
    {
        public string nickname;
        public int img_num;
        public int score;
    }

    [Serializable]
    public class Packet//패킷 정의(강의예제 그대로함)
    {
        public int Length;
        public int Type;

        public Packet()
        {
            this.Length = 0;
            this.Type = 0;
        }

        public static byte[] Serialize(Object o)
        {
            MemoryStream ms = new MemoryStream(1024 * 4);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            return ms.ToArray();
        }

        public static Object Desserialize(byte[] bt)
        {
            MemoryStream ms = new MemoryStream(1024 * 4);
            foreach (byte b in bt)
            {
                ms.WriteByte(b);
            }

            ms.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();
            Object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }
    }

    [Serializable]
    public class Chat_data : Packet//채팅 데이터 패킷
    {
        public string nickname;
        public string data;
    }

    [Serializable]
    public class Draw_data : Packet//그림 데이터 패킷
    {


    }

    [Serializable]
    public class Exit_data : Packet//게임 떠날 때 보낼 데이터 패킷
    {
        public string nickname;
    }

    [Serializable]
    public class Join_data : Packet//게임 참가할 때 보낼 데이터 패킷
    {
        public string nickname;
        public string pwd;
        public int img_num;
    }

    [Serializable]
    public class Correct_data : Packet //누가 정답맞췄을 때 서버가 클라이언트들에게 보낼 패킷
    {
        public string nickname; //맞춘 애
    }

    [Serializable]
    public class Answer : Packet  // 다음 정답 전송 (정답자에게만)
    {
        public string answer; // 정답
    }


    [Serializable]
    public class Connection : Packet
    {
        public bool connection;
        //client의 connect함수에서 연결후에 connection패킷 받음
        //=>이게 true일때만 로그인창에서 게임창으로 넘어가도록?
    }

    [Serializable]
    public class End : Packet
    {
        //각 클라이언트들의 점수 정보 전달?
    }

    [Serializable]
    public class Join_result : Packet
    {
        public bool success;
        //client의 connect함수에서 연결후에 join_result패킷 받음
        //=>이게 true일때만 로그인창에서 게임창으로 넘어가도록?
        public List<Player_light_Info> arrClient;//조인리절트보낼때 클라이언트들의 가벼운정보 보냄
        public bool it; //술래인지 아닌지 true이면 자기가 술래임
        public string first_answer; //최초 정답
    }

    [Serializable]
    public class New_Player: Packet
    {
        public Player_light_Info ligthInfo;
    }


}
