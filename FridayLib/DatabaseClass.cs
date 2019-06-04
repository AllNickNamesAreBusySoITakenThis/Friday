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
                            ID = Convert.ToInt32(reader["ID"]),
                            ProjectName = reader["ProjectName"].ToString()
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
                    string query = string.Format("UPDATE dbo.Friday SET ProjectName = N'{5}', SourceDir=N'{0}', ReleaseDir = N'{1}', RelatievePath = N'{2}' WHERE ID = {4}", 
                        file.SourcePath, file.ReleasePath, file.Name, file.SourceHash, file.ID, file.ProjectName);
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
                    string query = string.Format("INSERT INTO dbo.Friday (ID, SourceDir, ReleaseDir, RelatievePath, ProjectName) VALUES ({0},N'{1}',N'{2}',N'{3}',N'{4}')",
                        file.ID,file.SourcePath, file.ReleasePath, file.Name, file.ProjectName);
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
