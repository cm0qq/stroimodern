using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stroimodern
{
    public class MaterialManager
    {
        private string connectionString;

        public MaterialManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void AddMaterial(string name, decimal price, string articul, int quantity, string photo)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO товар (название, стоимость, артикул, количество, фото) VALUES (@наименование, @цена, @артикул, @количество, @фото)";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@наименование", name);
                    command.Parameters.AddWithValue("@цена", price);
                    command.Parameters.AddWithValue("@артикул", articul);
                    command.Parameters.AddWithValue("@количество", quantity);
                    command.Parameters.AddWithValue("@фото", photo);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateMaterial(int id, string name, decimal price, string articul, int quantity, string photo)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE товар SET название = @наименование, стоимость = @цена, артикул = @артикул, количество = @количество, фото = @фото WHERE id = @id";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@наименование", name);
                    command.Parameters.AddWithValue("@цена", price);
                    command.Parameters.AddWithValue("@артикул", articul);
                    command.Parameters.AddWithValue("@количество", quantity);
                    command.Parameters.AddWithValue("@фото", photo);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteMaterial(int id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE FROM товар WHERE id = @id";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
