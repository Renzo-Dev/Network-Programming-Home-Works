using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Home_Work_1_Server
{

    internal class Server
    {
        private IPEndPoint _serverEndPoint = new IPEndPoint(IPAddress.Any, 8888);
        Socket socket = null;

        Server()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(_serverEndPoint);
            socket.Listen(1000);
        }

        static void Main(string[] args)
        {
            Server server = new Server();

        }
    }
}