using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace UDPclient
{
    public partial class shpenzimet : Form
    {

        private static X509Certificate2 cert = GetCertificateFromStore("E=support@uni-pr.edu, CN=www.uni-pr.edu, OU=FIEK, O=University of Prishtina, L=Washington, S=DC, C=US");
        RSACryptoServiceProvider objRsa = new RSACryptoServiceProvider();
        DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
        UdpClient klienti = new UdpClient();

        public shpenzimet()
        {
            InitializeComponent();
            
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            send();
            MessageBox.Show("Te dhenat u ruajten me sukses");
        }

        private void send()
        {
            string bill = txtBill.Text;
            string year = txtYear.Text;
            string month = txtMonth.Text;
            string cost = txtCost.Text;
            string place = txtPlace.Text;
            string register = "2";

            string message = bill + "." + year + "." + month + "." + cost + "." + place + "." + register;
            message = encrypt(message);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500);

            Socket newSocket = new Socket(AddressFamily.InterNetwork,
                                       SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500);
            EndPoint tempRemote = (EndPoint)ep;
            klienti.Connect(ep);
            byte[] data = Encoding.Default.GetBytes(message);
            klienti.Send(data, data.Length);

         

        }

        private string encrypt(string plaintext)
        {
            objDes.GenerateKey();
            objDes.GenerateIV();
            objDes.Padding = PaddingMode.Zeros;
            objDes.Mode = CipherMode.CBC;
            string key = Encoding.Default.GetString(objDes.Key);
            string IV = Encoding.Default.GetString(objDes.IV);


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
