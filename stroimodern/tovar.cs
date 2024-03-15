using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static stroimodern.AuthenticationManager;

namespace stroimodern
{
    public partial class tovar : Form
    {
        private User currentUser;
        private NpgsqlDataAdapter dataAdapter;
        private DataTable dataTable; private string userRole;
        private Form1 previousForm;
        private string _role;
        private Form1 _form1;
        private DataTable originalDataTable;
        private string connectionString = "Host=localhost;Username=postgres;Password=1111;Database=stroimodern";

        public tovar(string role, Form1 form1)
        {
            InitializeComponent();
            _role = role;
            _form1 = form1;
        }

        public void UpdateUsernameLabel(string username)
        {
            Label lblUsername = Controls.OfType<Label>().FirstOrDefault();
            if (lblUsername != null)
            {
                lblUsername.Text = username;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Получение значения для поиска
            string searchValue = textBox1.Text.Trim();

            // Проверка наличия данных в DataGridView
            if (dataGridView1.DataSource == null)
                return;

            // Фильтрация данных в режиме реального времени
            ((DataTable)dataGridView1.DataSource).DefaultView.RowFilter = $"Название LIKE '%{searchValue}%'";
        }

        private void FillComboBox()
        {
            comboBox1.Items.AddRange(new object[] { "все", "обои", "двери", "фрески", "мебель", "другое" });
            comboBox1.SelectedIndex = 0; // Выберите значение по умолчанию
        }

        private void comboBoxTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string selectedType = comboBox1.SelectedItem.ToString().ToLower();

            // Проверка наличия данных в DataGridView
            if (dataGridView1.DataSource == null)
                return;

            // Определение фильтра для типа
            string typeFilter = (selectedType == "все") ? "" : $"тип = '{selectedType}'";

            // Фильтрация данных в режиме реального времени
            ((DataTable)dataGridView1.DataSource).DefaultView.RowFilter = typeFilter;
        }

        private void tovar_Load(object sender, EventArgs e)
        {
            if (label1.Text.Trim().Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                button2.Enabled = true; // Доступ к кнопке для администратора
            }
            else
            {
                button2.Enabled = false; // Отключаем кнопку для других пользователей
            }
            DataVivod();
        }

        public class TovarFormLogic
        {
            public bool IsAdminLabel(string labelText)
            {
                return labelText.Trim().Equals("admin", StringComparison.OrdinalIgnoreCase);
            }
        }

        private void DataVivod()
        {
            string connectionString = "Host=localhost;Username=postgres;Password=1111;Database=stroimodern";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT название, стоимость, артикул, фото, количество FROM товар";

                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(selectQuery, connection))
                    {
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        // Изменяем размеры изображений
                        foreach (DataRow row in dataTable.Rows)
                        {
                            byte[] imageData = (byte[])row["фото"];
                            Image originalImage = ByteArrayToImage(imageData);

                            // Уменьшаем размер изображения
                            int desiredWidth = 100; // Замените на ваше желаемое значение ширины
                            int desiredHeight = 100; // Замените на ваше желаемое значение высоты
                            Image resizedImage = ResizeImage(originalImage, desiredWidth, desiredHeight);

                            // Сохраняем уменьшенное изображение обратно в DataTable
                            row["фото"] = ImageToByteArray(resizedImage);
                        }

                        // Привязываем DataTable с измененными данными к DataGridView
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            dataGridView1.RowTemplate.Height = 100; // Замените 50 на нужную вам высоту

            // Применяем высоту ко всем строкам, если она одинакова для всех
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = dataGridView1.RowTemplate.Height;
            }
            textBox1.TextChanged += TxtSearch_TextChanged;
            FillComboBox();
        }

        // Метод для изменения размера изображения
        private Image ResizeImage(Image image, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(image, 0, 0, width, height);
            }
            return result;
        }

        // Метод для преобразования массива байт в изображение
        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        // Метод для преобразования изображения в массив байт
        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); // Используйте нужный формат
                return ms.ToArray();
            }
        }

        private string type;
        private string role;
        private Form1 form1;

        private void FilterByType()
        {
            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=1111;Database=stroimodern;";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM товар";

                if (!string.IsNullOrEmpty(type))
                {
                    // Если выбран конкретный тип, добавляем условие WHERE
                    query += " WHERE тип = @Type";
                }

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(type))
                    {
                        // Передаем значение параметра для фильтрации по типу
                        command.Parameters.AddWithValue("@Type", type);
                    }

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        foreach (DataRow row in dataTable.Rows)
                        {
                            byte[] imageData = (byte[])row["фото"];
                            Image originalImage = ByteArrayToImage(imageData);

                            // Уменьшаем размер изображения
                            int desiredWidth = 100; // Замените на ваше желаемое значение ширины
                            int desiredHeight = 100; // Замените на ваше желаемое значение высоты
                            Image resizedImage = ResizeImage(originalImage, desiredWidth, desiredHeight);

                            // Сохраняем уменьшенное изображение обратно в DataTable
                            row["фото"] = ImageToByteArray(resizedImage);
                        }
                        dataGridView1.DataSource = dataTable;
                        dataGridView1.RowTemplate.Height = 100; // Замените 50 на нужную вам высоту

                        // Применяем высоту ко всем строкам, если она одинакова для всех
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            row.Height = dataGridView1.RowTemplate.Height;
                        }
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "все")
            {
                type = null;
            }
            else
            {
                // В противном случае, устанавливаем фильтр в выбранный тип
                type = comboBox1.SelectedItem.ToString();
            }

            // Вызываем метод фильтрации
            FilterByType();
        }

        private void StMax()
        {
            string connectionString = "Host=localhost;Username=postgres;Password=1111;Database=stroimodern";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Выборка данных с сортировкой по столбцу "стоимость" в порядке убывания
                string selectQuery = "SELECT * FROM товар ORDER BY стоимость DESC";

                using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(selectQuery, connection))
                {
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        byte[] imageData = (byte[])row["фото"];
                        Image originalImage = ByteArrayToImage(imageData);

                        // Уменьшаем размер изображения
                        int desiredWidth = 100; // Замените на ваше желаемое значение ширины
                        int desiredHeight = 100; // Замените на ваше желаемое значение высоты
                        Image resizedImage = ResizeImage(originalImage, desiredWidth, desiredHeight);

                        // Сохраняем уменьшенное изображение обратно в DataTable
                        row["фото"] = ImageToByteArray(resizedImage);
                    }
                    dataGridView1.DataSource = dataTable;
                    dataGridView1.RowTemplate.Height = 100; // Замените 50 на нужную вам высоту

                    // Применяем высоту ко всем строкам, если она одинакова для всех
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        row.Height = dataGridView1.RowTemplate.Height;
                    }
                }
            }
        }

        private void StMin()
        {
            string connectionString = "Host=localhost;Username=postgres;Password=1111;Database=stroimodern";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Выборка данных с сортировкой по столбцу "стоимость" в порядке убывания
                string selectQuery = "SELECT * FROM товар ORDER BY стоимость ASC";

                using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(selectQuery, connection))
                {
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        byte[] imageData = (byte[])row["фото"];
                        Image originalImage = ByteArrayToImage(imageData);

                        // Уменьшаем размер изображения
                        int desiredWidth = 100; // Замените на ваше желаемое значение ширины
                        int desiredHeight = 100; // Замените на ваше желаемое значение высоты
                        Image resizedImage = ResizeImage(originalImage, desiredWidth, desiredHeight);

                        // Сохраняем уменьшенное изображение обратно в DataTable
                        row["фото"] = ImageToByteArray(resizedImage);
                    }
                    dataGridView1.DataSource = dataTable;
                    dataGridView1.RowTemplate.Height = 100; // Замените 50 на нужную вам высоту

                    // Применяем высоту ко всем строкам, если она одинакова для всех
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        row.Height = dataGridView1.RowTemplate.Height;
                    }
                }
            }
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedOption = comboBox2.SelectedItem.ToString();
            switch (selectedOption)
            {
                case "стоимость минимальная":
                    StMin();
                    break;

                case "стоимость максимальная":
                    StMax();
                    break;

                case "всё":
                    DataVivod();
                    break;
            }
        }

        private Bitmap DrawBarcode(string code, int resolution = 20) // resolution - пикселей на миллиметр
        {

            int numberCount = 8;

            float height = 25.93f * resolution;
            float lineHeight = 22.85f * resolution;
            float leftOffset = 3.63f * resolution;
            float rightOffset = 2.31f * resolution;
            float longLineHeight = lineHeight + 1.65f * resolution;
            float fontHeight = 2.75f * resolution;
            float lineToFontOffset = 0.165f * resolution;
            float lineWidthDelta = 0.15f * resolution;
            float lineWidthFull = 1.35f * resolution;
            float lineOffset = 0.2f * resolution;
            float width = leftOffset + rightOffset + 6 * (lineWidthDelta + lineOffset) + numberCount * (lineWidthFull + lineOffset);

            Bitmap bitmap = new Bitmap((int)width, (int)height);
            Graphics g = Graphics.FromImage(bitmap);
            Font font = new Font("Arial", fontHeight, FontStyle.Regular, GraphicsUnit.Pixel);
            StringFormat fontFormat = new StringFormat();
            fontFormat.Alignment = StringAlignment.Center;
            fontFormat.LineAlignment = StringAlignment.Center;
            float x = leftOffset;

            for (int i = 0; i < numberCount; i++)
            {
                int number = Convert.ToInt32(code[i].ToString());
                if (number != 0)
                {
                    g.FillRectangle(Brushes.Black, x, 0, number * lineWidthDelta, lineHeight);
                }
                RectangleF fontRect = new RectangleF(x, lineHeight + lineToFontOffset, lineWidthFull, fontHeight);
                g.DrawString(code[i].ToString(), font, Brushes.Black, fontRect, fontFormat);
                x += lineWidthFull + lineOffset;

                if (i == 0 || i == numberCount / 2 || i == numberCount - 1)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        g.FillRectangle(Brushes.Black, x, 0, lineWidthDelta, longLineHeight);
                        x += lineWidthDelta + lineOffset;
                    }
                }
            }
            return bitmap;
        }

        private void vava(string article)
        {
            Bitmap bitmap = DrawBarcode(article);

            using (PrintDocument printDocument = new PrintDocument())
            {
                printDocument.PrintPage += (s, e) =>
                {
                    RectangleF imageRect = new RectangleF(100, 100, 200, 150);
                    e.Graphics.DrawImage(bitmap, imageRect);
                    e.HasMorePages = false;
                };
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Получаем значение артикула из выбранной строки и столбца "Артикул"
                string article = dataGridView1.Rows[e.RowIndex].Cells["артикул"].Value.ToString();

                // Печатаем штрих-код по полученному артикулу
                vava(article);
            }
        }

        public void InsertOrder(string fio, string address, string products, decimal cost)
        {
            string connectionString = "Host=localhost;Username=postgres;Password=1111;Database=stroimodern";
            string query = "INSERT INTO заказы (ФИО, Адрес, Товары, Стоимость) VALUES (@Fio, @Address, @Products, @Cost)";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Fio", fio);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Products", products);
                    command.Parameters.AddWithValue("@Cost", cost);

                    command.ExecuteNonQuery();
                }
            }
        }

        private decimal CalculateTotalCost(List<string> selectedProducts)
        {
            decimal totalCost = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Selected) // Проверяем, выбран ли товар
                {
                    // Получаем цену товара из соответствующей ячейки в вашем DataGridView
                    if (decimal.TryParse(row.Cells["стоимость"].Value.ToString(), out decimal price))
                    {
                        totalCost += price;
                    }
                }
            }

            return totalCost;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fio = textBox2.Text;
            string address = textBox3.Text;

            // Формируем список выбранных товаров из датагрида с их ценой
            List<string> selectedProducts = new List<string>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                // Получаем название товара и его цену из соответствующих ячеек в DataGridView
                string productName = row.Cells["название"].Value.ToString();
                decimal productPrice = Convert.ToDecimal(row.Cells["стоимость"].Value);

                // Формируем строку товара с указанием его названия и цены
                string productWithPrice = $"{productName} ({productPrice:C})";

                selectedProducts.Add(productWithPrice);
            }
            string products = string.Join(", ", selectedProducts);

            // Вычисляем общую стоимость выбранных товаров
            decimal cost = CalculateTotalCost(selectedProducts);

            // Добавляем заказ в базу данных
            InsertOrder(fio, address, products, cost);

            zakaz zakaz = new zakaz();
            zakaz.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
                admins admins = new admins();
                admins.Show();
                this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Rassp();
        }

        private void Rassp()
        {
            // Настройки SMTP-сервера Mail.ru
            string smtpServer = "smtp.mail.ru"; //smpt сервер(зависит от почты отправителя)
            int smtpPort = 587; // Обычно используется порт 587 для TLS
            string smtpUsername = "cmoqq1@mail.ru"; //твоя почта, с которой отправляется сообщение
            string smtpPassword = "pBXRtYvUKY9YUykQEUX5";//пароль приложения (от почты)

            // Создаем объект клиента SMTP
            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                // Настройки аутентификации
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add("mstepanischeva@yandex.ru"); // Укажите адрес получателя
                    mailMessage.Subject = "Компания СТРОЙМОДЕРН";
                    mailMessage.Body = $"Уважаемый клиент! Только 24 февраля вся продукция будет со скидкой" +
                        $" – 20%, при указании кодового слова «Дэмоэкзамен 2023».";

                    try
                    {
                        // Отправляем сообщение
                        smtpClient.Send(mailMessage);
                        Console.WriteLine("Сообщение успешно отправлено.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
                    }
                }
            }        
        }
    }
}
