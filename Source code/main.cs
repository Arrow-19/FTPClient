using System.Net;


namespace Client_for_FTP
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
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
            public bool UploadFile(string localFilePath, string remoteFilePath)
            {
                try
                {
                    // Use UploadFile method to upload a file to the FTP server
                    _wc.UploadFile(remoteFilePath, localFilePath);
                    return true;
                }
                catch
                {
                    return false;
                }
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
        }
        private void button2_Click(object sender, EventArgs e)
        {
            download obj = new download();
            obj.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            createDir obj = new createDir();
            obj.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
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

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string? username = Form1.SharedData.SharedUsername;
            string? password = Form1.SharedData.SharedPassword;
            FTP ftpClient = new FTP("ftp://192.168.100.9", username, password);
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a file to upload",
                Filter = "All Files|*.*",
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string localFilePath = openFileDialog.FileName;

                string remoteFilePath = "/Uploads/" + openFileDialog.SafeFileName;
                bool uploadResult = ftpClient.UploadFile(localFilePath, remoteFilePath);
                if (uploadResult)
                {
                    MessageBox.Show("File upload was successful!");
                }
                else
                {
                    MessageBox.Show("File upload was unsuccessful!");
                }
            }
        }
    }
}
