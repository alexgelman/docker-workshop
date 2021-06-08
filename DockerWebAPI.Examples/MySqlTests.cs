using System;
using System.Linq;using DockerWebAPI.Persistance;
using Xunit;

namespace DockerWebAPI.Examples
{
    public class MySqlTests
    {
        private const string DatabaseAddress = "10.0.117.93";
        private const string DatabasePort = "3306";
        private const string DatabaseUser = "root";
        private const string DatabasePassword = "a";

        private readonly string ConnectionString = $"server={DatabaseAddress};user={DatabaseUser};port={DatabasePort};password={DatabasePassword}";


        [Fact]
        public void ConnectToDataBase()
        {
            using (var mysql = new DockerWebAPI.Persistance.MySql(ConnectionString))
            {
            }
        }

        [Fact]
        public void CreateSchema()
        {
            using (var mysql = new DockerWebAPI.Persistance.MySql(ConnectionString))
            {
                mysql.CreateSchema("DockerTest_Schema");
            }
        }

        [Fact]
        public void CreateSchemaTable()
        {
            using (var mysql = new DockerWebAPI.Persistance.MySql(ConnectionString))
            {
                var SchemaName = "DockerTest_Table";
                mysql.CreateSchema(SchemaName);
                mysql.CreateTable(SchemaName, "Test_Table", new [] {("ID", "INT AUTO_INCREMENT UNIQUE PRIMARY KEY"), ("data", "JSON")});
            }
        }

        [Fact]
        public void WriteObject()
        {
            using (var mysql = new DockerWebAPI.Persistance.MySql(ConnectionString))
            {
                var SchemaName = "DockerTest_Write";
                var TableName = "Write_Table";
                mysql.CreateSchema(SchemaName);
                mysql.CreateTable(SchemaName, TableName, new [] {("ID", "INT AUTO_INCREMENT UNIQUE PRIMARY KEY"), ("data", "JSON")});

                mysql.ExecuteCommand($"INSERT INTO {TableName}(data) VALUES('{{\"name\":\"alex\"}}')");
            }
        }

        [Fact]
        public void ReadObject()
        {
            using (var mysql = new DockerWebAPI.Persistance.MySql(ConnectionString))
            {
                var schemaName = "DockerTest_Read";
                var tableName = "Read_Table";
                var expectedName = "Alex";
                mysql.CreateSchema(schemaName);
                mysql.CreateTable(schemaName, tableName, new [] {("ID", "INT AUTO_INCREMENT UNIQUE PRIMARY KEY"), ("name", "TEXT")});

                mysql.ExecuteCommand($"INSERT INTO {tableName}(name) VALUES('{expectedName}')");

                var readResults = mysql.ExecuteRead($"SELECT * FROM {tableName}");
                foreach (var row in readResults)
                {
                    var actualName = row.GetString(row.GetOrdinal("name"));
                    Assert.Equal(expectedName, actualName);
                }
            }
        }
    }
}
