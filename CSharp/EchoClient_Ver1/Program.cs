﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EchoClient_Ver1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // DNS (Domain Name System)사용
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);// 식당 번호 / 입구 위치



            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);
            Console.WriteLine("Connect Success");



            byte[] sendBuffer = Encoding.UTF8.GetBytes("안녕 서버!");
            socket.Send(sendBuffer);
            Console.WriteLine("Send Success" );

            byte[] recvBuffer = new byte[1024];
            int bytes = socket.Receive(recvBuffer);

            string recvData = Encoding.UTF8.GetString(recvBuffer, 0, bytes);

            Console.WriteLine("Receive : " + recvData + " " + bytes);

        }
    }
}