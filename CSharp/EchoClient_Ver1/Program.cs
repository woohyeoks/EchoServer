using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EchoClient_Ver1
{

    public class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine("접속 완료");
        }
        public override void OnRecv(ArraySegment<byte> buffer)
        {
            Console.WriteLine("recv 완료");
        }
        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine("send 완료");
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine("OnDisconnected 완료");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // DNS (Domain Name System)사용
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);// 식당 번호 / 입구 위치


            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return new ServerSession(); });

            while (true)
            {
                try
                {

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(100);
            }

           /* for (int i = 0; i < 10; ++i)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
                Console.WriteLine("Connect Success");


                byte[] sendBuffer = Encoding.UTF8.GetBytes("안녕 서버!");
                socket.Send(sendBuffer);
                Console.WriteLine("Send Success");

                byte[] recvBuffer = new byte[1024];
                int bytes = socket.Receive(recvBuffer);

                string recvData = Encoding.UTF8.GetString(recvBuffer, 0, bytes);

                Console.WriteLine("Receive : " + recvData + " " + bytes);
                socket.Close();
            }*/

        }
    }
}
