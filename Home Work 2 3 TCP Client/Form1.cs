﻿using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Home_Work_2_3_TCP_Client
{
    public partial class Form1 : Form
    {
        private Client client;

        public Form1()
        {
            InitializeComponent();
        }

        private async void bConnect_Click(object sender, EventArgs e)
        {
            bConnect.Enabled = false;
            client = new Client(IPAddress.Parse(tbIP.Text), int.Parse(tbPort.Text));
            await client.Start();
            if (client.IsConnect)
            {
                pictureBox1.BackColor = Color.Green;
                gbMessage.Enabled = true;

                Thread thread = new Thread(async () =>
                {
                    while (client.IsConnect)
                    {
                        string mes = await client.ReaderNetworkStream();
                        if (mes != String.Empty)
                        {
                            if (lbChat.InvokeRequired)
                            {
                                lbChat.Invoke(new Action(() => { lbChat.Items.Add($@"Server >> {mes}"); }));
                            }
                        }
                    }

                }) { IsBackground = true };
                thread.Start();
            }
            else
            {
                bConnect.Enabled = true;
            }

        }

        private void bSendMessage_Click(object sender, EventArgs e)
        {
            if (tbMessage.Text == @"Bye")
            {
                client.IsConnect = false;
                gbMessage.Enabled = false;
                bConnect.Enabled = true;
                tbMessage.Text = String.Empty;
                pictureBox1.BackColor = Color.Red;
                lbChat.Items.Clear();
                client.Disconnect();
            }
            else
            {
                lbChat.Items.Add($@"Me >> {tbMessage.Text}");
                client.SendMessage(tbMessage.Text);
            }
        }
    }

    class Client
    {
        public bool IsConnect = false;

        // конечная точка хоста
        private IPEndPoint remoteEndPoint;

        private Socket _socket;

        // инициализация клиента
        public Client(IPAddress ip, int port)
        {
            remoteEndPoint = new IPEndPoint(ip, port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task Start()
        {
            try
            {
                await _socket.ConnectAsync(remoteEndPoint);

                IsConnect = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // получить пакет от сервера
        public Task<string> ReaderNetworkStream()
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                NetworkStream stream = new NetworkStream(_socket);
                int len = 0;
                byte[] data = new byte[1024];

                do
                {
                    len = stream.Read(data, 0, 1024);
                    builder.Append(Encoding.UTF8.GetString(data, 0, len));
                } while (stream.DataAvailable);
                return Task.FromResult(builder.ToString());
            }
            catch
            {
                //
                return Task.FromResult(string.Empty);
            }
        }

        // отключится от сервера
        public void Disconnect()
        {
            if (_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
            }

            _socket.Close();
        }

        // отправка сообщения
        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            _socket.Send(data);
        }
    }
}