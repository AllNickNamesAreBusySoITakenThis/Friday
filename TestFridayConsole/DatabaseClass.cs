using FridayLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFridayConsole
{
    public class DatabaseClass
    {
        /// <summary>
        /// Подключиться к БД
        /// </summary>
        /// <returns></returns>
        public static SqlConnection Connect()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection();
                //sqlConnection.ConnectionString = Service.ConnectionString;
                sqlConnection.Open();
                return sqlConnection;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Не удалось подключиться к БД: {0}", ex.Message));
                return null;
            }
        }

        /// <summary>
        /// Подключиться к БД асинхронно
        /// </summary>
        /// <returns></returns>
        public async static Task<SqlConnection> ConnectAsync()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection();
                //sqlConnection.ConnectionString = Service.ConnectionString;
                await sqlConnection.OpenAsync();
                return sqlConnection;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Не удалось подключиться к БД: {0}", ex.Message));
                return null;
            }
        }

        /// <summary>
        /// Отключиться от БД
        /// </summary>
        /// <param name="sqlConnection"></param>
        public static void Disconnect(SqlConnection sqlConnection)
        {
            try
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Не удалось отключиться от БД: {0}", ex.Message));
            }
        }
        public static ObservableCollection<ControlledProject> GetProjects()
        {
            try
            {
                ObservableCollection<ControlledProject> result = new ObservableCollection<ControlledProject>();
                using (var Connection = Connect())
                {
                    if (Connection != null)
                    {                        
                        var command = Connection.CreateCommand();
                        command.CommandText = "SELECT ProjectId, Name, ReleaseDirectory, WorkingDirectory, DocumentDirectory, Category, Task FROM dbo.Projects";
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            result.Add(new ControlledProject
                            {
                                Id = Convert.ToInt32(reader["ProjectId"]),
                                ReleaseDirectory = reader["ReleaseDirectory"].ToString(),
                                WorkingDirectory = reader["WorkingDirectory"].ToString(),
                                DocumentDirectory = reader["DocumentDirectory"].ToString(),
                                Name = reader["Name"].ToString(),
                                Apps = new ObservableCollection<ControlledApp>()
                            });
                        }
                        Disconnect(Connection);
                    }
                    else
                        throw new Exception("Ошибка подключения!");
                }
                return result;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Не удалось получить перечень проектов из БД: {0}", ex.Message));
                return new ObservableCollection<ControlledProject>();
            }
        }
        /// <summary>
        /// Получить перечень приложений для указанного проекта
        /// </summary>
        /// <param name="prj"></param>
        /// <returns></returns>
        public static ObservableCollection<ControlledApp> GetAppsForProject(ControlledProject prj)
        {
            try
            {
                ObservableCollection<ControlledApp> result = new ObservableCollection<ControlledApp>();
                using (var Connection = Connect())
                {
                    if (Connection != null)
                    {                        
                        var command = Connection.CreateCommand();
                        command.CommandText = string.Format("SELECT AppId, ProjectId, Name, ReleaseDirectory, SourceDirectory, DocumentDirectory,Description,MainFileName" +
                            ",MainFileReleaseHash,MainFileReleaseVersion,MainFileReleaseDate,Status,CompatibleOSs,CompatibleScadas,CompatibleSZI,IdentificationType,Installer,Report" +
                            ",BuildingComponents,FunctionalComponents,DataStoringMechanism,SUBD,LocalData,AuthorizationType,Platform,OtherSoft,IsInReestr, UserCategories  FROM dbo.Applications WHERE ProjectId={0}", prj.Id);
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            result.Add(new ControlledApp
                            {
                                Parent = prj,
                                Id = Convert.ToInt32(reader["AppId"]),
                                Name = reader["Name"].ToString(),
                                ReleaseDirectory = reader["ReleaseDirectory"].ToString(),
                                SourceDirectory = reader["SourceDirectory"].ToString(),
                                DocumentDirectory = reader["DocumentDirectory"].ToString(),
                                Description = reader["Description"].ToString(),
                                MainFileName = reader["MainFileName"].ToString(),
                                Status = (PPOReestrStatus)Convert.ToInt32(reader["Status"]),
                                CompatibleOSs = reader["CompatibleOSs"].ToString(),
                                CompatibleScadas = reader["CompatibleScadas"].ToString(),
                                CompatibleSZI = reader["CompatibleSZI"].ToString(),
                                IdentificationType = reader["IdentificationType"].ToString(),
                                Installer = reader["Installer"].ToString(),
                                Report = reader["Report"].ToString(),
                                BuildingComponents = reader["BuildingComponents"].ToString(),
                                FunctionalComponents = reader["FunctionalComponents"].ToString(),
                                DataStoringMechanism = reader["DataStoringMechanism"].ToString(),
                                SUBD = reader["SUBD"].ToString(),
                                LocalData = reader["LocalData"].ToString(),
                                AuthorizationType = reader["AuthorizationType"].ToString(),
                                Platform = reader["Platform"].ToString(),
                                OtherSoft = reader["OtherSoft"].ToString(),
                                IsInReestr = Convert.ToBoolean(reader["IsInReestr"]),
                                UserCategories = reader["UserCategories"].ToString(),
                            });
                        }
                        Disconnect(Connection);
                    }
                    else
                        throw new Exception("Ошибка подключения!");
                }
                return result;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Не удалось получить перечень приложений для проекта {0} из БД: {1}", prj.Name, ex.Message));
                return new ObservableCollection<ControlledApp>();
            }
        }
    }
}
