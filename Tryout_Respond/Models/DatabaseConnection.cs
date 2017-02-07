using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Tryout_Respond
{
    public class DatabaseConnection
    {
        private SqlConnection connection;

        public DatabaseConnection()
        {
            connection = new SqlConnection(GetConnectionString());
        }

        static private string GetConnectionString()
        {
            return "Data Source=(local);Initial Catalog=Respond_TryOut_Database;Integrated Security=SSPI;";
        }

        /*private string CreateSelectQuery(string column, string table)
        {
            return "SELECT " + column + " FROM " + table;
        }

        private string CreateSelectQuery(string[] columns, string table)
        {
            String query = "SELECT ";

            foreach (string column in columns)
            {
                query += column;

                if (column != columns[columns.Count() - 1])
                {
                    query += ", ";
                }
            }

            return query += " FROM " + table;
        }

        private string AddWhereClausToQuery(string valueToCompare, string comparator, string requiredValue, string query)
        {
            if (comparator != "=" && comparator != "!=" && comparator != "<" && comparator != ">" && comparator != "<=" && comparator != ">=")
            {
                throw new ArgumentException("operator: " + comparator + "is not a valid operator");
            }

            return query += " WHERE " + valueToCompare + " " + comparator + " " + requiredValue;
        }*/

        public void RunNonQuery(string query)
        {
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            SqlTransaction transaction;

            transaction = connection.BeginTransaction();

            command.Connection = connection;
            command.Transaction = transaction;

            try
            {
                command.CommandText = query;
                command.ExecuteNonQuery();

                transaction.Commit();
                Console.WriteLine("query: " + query + "succesfully executed");
            }
            catch(Exception ex)
            {
                Console.WriteLine("failed to execute query: " + query);

                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("failed to rollback");
                }
            }

            connection.Close();
        }

        public List<object[]> RunQuery(string query)
        {
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            SqlTransaction transaction;

            transaction = connection.BeginTransaction();

            command.Connection = connection;
            command.Transaction = transaction;

            List<object[]> result = new List<object[]>();

            try
            {
                command.CommandText = query;
                SqlDataReader reader = command.ExecuteReader();

                while(reader.Read())
                {
                    object[] row = new object[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader.GetValue(i);
                    }

                    result.Add(row);
                }

                reader.Close();

                transaction.Commit();
                Console.WriteLine("query: " + query + "succesfully executed");
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to execute query: " + query);

                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("failed to rollback");
                }
            }

            connection.Close();

            return result;
        }
    }
}