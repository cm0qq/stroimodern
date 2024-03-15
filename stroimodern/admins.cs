using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static stroimodern.AuthenticationManager;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace stroimodern
{
    public partial class admins : Form
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=1111;Database=stroimodern";
        public admins()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void dobAv()
        {
            int rold = 0;
            if (comboBox1.SelectedIndex == 0) // Проверяем выбранное значение в комбобоксе
            {
                rold = 1; // если выбрано "администратор", устанавливаем значение 1
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                rold = 2; // если выбрано "менеджер", устанавливаем значение 2
            }

            string login = textBox2.Text;
            string password = textBox3.Text;
            string query = "INSERT INTO пользователи (id, id_r, логин, пароль)" +
                          $"VALUES (6, @number_z, @date_z, @slom_oborud)";

            try
            {
                // Устанавливаем соединение с базой данных
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    // Создаем команду для выполнения SQL-запроса
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        // Добавляем параметры в запрос
                        command.Parameters.AddWithValue("@number_z", rold);
                        command.Parameters.AddWithValue("@date_z", login);
                        command.Parameters.AddWithValue("@slom_oborud", password);

                        // Выполняем SQL-запрос
                        int rowsAffected = command.ExecuteNonQuery();

                        // Проверяем, были ли успешно добавлены данные
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные успешно добавлены в базу данных.");
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить данные в базу данных.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dobAv();
        }
    }
}
