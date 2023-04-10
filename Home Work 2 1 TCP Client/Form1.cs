using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Home_Work_2_1_TCP_Client
{
    public partial class Form1 : Form
    {
        private IPEndPoint remotEndPoint;
        private Socket clientSocket;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // устанавливаем к кому мы будет подключатся ( host ip Port )
            remotEndPoint = new IPEndPoint(IPAddress.Parse(tbHost.Text), int.Parse(tbPort.Text));
            // создаем локальный сокет для подключения на него
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Connect();
        }

        private void Connect()
        {
            clientSocket.Connect(remotEndPoint);

            clientSocket.Send(Encoding.UTF8.GetBytes("Привет , сервер!"));

            StringBuilder builder = new StringBuilder();

            int len = 0;

            byte[] data = new byte[1024];
            try
            {
                do
                {
                    len = clientSocket.Receive(data);
                    builder.Append(Encoding.UTF8.GetString(data, 0, len));
                } while (clientSocket.Available > 0);

                lbMessages.Items.Insert(0, builder.ToString());

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show($@"Error: {e.Message}");
            }
        }
    }
}