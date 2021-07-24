using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace UDPclient
{
    public partial class Informatat : Form
    {
        public Informatat()
        {
            InitializeComponent();
        }
        XmlDocument objXml = new XmlDocument();

        private void btnVerify_Click(object sender, EventArgs e)
        {
            objXml.Load("fatura_e_nenshkruar.xml");

            SignedXml objSignedXml = new SignedXml(objXml);

            XmlNodeList signatureNodes = objXml.GetElementsByTagName("Signature");
            XmlElement signatureNode = (XmlElement)signatureNodes[0];

            objSignedXml.LoadXml(signatureNode);

            if (objSignedXml.CheckSignature() == true)
            {
                Info inf = new Info();
                inf.Show();

            }
            else
            {
                MessageBox.Show("Nenshkrimi nuk eshte valid");
            }
        }
    }
}
