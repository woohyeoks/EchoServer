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
        static Listener _listener = new Listener();
        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                Session session = new Session();
                session.Start(clientSocket);


                byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                session.Send(sendBuffer);

                Thread.Sleep(1000);
                session.Disconnect();
                session.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            // DNS (Domain Name System) 사용
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 식당 번호 / 입구 위치

            _listener.Init(endPoint , OnAcceptHandler);

            Console.WriteLine("Listener ...");
            // 손님 한명만 받는건 아니니 무한 루프
            while (true)
            {
            }

        }
    }
}
