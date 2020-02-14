using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace SQLConnection
{
    class Program
    {
        static string response;
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connString = config.GetConnectionString("DefaultConnection");

            do
            {
                Console.WriteLine("-------------------------------");
                var repo = new DepartmentRepository(connString);
                var departments = repo.GetAllDepartments();

                foreach (var dept in departments)
                {
                    Console.WriteLine(dept.Name);
                }

                Console.WriteLine("\n Above are the Departments at Best Buy");
                Console.WriteLine("\n Type 'ADD' to Add a Department \n Type 'UPDATE' to Update a Department's name \n Type 'DELETE' to Delete a Department \n Type 'EXIT' to Exit the program \n");
                response = Console.ReadLine().ToUpper();

                if (response == "ADD")
                {
                    Console.WriteLine("\n What is the name of the new department?");
                    var departmentName = Console.ReadLine();
                    CreateDepartment(departmentName);
                }

                if (response == "UPDATE")
                {
                    Console.WriteLine("\n What is the name of the department you want to update?");
                    var departmentName = Console.ReadLine();
                    Console.WriteLine($"\n What do you want to update {departmentName} to?");
                    var updateName = Console.ReadLine();
                    UpdateDepartment(departmentName, updateName);
                }

                if (response == "DELETE")
                {
                    Console.WriteLine("\nWhat is the name of the department you want to delete?");
                    var departmentName = Console.ReadLine();
                    Console.WriteLine($"\nOk, {departmentName} has been deleted.\n");
                    DeleteDepartment(departmentName);
                }

            } while (response != "EXIT");
        }

        static IEnumerable GetAllDepartments()
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = System.IO.File.ReadAllText("appsettings.json");

            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Name FROM Departments;";

            using (conn)
            {
                conn.Open();
                List<string> allDepartments = new List<string>();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read() == true)
                {
                    var currentDepartment = reader.GetString("Name");
                    allDepartments.Add(currentDepartment);
                }
                return allDepartments;
            }
        }
        static void CreateDepartment(string departmentName)
        {
            var connStr = System.IO.File.ReadAllText("appsettings.json");
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "INSERT INTO departments (Name) VALUES (@departmentName);";
                cmd.Parameters.AddWithValue("departmentName", departmentName);
                cmd.ExecuteNonQuery();
            }
        }

        static void UpdateDepartment(string departmentName, string updateName)
        {
            var connStr = System.IO.File.ReadAllText("appsettings.json");
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "UPDATE departments SET Name = @updateName WHERE Name = @departmentName";
                cmd.Parameters.AddWithValue("departmentName", departmentName);
                cmd.Parameters.AddWithValue("updateName", updateName);
                cmd.ExecuteNonQuery();
            }
        }

        static void DeleteDepartment(string departmentName)
        {
            var connStr = System.IO.File.ReadAllText("appsettings.json");
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "DELETE FROM departments WHERE Name = @departmentName";
                cmd.Parameters.AddWithValue("departmentName", departmentName);
                cmd.ExecuteNonQuery();
            }
        }
    }   
}
