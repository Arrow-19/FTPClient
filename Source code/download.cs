using System.Net;

namespace Client_for_FTP
{
    public partial class download : Form
    {
        bool initialOpening = true;
        string? browse = "";
        string? toDownload = "";
        public download()
        {
            InitializeComponent();
            string? username = Form1.SharedData.SharedUsername;
            string? password = Form1.SharedData.SharedPassword;
            FTP ftpClient = new FTP("ftp://192.168.100.9", username, password);
            List<string> fileList = ftpClient.GetFiles("/");
            if (fileList != null)
            {
                Console.WriteLine("Files in the directory:");
                foreach (string file in fileList)
                {
                    if (file.Contains("."))
                    {
                        ListViewItem item = new ListViewItem(file);
                        listView1.Items.Add(item);
                    }
                    else
                    {
                        ListViewItem item = new ListViewItem(file + "(folder)");
                        listView1.Items.Add(item);
                    }
                }
            }
            else
            {
                Console.WriteLine("Failed to list files in the directory.");
            }
        }

        public class FTP
        {
            private string _host;
            private NetworkCredential _credentials;
            private WebClient _wc;
            public FTP(string host, string username, string password)
            {
                _host = host;
                _credentials = new(username, password);
                _wc = new()
                {
                    BaseAddress = _host,
                    Credentials = _credentials
                };
            }
            private WebRequest CreateRequest(string path, string method)
            {
                var req = WebRequest.Create(_host + path);
                req.Method = method;
                req.Credentials = _credentials;
                return req;
            }
            public List<string> GetFiles(string remoteDir)
            {
                try
                {
                    var req = CreateRequest(remoteDir, WebRequestMethods.Ftp.ListDirectory);
                    var resp = req.GetResponse();
                    var sr = new StreamReader(resp.GetResponseStream());
                    var fileList = new List<string>();

                    string fileName;
                    while ((fileName = sr.ReadLine()) != null)
                    {
                        fileList.Add(fileName);
                    }

                    sr.Close();

                    return fileList;
                }
                catch
                {
                    return null;
                }
            }
            public bool DownloadFile(string remoteFile, string localFile)
            {
                try
                {
                    _wc.DownloadFile(remoteFile, localFile);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            toDownload = textBox1.Text;
            string? username = Form1.SharedData.SharedUsername;
            string? password = Form1.SharedData.SharedPassword;
            FTP ftpClient = new FTP("ftp://192.168.100.9", username, password);
            if (browse != "")
            {
                bool downloadResult = ftpClient.DownloadFile("/" + browse + "/" + toDownload, $"D://" + toDownload);
                if (downloadResult)
                {
                    MessageBox.Show("File downloaded successfully.");
                }
                else
                {
                    MessageBox.Show("File download failed. Entered file does not exist.");
                }
            }
            else
            {

                bool downloadResult = ftpClient.DownloadFile("/" + toDownload, $"D:\\" + toDownload);
                if (downloadResult)
                {
                    MessageBox.Show("File downloaded successfully.");
                }
                else
                {
                    MessageBox.Show("File download failed. Entered file does not exist.");
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            main obj = new main();
            obj.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                initialOpening = true;
            }
            if (initialOpening == true)
            {
                browse = textBox2.Text;
            }
            else
            {
                browse = browse + "/" + textBox2.Text;
                //MessageBox.Show($"Value of browse {browse}");
            }

            listView1.Items.Clear();
            string? username = Form1.SharedData.SharedUsername;
            string? password = Form1.SharedData.SharedPassword;
            FTP ftpClient = new FTP("ftp://192.168.100.9", username, password);
            if (browse == "root" || browse == "")
            {
                initialOpening = true;
                List<string> fileList = ftpClient.GetFiles("/");
                if (fileList != null)
                {
                    ListViewItem header = new ListViewItem("File contents present in root" + "---------------------");
                    listView1.Items.Add(header);
                    Console.WriteLine("Files in the directory:");
                    foreach (string file in fileList)
                    {
                        if (file.Contains("."))
                        {
                            ListViewItem item = new ListViewItem(file);
                            listView1.Items.Add(item);
                        }
                        else
                        {
                            ListViewItem item = new ListViewItem(file + "(folder)");
                            listView1.Items.Add(item);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Failed to list files in the directory./nPlease enter directory name precisely.");
                }
            }
            else
            {
                initialOpening = false;
                List<string> fileList = ftpClient.GetFiles("/" + browse);
                //MessageBox.Show($"The value of browse is {browse}");
                if (fileList != null)
                {
                    ListViewItem header = new ListViewItem("File contents present in " + browse + "---------------------");
                    listView1.Items.Add(header);
                    Console.WriteLine("Files in the directory:");
                    foreach (string file in fileList)
                    {
                        if (file.Contains("."))
                        {
                            int index = file.IndexOf("/");
                            string name = file.Substring(index + 1);
                            ListViewItem item = new ListViewItem(name);
                            listView1.Items.Add(item);
                        }
                        else
                        {
                            int index = file.IndexOf("/");
                            string name = file.Substring(index + 1);
                            ListViewItem item = new ListViewItem(name + "(folder)");
                            listView1.Items.Add(item);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Failed to list files in the directory./nPlease enter directory name precisely.");
                }
            }
            textBox2.Text = "";
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
