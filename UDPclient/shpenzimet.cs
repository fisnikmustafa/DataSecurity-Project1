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
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace UDPclient
{
    public partial class shpenzimet : Form
    {

        X509Certificate2 certifikata;
        RSACryptoServiceProvider objRsa = new RSACryptoServiceProvider();
        DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
        Socket clientSocket;

        public shpenzimet(Socket socket, X509Certificate2 certificate2)
        {
            InitializeComponent();
            clientSocket = socket;
            certifikata = certificate2;
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
            byte[] data = Encoding.Default.GetBytes(message);
            clientSocket.Send(data, 0, data.Length, 0);

        }

        private string encrypt(string plaintext)
        {
            objDes.GenerateKey();
            objDes.GenerateIV();
            objDes.Padding = PaddingMode.Zeros;
            objDes.Mode = CipherMode.CBC;
            string key = Encoding.Default.GetString(objDes.Key);
            string IV = Encoding.Default.GetString(objDes.IV);


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



        //private void txtBill_Enter(object sender, EventArgs e)
        //{
        //    String fname = txtBill.Text;
        //    if (fname.ToLower().Trim().Equals("bill type"))
        //    {
        //        txtBill.Text = "";
        //        txtBill.ForeColor = Color.Black;
        //    }
        //}

        //private void txtBill_Leave(object sender, EventArgs e)
        //{
        //    String fname = txtBill.Text;
        //    if (fname.ToLower().Trim().Equals("bill type") || fname.Trim().Equals(""))
        //    {
        //        txtBill.Text = "bill type";
        //        txtBill.ForeColor = Color.Gray;
        //    }
        //}

        //private void txtYear_Enter(object sender, EventArgs e)
        //{

        //    String lname = txtYear.Text;
        //    if (lname.ToLower().Trim().Equals("Year"))
        //    {
        //        txtYear.Text = "";
        //        txtYear.ForeColor = Color.Black;
        //    }


        //}

        //private void txtYear_Leave(object sender, EventArgs e)
        //{

        //    String lname = txtYear.Text;
        //    if (lname.ToLower().Trim().Equals("year") || lname.Trim().Equals(""))
        //    {
        //        txtYear.Text = "year";
        //        txtYear.ForeColor = Color.Gray;
        //    }

        //}

        //private void txtMonth_Enter(object sender, EventArgs e)
        //{

        //    String email = txtMonth.Text;
        //    if (email.ToLower().Trim().Equals("month"))
        //    {
        //        txtMonth.Text = "";
        //        txtMonth.ForeColor = Color.Black;
        //    }


        //}

        //private void txtMonth_Leave(object sender, EventArgs e)
        //{

        //    String email = txtMonth.Text;
        //    if (email.ToLower().Trim().Equals("month") || email.Trim().Equals(""))
        //    {
        //        txtMonth.Text = "month";
        //        txtMonth.ForeColor = Color.Gray;
        //    }

        //}

        //private void txtCost_Enter(object sender, EventArgs e)
        //{

        //    String username = txtMonth.Text;
        //    if (username.ToLower().Trim().Equals("cost"))
        //    {
        //        txtCost.Text = "";
        //        txtCost.ForeColor = Color.Black;
        //    }

        //}

        //private void txtCost_Leave(object sender, EventArgs e)
        //{

        //    String username = txtCost.Text;
        //    if (username.ToLower().Trim().Equals("totali euro") || username.Trim().Equals(""))
        //    {
        //        txtCost.Text = "cost";
        //        txtCost.ForeColor = Color.Gray;
        //    }

        //}

        //private void txtPlace_Enter(object sender, EventArgs e)
        //{
        //    String fname = txtBill.Text;
        //    if (fname.ToLower().Trim().Equals("place"))
        //    {
        //        txtBill.Text = "";
        //        txtBill.ForeColor = Color.Black;
        //    }
        //}

        //private void txtDollar_Leave(object sender, EventArgs e)
        //{
        //    String fname = txtBill.Text;
        //    if (fname.ToLower().Trim().Equals("place") || fname.Trim().Equals(""))
        //    {
        //        txtBill.Text = "place";
        //        txtBill.ForeColor = Color.Gray;
        //    }
        //}

        //public static X509Certificate2 GetCertificateFromStore(string certificateName)
        //{
        //    X509Store xstore = new X509Store(StoreLocation.CurrentUser);

        //    try
        //    {
        //        xstore.Open(OpenFlags.ReadOnly);

        //        X509Certificate2Collection certCollection = xstore.Certificates;
        //        X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
        //        X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certificateName, false);

        //        if (signingCert.Count == 0)
        //            return null;
        //        return signingCert[0];
        //    }
        //    finally
        //    {
        //        xstore.Close();
        //    }
        //}

        //public static byte[] EncryptDataSha1(X509Certificate2 certificate, byte[] data)
        //{
        //    using (RSA rsa = certificate.GetRSAPublicKey())
        //    {
        //        return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
        //    }
        //}

        //private void btnRegister_Click(object sender, EventArgs e)
        //{
        //    UdpClient klienti = new UdpClient();
        //    IPEndPoint ipend = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12000);
        //    klienti.Connect(ipend);

        //    byte[] byteSend = Encoding.ASCII.GetBytes(

        //                                    txtBill.Text + " " +
        //                                    txtYear.Text + " " +
        //                                    txtMonth.Text + " " +
        //                                    txtCost.Text + " " +
        //                                    txtPlace.Text


        //                            );

        //    klienti.Send(byteSend, byteSend.Length);

        //    X509Certificate2 certificate = GetCertificateFromStore("CN=RootCA");
        //    if (certificate == null)
        //    {
        //        Console.WriteLine("Certificate is not found.");
        //        Console.ReadLine();
        //    }

        //    string bill = txtBill.Text.Trim();
        //    string year = txtYear.Text.Trim();
        //    string month = txtMonth.Text.Trim();

        //    Des des = new Des();

        //    string message = bill + ":" + year + ":" + month + "!";
        //    Console.WriteLine(message);
        //    byte[] encryptedDATA = des.Enkripto(message);

        //    byte[] IV = des.getIV();
        //    byte[] key = des.getKey();

        //    byte[] encKey = EncryptDataSha1(certificate, key);

        //    Console.WriteLine(encKey.Length);
        //    Console.WriteLine(Convert.ToBase64String(encKey));

        //    Console.WriteLine(Convert.ToBase64String(key));
        //    Console.WriteLine(Convert.ToBase64String(DecryptDataSha1(certificate, encKey)));

        //    string delimiter = ".";
        //    string fullmessageEncrypted = Convert.ToBase64String(IV) + delimiter + Convert.ToBase64String(encKey) + delimiter + Convert.ToBase64String(encryptedDATA);



        //}
        //public static byte[] DecryptDataSha1(X509Certificate2 certificate, byte[] data)
        //    {
        //        // GetRSAPrivateKey returns an object with an independent lifetime, so it should be
        //        // handled via a using statement.
        //        using (RSA rsa = certificate.GetRSAPrivateKey())
        //        {
        //            return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA1);
        //        }
        //    }


    }     

    
}
