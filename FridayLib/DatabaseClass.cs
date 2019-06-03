using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace FridayLib
{
    /// <summary>
    /// Логика работы с БД
    /// </summary>
    class DatabaseClass
    {
        public static SqlConnection Connect()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = Service.ConnectionString;
                sqlConnection.Open();
                return sqlConnection;
            }
            catch(Exception ex)
            {
                return null;
            }            
        }

        public static void Disconnect(SqlConnection sqlConnection)
        {
            try
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            catch
            {

            }
        }

        public static ObservableCollection<CFile> GetFileData()
        {
            try
            {
                ObservableCollection<CFile> cFiles = new ObservableCollection<CFile>();
                var connection = Connect();
                if(connection!=null)
                {
                    string query = string.Format("SELECT SourceDir, ReleaseDir, RelatievePath, LastHash FROM dbo.Friday");
                    SqlCommand command = connection.CreateCommand();
                    SqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        var cFile = new CFile()
                        {
                            Name = reader["RelatievePath"].ToString(),
                            SourcePath = reader["SourceDir"].ToString(),
                            ReleasePath = reader["ReleaseDir"].ToString(),
                            LastHash = reader["LastHash"].ToString()
                        };
                        cFiles.Add(cFile);
                    }
                }
                return cFiles;
            }
            catch(Exception ex)
            {
                return new ObservableCollection<CFile>();
            }
        }
    }
}
