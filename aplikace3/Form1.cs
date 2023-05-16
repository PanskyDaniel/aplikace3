using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.IO;

namespace Aplikace3
{
    public partial class Form1 : Form
    {
        private const string nazevSouboru = "pokus.ini";
        private string soubor;
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            string exe = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string cesta = Path.GetDirectoryName(exe);
            soubor = Path.Combine(cesta, nazevSouboru);

            if (!File.Exists(soubor))
            {
                try
                {
                    File.Create(soubor);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Chyba při vytvoření souboru", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }

            IniFile ini = new IniFile(soubor);

            // Reading from INI file
            SMTP.Text = ini.Read("SMTP", "Sekce1");
            Port.Text = ini.Read("PORT", "Sekce1");
            SSL.Checked = ini.Read("SSL", "Sekce1") == "True";

            Odesilatel.Text = ini.Read("Odesilatel", "Sekce2");


        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SMTP.Text))
            {
                MessageBox.Show("Hodnota SMPT musí být vyplněna!");
                return;
            }
            if (string.IsNullOrEmpty(Port.Text))
            {
                MessageBox.Show("Port musí být vyplněn!");
                return;
            }

            int test = 0;
            if (!Int32.TryParse(Port.Text, out test))
            {
                MessageBox.Show("Port musí číslo!");
                return;
            }

            if (string.IsNullOrEmpty(Odesilatel.Text))
            {
                MessageBox.Show("Odesílatel musí být vyplněn!");
                return;
            }

            if (!IsValidEmail(Odesilatel.Text))
            {
                MessageBox.Show("Neplatný e-mail odesílatele!");
                return;
            }

            IniFile ini = new IniFile(soubor);

            // Writing to INI file
            ini.Write("SMTP", SMTP.Text, "Sekce1");
            ini.Write("PORT", Port.Text, "Sekce1");
            ini.Write("SSL", SSL.Checked.ToString(), "Sekce1");
            ini.Write("Odesilatel", Odesilatel.Text, "Sekce2");

            MessageBox.Show("Nastavení bylo uloženo", "Nastavení bylo uloženo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void button2_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(SMTP.Text))
            {
                MessageBox.Show("Hodnota SMPT musí být vyplněna!");
                return;
            }
            if (string.IsNullOrEmpty(Port.Text))
            {
                MessageBox.Show("Port musí být vyplněn!");
                return;
            }

            int test = 0;
            if (!Int32.TryParse(Port.Text, out test))
            {
                MessageBox.Show("Port musí číslo!");
                return;
            }

            if (string.IsNullOrEmpty(Odesilatel.Text))
            {
                MessageBox.Show("Odesílatel musí být vyplněn!");
                return;
            }

            if (!IsValidEmail(Odesilatel.Text))
            {
                MessageBox.Show("Neplatný e-mail odesílatele!");
                return;
            }

            if (string.IsNullOrEmpty(Prijemce.Text))
            {
                MessageBox.Show("Příjemce musí být vyplněn!");
                return;
            }

            if (!IsValidEmail(Prijemce.Text))
            {
                MessageBox.Show("Neplatný e-mail příjemce!");
                return;

            }

            if (string.IsNullOrEmpty(Predmet.Text))
            {
                MessageBox.Show("Předmět musí být vyplněn");
                return;
            }
            if (string.IsNullOrEmpty(TextZpravy.Text))
            {
                MessageBox.Show("Zpráva musí být vyplněna!");
                return;
            }


            try
            {
                MailMessage message = new MailMessage(Odesilatel.Text, Prijemce.Text, Predmet.Text, TextZpravy.Text);

                SmtpClient client = new SmtpClient(SMTP.Text, int.Parse(Port.Text));
                client.EnableSsl = SSL.Checked;
                client.Send(message);
                MessageBox.Show("E-mail byl odeslán", "E-mail byl odeslán", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při odeslání e-mailu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }

    class IniFile
    {
        private readonly string _path;

        public IniFile(string path)
        {
            _path = path;
        }

        public string Read(string key, string section = null)
        {
            var stringBuilder = new StringBuilder(255);
            GetPrivateProfileString(section ?? "", key, "", stringBuilder, 255, _path);
            return stringBuilder.ToString();
        }

        public void Write(string key, string value, string section = null)
        {
            WritePrivateProfileString(section ?? "", key, value, _path);
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string path);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder value, int size, string path);
    }
}
