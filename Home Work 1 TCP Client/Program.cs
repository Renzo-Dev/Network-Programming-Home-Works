using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Home_Work_1_TCP_Client
{
    internal class Client
    {
        private IPEndPoint _ipEndPoint = null;

        public Client()
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Parse("5.227.27.199"), 7777);
        }

        public void Connect()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            while (true)
            {
                // Соединяем Socket с удаленной точкой
                socket.Connect(_ipEndPoint);
                Console.Clear();
                Console.WriteLine("Введите сообщение: ");
                do
                {
                    string message = Console.ReadLine();

                    byte[] msg = Encoding.UTF8.GetBytes(message);

                    // отправляем пакет байтов ( нашу строку )
                    socket.Send(msg);

                    if (message == "/exit")
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                        return;
                    }

                    msg = new byte[1024];
                    int len = 0;
                    StringBuilder data = new StringBuilder();

                    // получаем ответ от сервера
                    do
                    {
                        len = socket.Receive(msg);
                        data.Append(Encoding.UTF8.GetString(msg, 0, len));
                    } while (socket.Available > 0);

                    Console.Clear();
                    Console.WriteLine(data);
                    if (data.ToString() == "...Server Restarting...")
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                        Connect();
                    }

                    Console.WriteLine("Введите сообщение: ");
                    data.Clear();
                } while (socket.Connected);

                Thread.Sleep(500);
            }
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Client client = new Client();
            client.Connect();
        }
    }
}