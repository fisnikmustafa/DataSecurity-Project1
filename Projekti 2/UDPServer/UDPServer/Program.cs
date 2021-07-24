using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Security.Cryptography.Xml;

namespace UDPServer
{
    class Program
    {

        private static DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
        private static RSACryptoServiceProvider objRsa = new RSACryptoServiceProvider();

        private static string desKey;
        private static string desIV;

        private static X509Certificate2 cert = GetCertificateFromStore("E=support@uni-pr.edu, CN=www.uni-pr.edu, OU=FIEK, O=University of Prishtina, L=Washington, S=DC, C=US");

        private static XmlDocument objXml = new XmlDocument();

        static void Main(string[] args)
        {   
            
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 8500);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(endPoint);

            Console.WriteLine("Serveri eshte ne pritje...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 8500);
            EndPoint senderEP = (EndPoint)sender;

            while (true)
            {
                byte [] buffer = new byte[1024];
                int recv = socket.ReceiveFrom(buffer, ref senderEP); // i merr te dhenat i dergon ne buffer
                string data = Encoding.Default.GetString(buffer, 0, recv);

                
                data = decrypt(data);   //merr vetem pjesen <MSG> nga base64(<IV>.rsa(<KEY).des(<MSG>))

                string[] dataElements = data.Split('.'); // ndan mesazhin ne pjese (mesazhi: <MSG> = <username>.<password>.<action>)

                string action = dataElements[dataElements.Length - 1].Substring(0, 1);

                if (action == "1") //action = login
                {
                    
                    string strDesKey = Convert.ToBase64String(objDes.Key);
                    string strDesIV = Convert.ToBase64String(objDes.IV);

                    if (User.isUser(dataElements[0], dataElements[1])) //check if user exists
                    {
                        createUserXmlDoc(dataElements[0], dataElements[1]);
                        signUserXmlDoc();

                        string okMsg = encrypt("OK", strDesKey, strDesIV);
                        byte[] okBytes = Encoding.UTF8.GetBytes(okMsg);

                        socket.SendTo(okBytes, senderEP);

                        //send XML bytes to Client;
                        byte[] byteLines = File.ReadAllBytes("signed_user.xml");
                        socket.SendTo(byteLines, senderEP);

                        byte[] endBytes = Encoding.UTF8.GetBytes("END");
                        socket.SendTo(endBytes, senderEP);


                    } 
                    else
                    {
                        string errorMsg = encrypt("ERROR", strDesKey, strDesIV);
                        byte[] errorBytes = Encoding.UTF8.GetBytes(errorMsg);

                        socket.SendTo(errorBytes, senderEP);
                    }
                   
                }
                else if (action == "2") //action = register user
                {
                    User.insert(dataElements[0], dataElements[1], dataElements[3], dataElements[2], dataElements[4]);
                } 
                else if (action == "3") 
                {
                    //insert into faturat
                }
            }

        }


        private static string encrypt(string plaintext, string key, string iv)
        {
            objDes.Key = Convert.FromBase64String(key);
            objDes.IV = Encoding.Default.GetBytes(iv);
            objDes.Padding = PaddingMode.Zeros;
            objDes.Mode = CipherMode.CBC;


            byte[] bytePlaintexti = Encoding.UTF8.GetBytes(plaintext);

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, objDes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(bytePlaintexti, 0, bytePlaintexti.Length);
            cs.Close();

            byte[] byteCiphertexti = ms.ToArray();

            return Convert.ToBase64String(byteCiphertexti);

        }
        private static string decrypt(string ciphertext)
        {
            string[] info = ciphertext.Split('.');
            desKey = info[1];
            desIV = info[0];


            objRsa = (RSACryptoServiceProvider)cert.PrivateKey;
            byte[] byteKey = objRsa.Decrypt(Convert.FromBase64String(desKey), true);

            desKey = Convert.ToBase64String(byteKey);
            objDes.Key = byteKey;
            objDes.IV = Encoding.Default.GetBytes(desIV);
            objDes.Padding = PaddingMode.Zeros;
            objDes.Mode = CipherMode.CBC;

            byte[] byteCiphertexti = Convert.FromBase64String(info[2]);
            MemoryStream ms = new MemoryStream(byteCiphertexti);
            CryptoStream cs = new CryptoStream(ms, objDes.CreateDecryptor(), CryptoStreamMode.Read);

            byte[] byteTextiDekriptuar = new byte[ms.Length];
            cs.Read(byteTextiDekriptuar, 0, byteTextiDekriptuar.Length);
            cs.Close();

            string decryptedText = Encoding.UTF8.GetString(byteTextiDekriptuar);
            return decryptedText;
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


        private static void createUserXmlDoc(string username, string password)
        {

            if (File.Exists("user.xml") == false)
            {
                XmlTextWriter xmlTw = new XmlTextWriter("user.xml", Encoding.UTF8);
                xmlTw.WriteStartElement("user");
                xmlTw.Close();
            }

            objXml.Load("user.xml");


            XmlElement rootNode = objXml.DocumentElement;

            if(User.getUserInfo(username, password) == null)
            {
                return;
            }

            string[] userDetails = User.getUserInfo(username, password);

            XmlElement userNode = objXml.CreateElement("user");
            XmlElement firstnameNode = objXml.CreateElement("firstname");
            XmlElement lastnameNode = objXml.CreateElement("lastname");
            XmlElement usernameNode = objXml.CreateElement("username");
            XmlElement emailNode = objXml.CreateElement("email");


            firstnameNode.InnerText = userDetails[0];
            lastnameNode.InnerText = userDetails[1];
            usernameNode.InnerText = userDetails[2];
            emailNode.InnerText = userDetails[3];


            userNode.AppendChild(firstnameNode);
            userNode.AppendChild(lastnameNode);
            userNode.AppendChild(usernameNode);
            userNode.AppendChild(emailNode);

            rootNode.AppendChild(userNode);


            objXml.Save("user.xml");
        }

        private static void signUserXmlDoc()
        {
            objXml.Load("user.xml");

            objRsa = (RSACryptoServiceProvider)cert.PrivateKey;

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

            objXml.Save("signed_user.xml");

        }

       
    }
}
