using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Home_Work_2_2_TCP_Client
{
    public partial class Form1 : Form
    {
        private IPEndPoint _remoEndPoint;
        private Socket _clientSocket;

        public Form1()
        {
            InitializeComponent();
        }

        private async void bConnect_Click(object sender, EventArgs e)
        {
            if (_clientSocket != null)
            {
                return;
            }

            _remoEndPoint = new IPEndPoint(IPAddress.Parse(tbHost.Text), int.Parse(tbPort.Text));
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await _clientSocket.ConnectAsync(_remoEndPoint);

                if (_clientSocket.Connected)
                {
                    pictureBox1.BackColor = Color.Green;
                    bGetDate.Enabled = true;
                    bGetTime.Enabled = true;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private async void bGetDate_Click(object sender, EventArgs e)
        {
            _clientSocket.Send(Encoding.UTF8.GetBytes("1"));
            await ReaderNetworkStream(new NetworkStream(_clientSocket));
        }

        private async void bGetTime_Click(object sender, EventArgs e)
        {
            _clientSocket.Send(Encoding.UTF8.GetBytes("2"));
            await ReaderNetworkStream(new NetworkStream(_clientSocket));
        }

        public Task ReaderNetworkStream(NetworkStream stream)
        {
            return Task.Run(() =>
            {
                int len = 0;
                StringBuilder builder = new StringBuilder();
                byte[] data = new byte[1024];

                do
                {
                    len = stream.Read(data, 0, 1024);
                    builder.Append(Encoding.UTF8.GetString(data, 0, len));
                } while (stream.DataAvailable);

                if (tbMessage.InvokeRequired)
                {
                    tbMessage.Invoke(new Action(() => { tbMessage.Text = builder.ToString(); }));
                }

                builder.Clear();
            });
        }
    }
}