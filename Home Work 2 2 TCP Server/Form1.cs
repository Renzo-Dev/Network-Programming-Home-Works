using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Home_Work_2_2_TCP_Server
{
    public partial class Form1 : Form
    {
        private IPEndPoint _localEndPoint;
        private Socket _server;

        public Form1()
        {
            InitializeComponent();
        }

        private async void bConnect_Click(object sender, EventArgs e)
        {
            if (_server != null)
            {
                return;
            }

            _localEndPoint = new IPEndPoint(IPAddress.Parse(tbHost.Text), int.Parse(tbPort.Text));

            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _server.Bind(_localEndPoint);
            _server.Listen(10);
            pictureBox1.BackColor = Color.Green;

            // получаем следующее подключение из очереди
            Socket client = null;

            await Task.Run(() => { client = _server.Accept(); });

            await Task.Run(() =>
            {
                while (client.Connected)
                {
                    StringBuilder builder = new StringBuilder();

                    int len = 0;

                    byte[] data = new byte[1024];
                    try
                    {
                        do
                        {
                            // считываем пакет в data
                            len = client.Receive(data); // узнаем размер пакета
                            builder.Append(Encoding.UTF8.GetString(data, 0, len));
                        } while (client.Available > 0);

                        if (builder.ToString() == "1")
                        {
                            DateTime date = DateTime.Now;
                            client.Send(Encoding.UTF8.GetBytes(date.ToLongDateString()));
                        }
                        else if (builder.ToString() == "2")
                        {
                            DateTime date = DateTime.Now;
                            client.Send(Encoding.UTF8.GetBytes(date.ToLongTimeString()));
                        }

                        builder.Clear();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            });
        }
    }
}