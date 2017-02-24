using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Receiver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public bool IsStopRequested { get; set; }
        private int _portToRun;
        private void button1_Click(object sender, EventArgs e)
        {
            StartReceiverAsync(1000); // Call function to start receive data on the same port 1000
        }
        /// <summary>
        ///  Use tcp listening to receive data by accept socet from sender on same port (1000)  and call processASync to get data 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task<List<Task<bool>>> StartReceiverAsync(int port)
        {
            return await Task.Factory.StartNew(() =>
            {
                _portToRun = port;
                List<Task<bool>> taskList = new List<Task<bool>>();
                TcpListener listener = new TcpListener(IPAddress.Any, _portToRun);
                listener.Start();
                while (!IsStopRequested)
                {
                    Socket client = listener.AcceptSocket();

                    taskList.Add(ProcessAsync(client));
                }
                return taskList;
            });
        }
        /// <summary>
        /// Get multiple data received and show it in message 
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <returns></returns>
        internal async Task<bool> ProcessAsync(Socket clientSocket)
        {

            return await Task.Factory.StartNew(() =>
            {
                var ns = new NetworkStream(clientSocket);
                byte[] buffer = new byte[1024];
                while (ns.Read(buffer, 0, buffer.Length) > 0)
                {
                    MessageBox.Show(System.Text.Encoding.Default.GetString(buffer));
                }
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Disconnect(true);
                // sr.Close();
                return true;
            });
        }
    }
}
