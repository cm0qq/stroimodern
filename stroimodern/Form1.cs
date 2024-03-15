using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static stroimodern.AuthenticationManager;
using stroimodern;

namespace stroimodern
{
    public partial class Form1 : Form
    {
        private const int maxLoginAttempts = 3;
        private const int lockoutDurationSeconds = 30;
        private int loginAttempts = 0;
        private string connectionString = "Host=localhost;Username=postgres;Password=1111;Database=stroimodern";
        private CaptchaForm capcha;
        private int lockoutSecondsRemaining = 0;
        private User currentUser;

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;
            AuthenticationManager authManager = new AuthenticationManager(connectionString);
            currentUser = authManager.AuthenticateUser(username, password);

            if (currentUser != null)
            {
                // Открываем основную форму с учетом роли пользователя
                ShowtovarForRole(currentUser.Role, currentUser);
            }
            else
            {
                loginAttempts++;

                if (loginAttempts >= maxLoginAttempts)
                {
                    LockoutUser();
                }

                MessageBox.Show("Неверные учетные данные. Попробуйте снова.");
            }
        }

        private void ShowtovarForRole(string role, User user)
        {
            tovar tovar = new tovar(role, this);
            tovar.UpdateUsernameLabel(user.Username);
            tovar.Show();
            Hide();
        }


        private void LockoutUser()
        {
            lockoutSecondsRemaining = lockoutDurationSeconds;
            MessageBox.Show($"Вы ввели неверные данные {maxLoginAttempts} раза. Учетная запись заблокирована на {lockoutDurationSeconds} секунд.");

            // Запуск таймера для отсчета времени блокировки
            Timer lockoutTimer = new Timer();
            lockoutTimer.Interval = 1000; // таймер срабатывает каждую секунду
            lockoutTimer.Tick += (sender, args) =>
            {
                lockoutSecondsRemaining--;

                if (lockoutSecondsRemaining <= 0)
                {
                    loginAttempts = 0; // Сбрасываем счетчик попыток после снятия блокировки
                    lockoutTimer.Stop();
                }
            };
            lockoutTimer.Start();

            // Показываем капчу
            using (CaptchaForm captchaForm = new CaptchaForm())
            {
                if (captchaForm.ShowDialog() == DialogResult.OK)
                {
                    // В этом месте можно добавить дополнительную логику для проверки капчи
                    captchaForm.ValidateCaptcha(captchaForm.EnteredCaptcha);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
