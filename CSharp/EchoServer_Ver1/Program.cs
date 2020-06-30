using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/**
 * Ver 1.0
 * Echo Server 
*/
namespace EchoServer_Ver1
{
    class Program
    {

        class GameSession : Session
        {
            // 엔진과 컨텐츠 분리
            public override void OnConnected(EndPoint endPoint)
            {
                byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                Send(sendBuffer);

                Thread.Sleep(1000);
                Disconnect();
            }

            public override void OnDisconnected(EndPoint endPoint)
            {
                Console.WriteLine($"OnDisConnected : {endPoint}");
            }

            public override void OnRecv(ArraySegment<byte> buffer)
            {
                string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                Console.WriteLine($"[From Client] {recvData}");
            }

            public override void OnSend(int numOfBytes)
            {
                Console.WriteLine($"Transferred bytes: {numOfBytes}");
            }

        }

        static Listener _listener = new Listener();
  
        static void Main(string[] args)
        {
            // DNS (Domain Name System) 사용
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 식당 번호 / 입구 위치

            _listener.Init(endPoint, () => { return new GameSession(); });

            Console.WriteLine("Listener ...");
            // 손님 한명만 받는건 아니니 무한 루프
            while (true)
            {
            }

        }
    }
}
