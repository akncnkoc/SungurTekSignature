using SungurTekSignature.Signature;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SungurTekSignature.Signature.SmartCard;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.common.util.bag;
using tr.gov.tubitak.uekae.esya.api.smartcard.pkcs11;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;
using System.IO;
using AkisCIF;
using Microsoft.VisualBasic;
using tr.gov.tubitak.uekae.esya.asn.util;

namespace SungurTekSignature
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public IBaseSmartCard bsc;
        private void Form2_Load(object sender, EventArgs e)
        {

            //BaseSignedData bs = new BaseSignedData();
            //ISignable content = new SignableByteArray(ASCIIEncoding.ASCII.GetBytes("akın"));
            //bs.addContent(content);

            //Dictionary<String, Object> params_ = new Dictionary<String, Object>();
            ////if the user does not want certificate validation at generating signature,he can add 
            ////P_VALIDATE_CERTIFICATE_BEFORE_SIGNING parameter with its value set to false
            //params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;

            ////necessary for certificate validation.By default,certificate validation is done 
            //params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();

            ////By default, QC statement is checked,and signature wont be created if it is not a 
            ////qualified certificate. 

            //bool checkQCStatement = TestConstants.getCheckQCStatement();

            ////Get qualified or non-qualified certificate.
            //ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
            //String[] terminals = SmartOp.getCardTerminals();


            //Pair<long, CardType> slotAndCardType = SmartOp.getSlotAndCardType(terminals[0]);
            //bsc = new P11SmartCard(slotAndCardType.getmObj2());
            //bsc.openSession(slotAndCardType.getmObj1());

            //treeView1.Nodes.Add(bsc.);

            //BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);

            ////add signer
            ////Since the specified attributes are mandatory for bes,null is given as parameter 
            ////for optional attributes
            //try
            //{
            //    bs.addSigner(ESignatureType.TYPE_BES, cert, signer, null, params_);
            //}
            //catch (CertificateValidationException cve)
            //{
            //    Console.WriteLine(cve.getCertStatusInfo().getDetailedMessage());
            //}


            //SmartCardManager.getInstance().logout();

            //byte[] signedDocument = bs.getEncoded();

            ////write the contentinfo to file
            //DirectoryInfo di = Directory.CreateDirectory(TestConstants.getDirectory() + @"\testVerileri");
            //AsnIO.dosyayaz(signedDocument, di.FullName + @"\BES-1.p7s");


            var testCostants = new TestConstants();

            string selectedTerminal = "";
            Pair<long, CardType> sloatAndCardType = new Pair<long, CardType>();
            string[] cardTerminals = SmartOp.getCardTerminals();
            if (cardTerminals.Length == 1)
            {
                selectedTerminal = cardTerminals[0];
                treeView1.Nodes.Add(selectedTerminal);
                sloatAndCardType = SmartOp.getSlotAndCardType(selectedTerminal);
                var cardType = sloatAndCardType.getmObj2();
                long cardSlot = sloatAndCardType.getmObj1();
                treeView1.Nodes[0].Nodes.Add(cardType.getLibName() + "-" + cardType.getName());
                SmartCard sc = new SmartCard(CardType.getCardType(cardType.getLibName()));

                long session = sc.openSession(cardSlot);
                //Login sonradan olacak
                //sc.login(session, "977754");

                List<byte[]> certBytes = sc.getSignatureCertificates(session);
                foreach (byte[] bs in certBytes)
                {
                    ECertificate cert = new ECertificate(bs);
                    //cert.isQualifiedCertificate()
                    treeView1.Nodes[0].Nodes[0].Nodes.Add(cert.getSerialNumberHex(), cert.getSubject().getCommonNameAttribute());
                }
                treeView1.CollapseAll();
                treeView1.SelectedNode = treeView1.Nodes[0].Nodes[0].LastNode;
                treeView1.SelectedNode.BackColor = Color.Bisque;
            }
            else
            {
                foreach (var item in cardTerminals)
                {
                    treeView1.Nodes.Add(item);
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string password = Interaction.InputBox("Şifre", "Şifrenizi giriniz");
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            e.Node.BackColor = Color.Red;
        }
    }
}
