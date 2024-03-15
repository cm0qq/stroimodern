using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using stroimodern;
using Npgsql;
using System.Data;
using static stroimodern.tovar;
using System.Collections.Generic;
using static stroimodern.AuthenticationManager;
using System.Linq;

namespace stroimoderntests
{
    [TestClass]
    public class stroimoderntest
    {
        private AuthenticationManager _authenticationManager;

        [TestInitialize]
        public void Setup()
        {
            _authenticationManager = new AuthenticationManager("Host=localhost;Username=postgres;Password=1111;Database=stroimodern");
        }

        [TestMethod]
        public void AuthenticateUser_ValidCredentials_ReturnsUserObject()
        {
            string username = "test";
            string password = "test";
            var user = _authenticationManager.AuthenticateUser(username, password);
            Assert.IsNotNull(user);
            Assert.AreEqual(username, user.Username);
        }

        [TestMethod]
        public void IsAdminLabel_ShouldReturnFalse_WhenLabelTextIsNotAdmin()
        {
            // Arrange
            var logic = new TovarFormLogic();
            string nonAdminLabelText = "user";

            // Act
            bool isAdmin = logic.IsAdminLabel(nonAdminLabelText);

            // Assert
            Assert.IsFalse(isAdmin);
        }

        [TestMethod]
        public void TestDatabaseConnection()
        {
            // Arrange
            string connectionString = "Host=localhost;Username=postgres;Password=1111;Database=stroimodern";

            // Act
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Assert
                    Assert.AreEqual(ConnectionState.Open, connection.State);
                }
                catch (Exception ex)
                {
                    // Assert
                    Assert.Fail($"Failed to connect to the database. Error: {ex.Message}");
                }
            }
        }

        [TestMethod]
        public void TestIsAdminLabel_AdminLabel_ReturnsTrue()
        {
            // Arrange
            string adminLabelText = "admin";
            tovar.TovarFormLogic logic = new tovar.TovarFormLogic();

            // Act
            bool isAdminLabel = logic.IsAdminLabel(adminLabelText);

            // Assert
            Assert.IsTrue(isAdminLabel);
        }

        private string connectionString = "Host=localhost;Username=postgres;Password=1111;Database=stroimodern";

        [TestMethod]
        public void TestInsertUser_InsertsUserCorrectly()
        {
            // Arrange
            string username = "testuser";
            string password = "testpassword";
            string id_r = "1"; // Предполагается, что это ID роли пользователя
            int rold = int.Parse(id_r);
            var originalRowCount = GetRowCount("пользователи"); // Получаем исходное количество записей в таблице пользователей

            var adminsForm = new admins(); // Создаем форму для добавления пользователя

            // Заполнение текстовых полей для ввода данных
            adminsForm.textBox1.Text = id_r;
            adminsForm.textBox2.Text = username;
            adminsForm.textBox3.Text = password;

            // Act
            adminsForm.dobAv();

            var newRowCount = GetRowCount("пользователи"); // Получаем новое количество записей в таблице пользователей

            // Assert
            Assert.AreEqual(originalRowCount + 1, newRowCount, "User was not inserted correctly.");
        }


        private int GetRowCount(string tableName)
        {
            int rowCount = 0;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand($"SELECT COUNT(*) FROM {tableName}", connection);
                object result = command.ExecuteScalar();

                rowCount = Convert.ToInt32(result);
            }

            return rowCount;
        }

    }
}
