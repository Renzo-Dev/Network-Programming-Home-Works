using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Home_Work_1_TCP_Server
{
    class Server
    {
        // Устанавливаем для Socket локальную конечную точку
        private IPEndPoint _localEndPoint = null;

        public Server()
        {
            _localEndPoint = new IPEndPoint(IPAddress.Any, 7777);
        }

        public void Run()
        {
            // создаем Socket TCP/IP
            Console.WriteLine("Запуск сервера...");

            // Socket сервера, с которым связывается клиент
            Socket sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // назначаем Socket локальной конечной точки и 5слушаем входящие Sockets

            try
            {
                sListener.Bind(_localEndPoint);
                sListener.Listen(10);
                Console.WriteLine("Ожидаем соединение через порт {0}", _localEndPoint);
                // начинаем слушать соединения
                while (true)
                {
                    // Программа приостанавливается, ожидая входящие соединения
                    Socket client = sListener.Accept();
                    Console.WriteLine("Подключился :{0}", client.RemoteEndPoint);

                    StringBuilder data = new StringBuilder();

                    do
                    {
                        int len; // размер пакета в байтах
                        byte[] bytes = new byte[1024]; // буфер для приема пакета

                        // получаем пакеты, пока Available > 0
                        do
                        {
                            len = client.Receive(bytes); // получаем размер пакета в байтах
                            data.Append(Encoding.UTF8.GetString(bytes, 0,
                                len)); // записываем , форматируем пакет в BuilderString
                        } while (client.Available > 0);

                        Console.WriteLine("Полученное сообщение: {0}", data);
                        if (data.ToString() == "/help")
                        {
                            client.Send(Encoding.UTF8.GetBytes($"Список команд:\n" +
                                                               $"/time получить время ( локальное на сервере )" +
                                                               $"\n/data получить дату ( локальную дату сервера )" +
                                                               $"\n/restart ( перезапустить сервер )" +
                                                               $"\n/exit ( выйти из программы )"));
                        }
                        else if (data.ToString() == "/restart")
                        {
                            client.Send(Encoding.UTF8.GetBytes("...Server Restarting..."));
                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                            sListener.Close();
                            Run();
                        }
                        else if (data.ToString() == "/time")
                        {
                            DateTime time = DateTime.Now;
                            client.Send(Encoding.UTF8.GetBytes(time.ToLongTimeString()));
                        }
                        else if (data.ToString() == "/data")
                        {
                            DateTime time = DateTime.Now;
                            client.Send(Encoding.UTF8.GetBytes(time.ToLongDateString()));
                        }
                        else if (data.ToString() == "/exit")
                        {
                            Console.WriteLine("Соединение с {0} разорвано", client.RemoteEndPoint);
                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                            break;
                        }
                        else
                        {
                            client.Send(
                                Encoding.UTF8.GetBytes($"Неизвестная команда. Просмотреть команды введите /help"));
                        }

                        data.Clear();
                    } while (client.Connected);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Run();
        }
    }
}