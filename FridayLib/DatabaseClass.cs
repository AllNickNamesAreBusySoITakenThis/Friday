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
    public class DatabaseClass
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
                    string query = string.Format("SELECT ID,ProjectName, SourceDir, ReleaseDir, RelatievePath, LastHash, Date, Version FROM dbo.Friday");
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    SqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        var cFile = new CFile()
                        {
                            Name = reader["RelatievePath"].ToString(),
                            SourcePath = reader["SourceDir"].ToString(),
                            ReleasePath = reader["ReleaseDir"].ToString(),
                            LastHash = reader["LastHash"].ToString(),
                            ID = Convert.ToInt32(reader["ID"]),
                            ProjectName = reader["ProjectName"].ToString(),
                            Date = Convert.ToDateTime(reader["Date"]),
                            Version = reader["Version"].ToString()
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

        public static void UpdateCFile(CFile file)
        {
            try
            {
                var connection = Connect();
                if (connection != null)
                {
                    string query = string.Format("UPDATE dbo.Friday SET ProjectName = N'{5}', SourceDir=N'{0}', ReleaseDir = N'{1}', RelatievePath = N'{2}', LastHash = N'{3}', Date = N'{6}', Version = N'{7}' WHERE ID = {4}", 
                        file.SourcePath, file.ReleasePath, file.Name, file.CurrentHash, file.ID, file.ProjectName, file.Date.Date.ToString(), file.Version);
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {

            }
        }

        public static void AddCFile(CFile file)
        {
            try
            {
                var connection = Connect();
                if (connection != null)
                {
                    string query = string.Format("INSERT INTO dbo.Friday (ID, SourceDir, ReleaseDir, RelatievePath, LastHash, ProjectName, Date,Version) VALUES ({0},N'{1}',N'{2}',N'{3}',N'{4}',N'{5}',N'{6}',N'{7}')",
                        file.ID,file.SourcePath, file.ReleasePath, file.Name, file.CurrentHash, file.ProjectName,file.Date.Date.ToString(), file.Version);
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void DeleteCFile(CFile file)
        {
            try
            {
                var connection = Connect();
                if (connection != null)
                {
                    string query = string.Format("DELETE FROM dbo.Friday WHERE ID={0}", file.ID);
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
