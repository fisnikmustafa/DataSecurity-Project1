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
        private static X509Certificate2 cert = GetCertificateFromStore("E=support@uni-pr.edu, CN=www.uni-pr.edu, OU=FIEK, O=University of Prishtina, L=Washington, S=DC, C=US");

        RSACryptoServiceProvider objRsa = new RSACryptoServiceProvider();
        DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
      
        string key;
        string IV;  
        UdpClient klienti = new UdpClient();

    

        public Form1()
        {
            InitializeComponent();
           
            connect();

        }



        private void button2_Click(object sender, EventArgs e)
        {
            Register rs = new Register();
            rs.ShowDialog();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            send();
            
        }


        private void connect()
        {
            string ip = "127.0.0.1";
            int port = 8500;
            

            try
            {
                klienti.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

                read();
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
                    IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500);

                    byte[] rec = klienti.Receive(ref iep);
                    

                   
                        
                            string data = Encoding.Default.GetString(buffer);
                            //data = decrypt(data);
                            if (data.Substring(0, 5) == "ERROR")
                            {
                                MessageBox.Show("Wrong Credentials");
                            }
                            else
                            {
                                MessageBox.Show("Login success!");
                                Informatat informatat = new Informatat();
                                informatat.Show();
                                
                            }
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

            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500);
            EndPoint ep = (EndPoint)iep;
            klienti.Connect(iep);

            string msg = username + "." + password + "." + login;

            msg = encrypt(msg);
            

            byte[] data = Encoding.Default.GetBytes(msg);
            klienti.Send(data ,data.Length);
        }

        

        private string encrypt(string plaintext)
        {
            objDes.GenerateKey();
            objDes.GenerateIV();
            objDes.Padding = PaddingMode.Zeros;
            objDes.Mode = CipherMode.CBC;
            key = Encoding.Default.GetString(objDes.Key);
            IV = Encoding.Default.GetString(objDes.IV);

            objRsa = (RSACryptoServiceProvider)cert.PublicKey.Key;
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

        private static X509Certificate2 GetCertificateFromStore(string certName)
        {

            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = store.Certificates;
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, false);
                if (signingCert.Count == 0)
                    return null;
                return signingCert[0];
            }
            finally
            {
                store.Close();
            }

        }

    }

}
