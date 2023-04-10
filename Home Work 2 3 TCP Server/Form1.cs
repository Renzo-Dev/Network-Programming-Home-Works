using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Home_Work_2_3_TCP_Server
{
    public partial class Form1 : Form
    {
        private List<string> _phrases;
        private IPEndPoint localEndPoint;
        private Socket socket;

        public Form1()
        {
            InitializeComponent();
            _phrases = new List<string>()
            {
                "Привет",
                "А у вас как дела?",
                "Сколько сейчас времени?"
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            localEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.172"), int.Parse(tbPort.Text));
            // создаем локальный сокет для подключения на него
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // устанавливаем прослушивание
                socket.Bind(localEndPoint);
                socket.Listen(10);

                socket.BeginAccept(AcceptCallback, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Error:{ex.Message}");
            }

            pictureBox1.BackColor = Color.Green;
        }

        private async void AcceptCallback(IAsyncResult ar)
        {
            if (socket == null)
            {
                return;
            }

            // принимаем входящие подключения, и создаем новый Socket с ним
            Socket client = socket.EndAccept(ar);

            await Task.Run(() => { StartMessaging(client); });

            socket.BeginAccept(AcceptCallback, null);
        }

        private void StartMessaging(Socket client)
        {

            StringBuilder builder = new StringBuilder();

            int len = 0;
            byte[] data = new byte[1024];

            try
            {
                while (true)
                {
                    // получаем пакеты данных от клиента
                    do
                    {
                        len = client.Receive(data);
                        builder.Append(Encoding.UTF8.GetString(data, 0, len));
                    } while (client.Available > 0);

                    // Messages

                    if (builder.ToString() == "Привет")
                    {
                        client.Send(Encoding.UTF8.GetBytes(_phrases[new Random().Next(0, _phrases.Count)]));
                    }
                    else if (builder.ToString() == "Bye")
                    {
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        return;
                    }
                    else
                    {
                        client.Send(Encoding.UTF8.GetBytes(_phrases[new Random().Next(0, _phrases.Count)]));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Error:{ex.Message}");
            }
        }
    }
}