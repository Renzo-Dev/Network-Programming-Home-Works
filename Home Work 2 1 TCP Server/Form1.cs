using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Home_Work_2_1_TCP_Server
{
    public partial class Form1 : Form
    {
        private IPEndPoint localEndPoint;
        private Socket socket;

        public Form1()
        {
            InitializeComponent();
        }

        private void bStartServer_Click(object sender, EventArgs e)
        {
            localEndPoint = new IPEndPoint(IPAddress.Parse(tbHost.Text), int.Parse(tbPort.Text));
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
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            if (socket == null)
            {
                return;
            }

            // принимаем входящие подключения, и создаем новый Socket с ним
            Socket client = socket.EndAccept(ar);

            Task.Run(() => { StartMessaging(client); });

            socket.BeginAccept(AcceptCallback, null);
        }

        private void StartMessaging(Socket client)
        {
            Socket _client = client;

            _client.Send(Encoding.UTF8.GetBytes("Привет , клиент!"));
            StringBuilder builder = new StringBuilder();

            int len = 0;
            byte[] data = new byte[1024];

            try
            {
                // получаем пакеты данных от клиента
                do
                {
                    len = _client.Receive(data);
                    builder.Append(Encoding.UTF8.GetString(data, 0, len));
                } while (_client.Available > 0);

                if (lbMessages.InvokeRequired)
                {
                    lbMessages.Invoke(new Action(() =>
                    {
                        lbMessages.Items.Insert(0, builder.ToString());
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Error:{ex.Message}");
            }
            finally
            {
                _client.Shutdown(SocketShutdown.Both);
                _client.Close();
            }
        }
    }
}