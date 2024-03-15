using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stroimodern
{
    public partial class CaptchaForm : Form
    {
        private string captcha;
        public string EnteredCaptcha { get; private set; }
        public CaptchaForm()
        {
            InitializeComponent();
        }

        private void GenerateCaptcha()
        {
            Random random = new Random();
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numbers = "0123456789";
            captcha = new string(Enumerable.Repeat(letters, 5)
                                          .Select(s => s[random.Next(s.Length)])
                                          .Concat(Enumerable.Repeat(numbers, 5)
                                                          .Select(s => s[random.Next(s.Length)]))
                                          .ToArray());

            label1.Text = captcha;
        }

        public bool ValidateCaptcha(string enteredCaptcha)
        {
            return string.Equals(enteredCaptcha, captcha, StringComparison.OrdinalIgnoreCase);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EnteredCaptcha = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CaptchaForm_Load(object sender, EventArgs e)
        {
            GenerateCaptcha();
        }
    }
}
