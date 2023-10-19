using System.Net;

namespace Client_for_FTP
{
    public partial class Form1 : Form
    {
        public string? password = "";
        public string? username = "";
        public Form1()
        {
            InitializeComponent();
        }
        public static class SharedData
        {
            public static string? SharedUsername { get; set; }
            public static string? SharedPassword { get; set; }
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

            public bool TestConnection()
            {
                try
                {
                    // Try to establish a connection by requesting a response from the server.
                    var req = CreateRequest("/", WebRequestMethods.Ftp.ListDirectory);
                    var resp = req.GetResponse();
                    resp.Close();
                    return true;
                }
                catch (WebException ex)
                {
                    // Handle the exception or log the error as needed.
                    Console.WriteLine("FTP Connection Error: " + ex.Message);
                    return false;
                }
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            username = textBox1.Text;
            SharedData.SharedUsername = username;
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            password = textBox2.Text;
            SharedData.SharedPassword = password;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text == "") || (textBox2.Text == ""))
            { MessageBox.Show("You need to enter username and password."); }
            FTP ftpClient = new FTP("ftp://192.168.100.9", username, password);
            if (ftpClient.TestConnection())
            {
                MessageBox.Show("Connected to the FTP server.");
                main obj = new main();
                SharedData.SharedUsername = username;
                SharedData.SharedPassword = password;
                obj.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Failed to connect to the FTP server.");
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}