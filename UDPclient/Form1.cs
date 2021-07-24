using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace UDPclient
{
    public partial class Form1 : Form
    {
        Socket clientSocket;
        RSACryptoServiceProvider objRsa = new RSACryptoServiceProvider();
        DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
        X509Certificate2 certifikata = new X509Certificate2();
        string key;
        string IV;
        XmlDocument objXml = new XmlDocument();

        Socket socket()
        {
            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            return newSocket;
        }


        public Form1()
        {
            InitializeComponent();
            clientSocket = socket();
            connect();

        }



        private void button2_Click(object sender, EventArgs e)
        {
            Register rs = new Register(clientSocket, certifikata);
            rs.ShowDialog();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            send();
            
        }


        private void connect()
        {
            string ip = "127.0.0.1";
            int port = 3;

            try
            {
                clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

                new Thread(() =>
                {
                    read();
                }).Start();
            }
            catch
            {
                MessageBox.Show("Connection Failed");
            }

        }

        void read()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[2048];
                    int rec = clientSocket.Receive(buffer, 0, buffer.Length, 0);
                    if (rec <= 0)
                    {
                        throw new SocketException();
                    }
                    Array.Resize(ref buffer, rec);

                    Invoke((MethodInvoker)delegate
                    {
                        if (buffer.Length > 900)
                        {
                            certifikata.Import(buffer);
                        }
                        else
                        {
                            string data = Encoding.Default.GetString(buffer);
                            //data = decrypt(data);
                            if (data.Substring(0, 5) == "error")
                            {
                                MessageBox.Show("Wrong Credentials");
                            }
                            else
                            {
                                MessageBox.Show("Jeni kycur me sukses");
                                Informatat informatat = new Informatat();
                                informatat.Show();
                                
                            }
                        }
                    });
                }
                catch
                {
                    MessageBox.Show("Disconnected");
                    Application.Exit();
                }
            }
        }

        private void send()
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string login = "1";

            string msg = username + "." + password + "." + login;

            msg = encrypt(msg);
            byte[] data = Encoding.Default.GetBytes(msg);
            clientSocket.Send(data, 0, data.Length, 0);
        }

        

        private string encrypt(string plaintext)
        {
            objDes.GenerateKey();
            objDes.GenerateIV();
            objDes.Padding = PaddingMode.Zeros;
            objDes.Mode = CipherMode.CBC;
            key = Encoding.Default.GetString(objDes.Key);
            IV = Encoding.Default.GetString(objDes.IV);

            objRsa = (RSACryptoServiceProvider)certifikata.PublicKey.Key;
            byte[] byteKey = objRsa.Encrypt(objDes.Key, true);
            string encryptedKey = Convert.ToBase64String(byteKey);

            byte[] bytePlaintexti = Encoding.UTF8.GetBytes(plaintext);

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, objDes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(bytePlaintexti, 0, bytePlaintexti.Length);
            cs.Close();



            byte[] byteCiphertexti = ms.ToArray();

            return IV + "." + encryptedKey + "." + Convert.ToBase64String(byteCiphertexti);

        }

    }

}
