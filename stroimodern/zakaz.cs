using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using System.IO;
using Aspose.Pdf;
using Aspose.Pdf.Text;

namespace stroimodern
{
    public partial class zakaz : Form
    {
        public zakaz()
        {
            InitializeComponent();
        }

        private void zakaz_Load(object sender, EventArgs e)
        {
            string connectionString = "Host=localhost;Username=postgres;Password=1111;Database=stroimodern";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT ФИО, Адрес, Товары, Стоимость FROM заказы";

                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(selectQuery, connection))
                    {
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
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
            string fio = dataGridView1.SelectedRows[0].Cells["ФИО"].Value.ToString();
            string address = dataGridView1.SelectedRows[0].Cells["Адрес"].Value.ToString();

            // Получаем товары и стоимость из выбранной строки в датагриде
            string products = dataGridView1.SelectedRows[0].Cells["Товары"].Value.ToString();
            decimal cost = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["Стоимость"].Value);

            // Создаем новый документ PDF
            Document pdfDocument = new Document();

            // Создаем страницу PDF
            Aspose.Pdf.Page page = pdfDocument.Pages.Add();

            // Создаем объект TextFragment для добавления текста
            TextFragment textFragment = new TextFragment();
            textFragment.Text = $"Компания ООО \"СтройМодерн\" Чек\n\n" +
                                 $"ФИО: {fio}\n" +
                                 $"Адрес: {address}\n" +
                                 $"Товары: {products}\n" +
                                 $"Итоговая стоимость: {cost}\n";

            // Настраиваем форматирование текста
            textFragment.TextState.FontSize = 12;

            // Добавляем текст на страницу
            page.Paragraphs.Add(textFragment);

            // Сохраняем документ PDF
            string pdfFilePath = "invoice.pdf";
            pdfDocument.Save(pdfFilePath);

            // Открываем сохраненный PDF-файл
            Process.Start(pdfFilePath);

        }       
    }
}
