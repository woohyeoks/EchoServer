using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

/**
 * Ver 1.0
 * Echo Server 
*/

namespace EchoServer_Ver1
{
    class Program
    {

        static Listener _listener = new Listener();

        static void OnAcceptCompleted()
        {

        }

        static void Main(string[] args)
        {
            // DNS (Domain Name System) 사용
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 식당 번호 / 입구 위치

            _listener.Init(endPoint);

            // 손님 한명만 받는건 아니니 무한 루프
            while (true)
            {
                Console.WriteLine("Listening...");

                // 손님을 입장시킨다.
                Socket clientSocket = _listener.Accept();
                Console.WriteLine("Accept...");

                // 손님 주문 받기!!!
                byte[] recvBuffer = new byte[1024];
                int recvBytes = clientSocket.Receive(recvBuffer);

                string recv_data = Encoding.UTF8.GetString(recvBuffer, 0, recvBytes);
                Console.WriteLine("RecvData " + recv_data);

                // 보낸다.
                byte[] sendBuffer = Encoding.UTF8.GetBytes("안녕 클라이언트야 !");
                clientSocket.Send(sendBuffer);
                clientSocket.Close();
            }

        }
    }
}
