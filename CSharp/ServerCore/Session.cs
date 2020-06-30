using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
   public abstract class Session
    {
        Socket _socket;

        int _disconnected = 0;

        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        bool _pending = false;
        object _lock = new object();


        List<ArraySegment<byte>> _pendinglist = new List<ArraySegment<byte>>(); // 대기 중인 목록이다.
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();


        public abstract void OnConnected(EndPoint endPoint); // 접속 완료
        public abstract void OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);


        public void Start(Socket socket)
        {
            _socket = socket;
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);


            RegisterRecv(_recvArgs);
        }

        public void Send(byte[] sendBuffer)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuffer);
                if (_pendinglist.Count == 0)
                    RegisterSend();
            }
        }


        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            {
                return;
            }
            OnDisconnected(_socket.RemoteEndPoint);

            _socket.Shutdown(SocketShutdown.Both); // 우아하게 종료 시킨다.
            _socket.Close();
        }

        #region 네트워크 통신
        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if (pending == false)
            {
                OnRecvCompleted(null, args);
            }
        }

        void RegisterSend()
        {
            // c++ 은 포인터를 이용해서 넘겨주면 되는데...

            while (_sendQueue.Count > 0)
            {
                byte[] buff = _sendQueue.Dequeue();
                // ArraySegment 는 어떤 배열의 일부 구조체로 되어 있어, 스택에 할당된다.
                // 값이 복사되는 형태로 사용
                _pendinglist.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }
            _sendArgs.BufferList = _pendinglist;

            bool pendig = _socket.SendAsync(_sendArgs);
            if (pendig == false)
                OnSendCompleted(null, _sendArgs);

        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) // 0 바이트 오는 경우 상대방 연결 끊었을 때
            { 
                try
                {
                    OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));

                    RegisterRecv(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                // Disconnect();
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) // 0 바이트 오는 경우 상대방 연결 끊었을 때
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendinglist.Clear();
                        // Console.WriteLine($"Transferred bytes: {_sendArgs.BytesTransferred}");
                        OnSend(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count > 0)
                            RegisterSend();    
                        else
                            _pending = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }
        #endregion
    }
}
