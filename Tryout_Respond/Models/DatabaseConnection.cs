using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Tryout_Respond
{
    public class DatabaseConnection
    {
        public SqlConnection sqlConnection { get; set; }
        public static string CONNECTIONSTRING { get; set; }

        public DatabaseConnection()
        {
            CONNECTIONSTRING = "Data Source=(local);Initial Catalog=Respond_TryOut_Database;Integrated Security=SSPI;";
            sqlConnection = new SqlConnection(CONNECTIONSTRING);
        }

        public bool RunNonQuery(string query)
        {
            var success = false;

            sqlConnection.Open();

            SqlCommand command = sqlConnection.CreateCommand();
            SqlTransaction transaction;

            transaction = sqlConnection.BeginTransaction();

            command.Connection = sqlConnection;
            command.Transaction = transaction;

            try
            {
                command.CommandText = query;

                if (command.ExecuteNonQuery() > 0)
                {
                    success = true;
                }

                transaction.Commit();
                Console.WriteLine("query: " + query + "succesfully executed");
            }
            catch(Exception queryExecutionException)
            {
                Console.WriteLine(queryExecutionException.InnerException);
                Console.WriteLine("failed to execute query: " + query);

                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackException)
                {
                    Console.WriteLine("failed to rollback");
                }

                success = false;
            }

            sqlConnection.Close();

            return success;
        }

        public IList<object[]> RunQuery(string query)
        {
            sqlConnection.Open();

            var command = sqlConnection.CreateCommand();
            SqlTransaction transaction;

            transaction = sqlConnection.BeginTransaction();

            command.Connection = sqlConnection;
            command.Transaction = transaction;

            var result = new List<object[]>();

            try
            {
                command.CommandText = query;
                var reader = command.ExecuteReader();

                while(reader.Read())
                {
                    var row = new object[reader.FieldCount];

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

            sqlConnection.Close();

            return result;
        }
    }
}