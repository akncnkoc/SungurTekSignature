using SungurTekSignature.Signature;
using SungurTekSignature.Signature.SmartCard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.common.util.bag;
using tr.gov.tubitak.uekae.esya.api.smartcard.pkcs11;
using System.IO;
using iaik.pkcs.pkcs11.wrapper;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.asn.util;
using tr.gov.tubitak.uekae.esya.api.infra.tsclient;
using tr.gov.tubitak.uekae.esya.api.crypto.alg;
using Microsoft.Win32;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Text;

namespace SungurTekSignature
{
    public partial class Form1 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        //BORDER RADIUS
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        private static String time_stamp_url = "http://zd.kamusm.gov.tr";
        private static int time_stamp_id = 18434;
        private static String time_stamp_pass = "6keOOHo7";

        const string UriScheme = "SungurTekImza";
        const string FriendlyName = "Sungur Teknolojileri E-Imza";
        private string fileNameUrl = "";

        private string imza = "";
        private string detached = "";
        private string returnurl = "";
        private string imzaturu = "";
        private string dosyaadi = "";
        private string[] uzantis = new string[] {};
        private string uzanti = "";
        private string imzalanmis = "";
        private string pathUrl = "";

        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }
        
        

       
        private void Form1_Load(object sender, EventArgs e)
        {
            String[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var url = new Uri(args[1]);
                fileNameUrl = url.GetLeftPart(UriPartial.Path).Remove(0,15);
                uzantis = url.AbsolutePath.Split('.');
                uzanti = uzantis[uzantis.Length - 1];
                imza = HttpUtility.ParseQueryString(url.Query).Get("imza");
                detached = HttpUtility.ParseQueryString(url.Query).Get("detached");
                returnurl = HttpUtility.ParseQueryString(url.Query).Get("url");
                imzaturu = HttpUtility.ParseQueryString(url.Query).Get("imzaturu");
                imzalanmis = HttpUtility.ParseQueryString(url.Query).Get("imzalanmis");
                dosyaadi = HttpUtility.ParseQueryString(url.Query).Get("dosyaadi");

            }
            this.timer1.Interval = 1000;
            timer1.Start();

            this.btnSayi0.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.btnSayi0.Width, this.btnSayi0.Height, 4, 4));
            this.btnSayi1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.btnSayi0.Width, this.btnSayi0.Height, 4, 4));
            this.btnSayi2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.btnSayi0.Width, this.btnSayi0.Height, 4, 4));
            this.btnSayi3.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.btnSayi0.Width, this.btnSayi0.Height, 4, 4));
            this.btnSayi4.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.btnSayi0.Width, this.btnSayi0.Height, 4, 4));
            this.btnSayi5.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.btnSayi0.Width, this.btnSayi0.Height, 4, 4));
            this.btnSayi6.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.btnSayi0.Width, this.btnSayi0.Height, 4, 4));
            this.btnSayi7.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.btnSayi0.Width, this.btnSayi0.Height, 4, 4));
            this.btnSayi8.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.btnSayi0.Width, this.btnSayi0.Height, 4, 4));
            this.btnSayi9.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.btnSayi0.Width, this.btnSayi0.Height, 4, 4));

            //WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            //bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

            //if (!hasAdministrativeRight)
            //{
            //    ProcessStartInfo processInfo = new ProcessStartInfo();
            //    processInfo.Verb = "runas";
            //    processInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "SungurTekSignature.exe";
            //    Process.Start(processInfo);
            //    this.Close();
            //}

            //addingProtocols();

            
            string sourceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"smartcard-config.xml");
            

            string destinationFileGoogle = @"C:\\Program Files (x86)\\Google\\Chrome\\Application\\";
            string destinationFileFirefox = @"C:\\Program Files\\Mozilla Firefox\\";

            string[] directoriesGoogle = Directory.GetDirectories(destinationFileGoogle);
            foreach (var s in directoriesGoogle)
            {
                string news = s.Replace(destinationFileGoogle, "").Replace(".", "").Trim();
                string wReplaces = s.Replace(destinationFileGoogle, "");
                if (decimal.TryParse(news ,out decimal val))
                {
                    try
                    {
                        if (Directory.Exists(destinationFileGoogle))
                        {
                            if (!File.Exists(destinationFileGoogle + wReplaces + "\\smartcard-config.xml"))
                            {
                                File.Copy(sourceFile, destinationFileGoogle + wReplaces + "\\smartcard-config.xml",
                                    true);
                            }
                        }

                        if(Directory.Exists(destinationFileFirefox))
                        {
                            if(!File.Exists(destinationFileFirefox + "\\smartcard-config.xml")) { 
                                File.Copy(sourceFile, destinationFileFirefox + "\\smartcard-config.xml", true);
                            }
                        }
                    }
                    catch (IOException exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }


            
        }
        private void addingProtocols()
        {
            using (var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\"+ UriScheme))
            {
                string appLocation = AppDomain.CurrentDomain.BaseDirectory+ "SungurTekSignature.exe";

                key.SetValue("","URL:"+FriendlyName);
                key.SetValue("URL Protocol","");
                using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                {
                    defaultIcon.SetValue("",appLocation+",1");
                }

                using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                {
                    commandKey.SetValue("","\""+ appLocation + "\" \"%1\"");
                }
            }
        }
        private void getTerminalCards()
        {
            var testCostants = new TestConstants();
            try
            {
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
                    treeView1.Nodes[0].Nodes
                        .Add(cardType.getLibName(), cardType.getLibName() + "-" + cardType.getName());
                    SmartCard sc = new SmartCard(CardType.getCardType(cardType.getLibName()));

                    long session = sc.openSession(cardSlot);

                    List<byte[]> certBytes = sc.getSignatureCertificates(session);
                    foreach (byte[] bsa in certBytes)
                    {
                        ECertificate cert = new ECertificate(bsa);
                        treeView1.Nodes[0].Nodes[0].Nodes.Add(cert.getSerialNumberHex(),
                            cert.getSubject().getCommonNameAttribute());
                    }

                    treeView1.CollapseAll();
                    treeView1.SelectedNode = treeView1.Nodes[0].Nodes[0].LastNode;
                    treeView1.SelectedNode.BackColor = Color.Bisque;
                    this.label4.Visible = false;
                }
                else
                {
                    foreach (var item in cardTerminals)
                    {
                        treeView1.Nodes.Add(item);
                    }
                }

                
            }
            catch (SmartCardException e)
            {
                MessageBox.Show("Sisteminizde bir akıllı kart takılı değil.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kartlar getirilirken bir sorun oluştu. Tekrar deneyiniz");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnImzala_Click(object sender, EventArgs e)
        {
            try
            {
                string password = txtPassword.Text;
                if (int.TryParse(password, out int passwdInt) && password != null)
                {
                    string selectedTerminal = "";
                    Pair<long, CardType> sloatAndCardType = new Pair<long, CardType>();
                    string[] cardTerminals = SmartOp.getCardTerminals();
                    if (cardTerminals.Length >= 1)
                    {
                        try
                        {
                            selectedTerminal = cardTerminals[0];
                            sloatAndCardType = SmartOp.getSlotAndCardType(selectedTerminal);
                            var cardType = sloatAndCardType.getmObj2();
                            long cardSlot = sloatAndCardType.getmObj1();
                            SmartCard sc = new SmartCard(CardType.getCardType(cardType.getLibName()));
                            long session = sc.openSession(cardSlot);
                            sc.login(session, passwdInt.ToString());

                            CK_SESSION_INFO scInfo = sc.getSessionInfo(session);


                            if (scInfo != null)
                            {

                                if (imzalanmis == "true")
                                {
                                    try
                                    {
                                        byte[] signature = AsnIO.dosyadanOKU(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "donenveri/"+dosyaadi+"."+uzanti+".p7s"));
                                        BaseSignedData bs = new BaseSignedData(signature);

                                        Dictionary<String, Object> params_ = new Dictionary<String, Object>();

                                        params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;

                                        params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();
                                        bool checkQCStatement = TestConstants.getCheckQCStatement();
                                        ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
                                        BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);
                                        try
                                        {
                                            Signer cSigner = bs.getSignerList()[0];

                                            if (imza == "zaman")
                                            {
                                                TSSettings tsSettings = new TSSettings(time_stamp_url, time_stamp_id, time_stamp_pass, DigestAlg.SHA512);
                                                params_.Add(EParameters.P_TSS_INFO, tsSettings);
                                                cSigner.addCounterSigner(ESignatureType.TYPE_EST, cert, signer, null, params_);
                                            }
                                            else if (imza == "normal")
                                            {
                                                List<IAttribute> optionalAttributes = new List<IAttribute>();
                                                optionalAttributes.Add(new SigningTimeAttr(DateTime.UtcNow));
                                                cSigner.addCounterSigner(ESignatureType.TYPE_BES, cert, signer, optionalAttributes, params_);
                                            }

                                        }
                                        catch (NullReferenceException exception)
                                        {
                                            //Eğer bir kullanıcı daha önce imzalamamışssa burası çalışıyor
                                            if (imza == "zaman")
                                            {
                                                TSSettings tsSettings = new TSSettings(time_stamp_url, time_stamp_id, time_stamp_pass, DigestAlg.SHA512);
                                                params_.Add(EParameters.P_TSS_INFO, tsSettings);
                                                bs.addSigner(ESignatureType.TYPE_EST, cert, signer, null, params_);
                                            }
                                            else if (imza == "normal")
                                            {
                                                List<IAttribute> optionalAttributes = new List<IAttribute>();
                                                optionalAttributes.Add(new SigningTimeAttr(DateTime.UtcNow));
                                                bs.addSigner(ESignatureType.TYPE_BES, cert, signer, optionalAttributes, params_);
                                            }
                                        }


                                        byte[] signedDocument = bs.getEncoded();

                                        //write the contentinfo to file
                                        DirectoryInfo di = Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "donenveri"));
                                        AsnIO.dosyayaz(signedDocument, di.FullName + @"\" + dosyaadi +"."+uzanti+ ".p7s");
                                        SmartCardManager.getInstance().logout();

                                       

                                    }
                                    catch (NotSignedDataException cve)
                                    {
                                        MessageBox.Show("Bu dosya daha önce imzalanmamış");
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        byte[] buff = null;
                                        WebClient c = new WebClient();
                                        buff = c.DownloadData(fileNameUrl);
                                        BaseSignedData bs = new BaseSignedData();
                                        ISignable _iSignableContent = new SignableByteArray(buff);
                                        bs.addContent(_iSignableContent);
                                        Dictionary<String, Object> params_ = new Dictionary<String, Object>();
                                        params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;
                                        params_[EParameters.P_CERT_VALIDATION_POLICY] = TestConstants.getPolicy();
                                        bool checkQCStatement = TestConstants.getCheckQCStatement();
                                        ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement, !checkQCStatement);
                                        BaseSigner signer = SmartCardManager.getInstance().getSigner(TestConstants.getPIN(), cert);
                                        try
                                        {
                                            if (imza == "zaman")
                                            {
                                                TSSettings tsSettings = new TSSettings(time_stamp_url, time_stamp_id, time_stamp_pass, DigestAlg.SHA512);
                                                params_.Add(EParameters.P_TSS_INFO, tsSettings);
                                                bs.addSigner(ESignatureType.TYPE_EST, cert, signer, null, params_);
                                            }
                                            else if (imza == "normal")
                                            {
                                                List<IAttribute> optionalAttributes = new List<IAttribute>();
                                                optionalAttributes.Add(new SigningTimeAttr(DateTime.UtcNow));
                                                TSSettings tsSettings = new TSSettings(time_stamp_url, time_stamp_id, time_stamp_pass, DigestAlg.SHA512);

                                                bs.addSigner(ESignatureType.TYPE_BES, cert, signer, optionalAttributes, params_);
                                            }

                                        }
                                        catch (NullReferenceException exception)
                                        {
                                            //Eğer bir kullanıcı daha önce imzalamamışssa burası çalışıyor
                                            MessageBox.Show(exception.Source);
                                        }


                                        byte[] signedDocument = bs.getEncoded();

                                        //write the contentinfo to file
                                        DirectoryInfo di = Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "donenveri"));
                                        AsnIO.dosyayaz(signedDocument, di.FullName + @"\" + dosyaadi +"."+uzanti+ ".p7s");
                                        if (File.Exists(di.FullName + @"\" + dosyaadi + "." + uzanti + ".p7s"))
                                        {
                                            //FileStream fs = new FileStream(di.FullName + @"\" + dosyaadi + "." + uzanti + ".p7s", FileMode.Open, FileAccess.Read);
                                            try
                                            {
                                                string requestURL = "http://localhost:24298/Home/Post";
                                                string fileName = di.FullName + @"\" + dosyaadi + "." + uzanti + ".p7s";

                                                Send(requestURL,signedDocument);
                                            }
                                            catch (Exception exp)
                                            {
                                                throw;
                                            }
                                        }
                                        SmartCardManager.getInstance().logout();
                                    }
                                    catch (NotSignedDataException cve)
                                    {
                                        MessageBox.Show("Bu dosya daha önce imzalanmamış");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Error");
                            }

                        }
                        catch (Exception exception)
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Şifre boş geçilemez.");
                }
            }
            catch (NullReferenceException exception)
            {
                MessageBox.Show("Lütfen bir akıllı kart seçip işleme öyle devam ediniz");
            }
            

        }

        private async void Send(string URI, byte[] data)
        {
            using (var client = new HttpClient())
            {
                var multipartContent = new MultipartFormDataContent();
                var byteContent = new ByteArrayContent(data);
                

                multipartContent.Add(byteContent, "file", "file");
                var response = await client.PostAsync(URI, multipartContent);
            }
        }


        private void btnSayi1_Click(object sender, EventArgs e)
        {
            txtPassword.Text = txtPassword.Text + "1";
        }

        private void btnSayi2_Click(object sender, EventArgs e)
        {
            txtPassword.Text = txtPassword.Text + "2";
        }

        private void btnSayi3_Click(object sender, EventArgs e)
        {
            txtPassword.Text = txtPassword.Text + "3";
        }

        private void btnSayi4_Click(object sender, EventArgs e)
        {
            txtPassword.Text = txtPassword.Text + "4";
        }

        private void btnSayi5_Click(object sender, EventArgs e)
        {
            txtPassword.Text = txtPassword.Text + "5";
        }

        private void btnSayi6_Click(object sender, EventArgs e)
        {
            txtPassword.Text = txtPassword.Text + "6";
        }

        private void btnSayi7_Click(object sender, EventArgs e)
        {
            txtPassword.Text = txtPassword.Text + "7";
        }

        private void btnSayi8_Click(object sender, EventArgs e)
        {
            txtPassword.Text = txtPassword.Text + "8";
        }

        private void btnSayi9_Click(object sender, EventArgs e)
        {
            txtPassword.Text = txtPassword.Text + "9";
        }

        private void btnSayi0_Click(object sender, EventArgs e)
        {
            txtPassword.Text = txtPassword.Text + "0";
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            
            if (txtPassword.Text.Length > 6)
            {
                MessageBox.Show("Şifre 6 karakterden büyük olamaz");
                txtPassword.Text = "";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            getTerminalCards();
        }
    }
}
