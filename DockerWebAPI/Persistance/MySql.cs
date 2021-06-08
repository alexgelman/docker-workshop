using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace DockerWebAPI.Persistance
{
    public class MySql : IDisposable
    {
        private readonly MySqlConnection connection;

        public MySql()
            : this("server=localhost;user=root;database=world;port=3306;password=******")
        {
        }

        public MySql(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }

        public void CreateSchema(string schemaName)
        {
            ExecuteCommand($"CREATE DATABASE IF NOT EXISTS {schemaName} DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci");
            connection.ChangeDatabase(schemaName);
        }

        public void CreateTable(string schemaName, string tableName, IEnumerable<(string columnName, string columnType)> columns)
        {
            var columnDefinitions = string.Join(", ", columns.Select(col => $"{col.columnName} {col.columnType}"));
            Console.WriteLine(columnDefinitions);
            ExecuteCommand($"CREATE TABLE IF NOT EXISTS {schemaName}.{tableName}({columnDefinitions});");
        }

        public IEnumerable<IDataRecord> ExecuteRead(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    yield return rdr;
                }
            }
        }

        public void ExecuteCommand(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}