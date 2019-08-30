using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PublicAPIToolkit.Database.Controllers
{
   public class DatabaseController
   {
      private static string connectionString = string.Empty;
      private SqlConnection connection;
      private static DatabaseController databaseController = new DatabaseController(@Environment.ExpandEnvironmentVariables("%BinSolDBConnectionString%"));

      public DatabaseController (string connectionString)
      {
         connection = new SqlConnection(connectionString);
      }

      public void InsertInto(string tableName, params string[] values)
      {
         string combinedValues = string.Empty;
         connection.Open();
         SqlCommand command;
         SqlDataAdapter adapter = new SqlDataAdapter();
         int count = 0;
         foreach (string value in values)
         {
            count++;
            combinedValues = combinedValues + "'" + value + "'";
            if (count < values.Length)
            {
               combinedValues = combinedValues + ", ";
            }
         }
         string sql = "INSERT INTO " + tableName + " VALUES(" + combinedValues + ");";
         command = new SqlCommand(sql, connection);
         adapter.InsertCommand = new SqlCommand(sql, connection);
         adapter.InsertCommand.ExecuteNonQuery();
         adapter.Dispose();
         command.Dispose();
         connection.Close();
      }

      public void InsertInto(string tableName, params string[][] columnsAndValues)
      {

      }

      public static DatabaseController GetInstance(string connectionString)
      {
         databaseController.connection.ConnectionString = connectionString;
         return databaseController;
      }

      public static DatabaseController GetInstance()
      {
         return databaseController;
      }
   }
}