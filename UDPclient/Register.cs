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
using System.Xml;
using System.Security.Cryptography.Xml;

namespace UDPclient
{
    public partial class Register : Form
    {
        X509Certificate2 certifikata;
        RSACryptoServiceProvider objRsa = new RSACryptoServiceProvider();
        DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
        Socket clientSocket;
        XmlDocument objXml = new XmlDocument();

        public Register(Socket socket, X509Certificate2 certificate2)
        {
            InitializeComponent();
            
            clientSocket = socket;
            certifikata = certificate2;
            
        }


        private void btnRegister_Click(object sender, EventArgs e)
        {

            send();
            //XmlFile();
            //xmlSignature();
            MessageBox.Show("Te dhenat u ruajten me sukses!");
          
        }



        private void send()
        {
            string fname = txtFirstName.Text;
            string lname = txtLastName.Text;
            string email = txtEmail.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Text;
           

            string message = fname + "." + lname + "." + email + "." + username + "." + password;
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

        void XmlFile()
        {
            if (File.Exists("information.xml") == false)
            {
                XmlTextWriter xmlTw = new XmlTextWriter("fatura.xml", Encoding.UTF8);
                xmlTw.WriteStartElement("fatura");
                xmlTw.Close();
            }

            objXml.Load("information.xml");

            XmlElement rootNode = objXml.DocumentElement;

            XmlElement userNode = objXml.CreateElement("users");
            XmlElement nameNode = objXml.CreateElement("firstname");
            XmlElement surnameNode = objXml.CreateElement("lastname");
            XmlElement usernameNode = objXml.CreateElement("username");
            XmlElement emailNode = objXml.CreateElement("email");


            nameNode.InnerText = txtFirstName.Text;
            surnameNode.InnerText = txtLastName.Text;
            usernameNode.InnerText =txtUsername.Text;
            emailNode.InnerText = txtEmail.Text;

            userNode.AppendChild(nameNode);
            userNode.AppendChild(surnameNode);
            userNode.AppendChild(usernameNode);
            userNode.AppendChild(emailNode);

            rootNode.AppendChild(userNode);

            objXml.Save("information.xml");
        }

        void xmlSignature()
        {
            objXml.Load("fatura.xml");

            SignedXml objSignedXml = new SignedXml(objXml);

            Reference referenca = new Reference();
            referenca.Uri = "";


            XmlDsigEnvelopedSignatureTransform xmlDsigEnvelopedSignatureTransform = new XmlDsigEnvelopedSignatureTransform();

            referenca.AddTransform(xmlDsigEnvelopedSignatureTransform);

            objSignedXml.AddReference(referenca);

            KeyInfo ki = new KeyInfo();
            ki.AddClause(new RSAKeyValue(objRsa));


            objSignedXml.KeyInfo = ki;

            objSignedXml.SigningKey = objRsa;

            objSignedXml.ComputeSignature();

            XmlElement signatureNode = objSignedXml.GetXml();
            XmlElement rootNode = objXml.DocumentElement;

            rootNode.AppendChild(signatureNode);

            objXml.Save("fatura_e_nenshkruar.xml");

           
        }



    }
}
