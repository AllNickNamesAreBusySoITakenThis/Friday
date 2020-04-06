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
        /// <summary>
        /// Подключиться к БД
        /// </summary>
        /// <returns></returns>
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
                MainClass.OnErrorInLibrary(string.Format("Не удалось подключиться к БД: {0}", ex.Message));
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
                sqlConnection.ConnectionString = Service.ConnectionString;
                await sqlConnection.OpenAsync();
                return sqlConnection;
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Не удалось подключиться к БД: {0}", ex.Message));
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
            catch(Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Не удалось отключиться от БД: {0}", ex.Message));
            }
        }
        /// <summary>
        /// Проверить наличие таблицы в БД
        /// </summary>
        /// <param name="connection">Активное подключение</param>
        /// <param name="tableName">Имя таблицы для поиска</param>
        /// <returns></returns>
        public async static Task<bool> CheckTableExistance(SqlConnection connection, string tableName)
        {
            try
            {
                SqlCommand checkCmd = connection.CreateCommand();
                checkCmd.CommandText = string.Format("SELECT * FROM SYSOBJECTS WHERE NAME='{0}' AND xtype='U'", tableName);
                var reader = await checkCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    if (reader[0].ToString().ToUpper() == tableName.ToUpper())
                    {
                        reader.Close();
                        return true;
                    }
                }
                reader.Close();
                return false;
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка проверки существования таблицы в БД: {0}", ex.Message));
                return false;
            }
        }

        public async static Task CreateAppTable()
        {
            try
            {
                using (var Connection = await ConnectAsync())
                {
                    var command = Connection.CreateCommand();
                    command.CommandText = string.Format("CREATE TABLE {0}.dbo.Applications (" +
                        "Id INT IDENTITY" +
                        ",AppId INT NOT NULL" +
                        ", ProjectId INT NOT NULL" +
                        ", Name VARCHAR(MAX) NULL" +
                        ", Description VARCHAR(MAX) NULL" +
                        ", SourceDirectory VARCHAR(MAX) NULL" +
                        ", ReleaseDirectory VARCHAR(MAX) NULL" +
                        ", DocumentDirectory VARCHAR(MAX) NULL" +
                        " , MainFileName VARCHAR(MAX) NULL" +
                        ", MainFileReleaseHash VARCHAR(MAX) NULL" +
                        ", MainFileReleaseVersion VARCHAR(MAX) NULL" +
                        ", MainFileReleaseDate VARCHAR(MAX) NULL" +
                        ", Status VARCHAR(MAX) NULL" +
                        ", CompatibleOSs VARCHAR(MAX) NULL" +
                        ", CompatibleScadas VARCHAR(MAX) NULL" +
                        ", CompatibleSZI VARCHAR(MAX) NULL" +
                        ", IdentificationType VARCHAR(MAX) NULL" +
                        ", Installer VARCHAR(MAX) NULL" +
                        ", Report VARCHAR(MAX) NULL" +
                        ", BuildingComponents VARCHAR(MAX) NULL" +
                        ", FunctionalComponents VARCHAR(MAX) NULL" +
                        ", DataStoringMechanism VARCHAR(MAX) NULL" +
                        ", SUBD VARCHAR(MAX) NULL" +
                        ", LocalData VARCHAR(MAX) NULL" +
                        ", AuthorizationType VARCHAR(MAX) NULL" +
                        ", Platform VARCHAR(MAX) NULL" +
                        ", OtherSoft VARCHAR(MAX) NULL" +
                        ", UserCategories VARCHAR(MAX) NULL" +
                        ", IsInReestr BIT NULL" +
                        ", CONSTRAINT PK_Applications_Id PRIMARY KEY CLUSTERED(Id)" +
                        ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]",
                        Connection.Database);
                    await command.ExecuteNonQueryAsync();
                    Disconnect(Connection);
                }
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка создания таблицы приложений: {0}", ex.Message));
            }
        }

        public async static Task GenProjectTable()
        {
            try
            {
                using (var Connection = await ConnectAsync())
                {
                    var command = Connection.CreateCommand();
                    command.CommandText = string.Format("CREATE TABLE {0}.dbo.Projects (" +
                        "Id INT IDENTITY" +
                        ", ProjectId INT NULL" +
                        ", Name VARCHAR(MAX) NULL" +
                        ", ReleaseDirectory VARCHAR(MAX) NULL" +
                        ", WorkingDirectory VARCHAR(MAX) NULL" +
                        ", DocumentDirectory VARCHAR(MAX) NULL" +
                        ", Category INT NULL" +
                        ", Task INT NULL" +
                        ", CONSTRAINT PK_Projects_Id PRIMARY KEY CLUSTERED(Id)" +
                        ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]",
                        Connection.Database);
                    await command.ExecuteNonQueryAsync();
                    Disconnect(Connection);
                }
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка создания таблицы приложений: {0}", ex.Message));
            }
        }

        #region Project

        /// <summary>
        /// Получить перечень проектов, сохраненных в БД
        /// </summary>
        /// <returns></returns>
        public async static Task<ObservableCollection<ControlledProject>> GetProjects()
        {
            try
            {
                ObservableCollection<ControlledProject> result = new ObservableCollection<ControlledProject>();
                using (var Connection = await ConnectAsync())
                {
                    if (Connection != null)
                    {
                        if (!await CheckTableExistance(Connection, "Projects"))
                        {
                            await GenProjectTable();
                            Disconnect(Connection);
                            return new ObservableCollection<ControlledProject>();
                        }
                        var command = Connection.CreateCommand();
                        command.CommandText = "SELECT ProjectId, Name, ReleaseDirectory, WorkingDirectory, DocumentDirectory, Category, Task FROM dbo.Projects";
                        var reader = await command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            result.Add(new ControlledProject
                            {
                                Id = Convert.ToInt32(reader["ProjectId"]),
                                ReleaseDirectory = reader["ReleaseDirectory"].ToString(),
                                WorkingDirectory = reader["WorkingDirectory"].ToString(),
                                DocumentDirectory = reader["DocumentDirectory"].ToString(),
                                Category = (PPOCategories)Convert.ToInt32(reader["Category"]),
                                Task = (PPOTasks)Convert.ToInt32(reader["Task"]),
                                Name = reader["Name"].ToString(),
                                AllAppsAreInReestr = false,
                                AllApрsAreUpToDate = false,
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
                MainClass.OnErrorInLibrary(string.Format("Не удалось получить перечень проектов из БД: {0}", ex.Message));
                return new ObservableCollection<ControlledProject>();
            }
        }

        /// <summary>
        /// Добавить проект в БД 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async static Task AddProject(ControlledProject project)
        {
            try
            {
                ObservableCollection<ControlledProject> result = new ObservableCollection<ControlledProject>();
                using (var Connection = await ConnectAsync())
                {                    
                    if (Connection != null)
                    {
                        if (!await CheckTableExistance(Connection, "Projects"))
                        {
                            await GenProjectTable();
                        }
                        var command = Connection.CreateCommand();
                        command.CommandText = string.Format("INSERT INTO dbo.Projects (ProjectId, Name, ReleaseDirectory, WorkingDirectory, DocumentDirectory, Category, Task) VALUES (" +
                            "{0},'{1}','{2}','{3}','{4}',{5},{6})", project.Id, project.Name, project.ReleaseDirectory, project.WorkingDirectory, project.DocumentDirectory, (int)project.Category, (int)project.Task);
                        await command.ExecuteNonQueryAsync();
                        Disconnect(Connection);
                    }
                    else
                        throw new Exception("Ошибка подключения!");
                }
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Не удалось сохранить проект в БД: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Обновить проект в БД
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async static Task UpdateProject(ControlledProject project)
        {
            try
            {
                ObservableCollection<ControlledProject> result = new ObservableCollection<ControlledProject>();
                using (var Connection = await ConnectAsync())
                {
                    if (Connection != null)
                    {
                        var command = Connection.CreateCommand();
                        command.CommandText = string.Format("UPDATE dbo.Projects SET Name='{1}', ReleaseDirectory='{2}', WorkingDirectory='{3}', DocumentDirectory='{4}', Category={5}, Task={6} WHERE " +
                            "ProjectId={0}", project.Id, project.Name, project.ReleaseDirectory, project.WorkingDirectory, project.DocumentDirectory, (int)project.Category, (int)project.Task);
                        await command.ExecuteNonQueryAsync();
                        Disconnect(Connection);
                    }
                    else
                        throw new Exception("Ошибка подключения!");
                }
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Не удалось обновитб проект в БД: {0}", ex.Message));
            }
        }
        /// <summary>
        /// Удалить проект
        /// </summary>
        /// <param name="prj"></param>
        /// <returns></returns>
        public async static Task DeleteProject(ControlledProject prj)
        {
            try
            {
                foreach (var app in prj.Apps)
                {
                    await DeleteApp(app);
                }
                using (var Connection = await ConnectAsync())
                {
                    var command = Connection.CreateCommand();
                    command.CommandText = string.Format("DELETE FROM dbo.Projects WHERE ProjectId={0}", prj.Id);
                    await command.ExecuteNonQueryAsync();
                    Disconnect(Connection);
                }
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Не удалось удалить данные по проекту {0} в БД: {1}", prj.Name, ex.Message));
            }
        }

        public async static Task<List<int>> CheckForEqual(ControlledProject prj)
        {
            try
            {
                List<int> result = new List<int>();
                using (var Connection = await ConnectAsync())
                {
                    var command = Connection.CreateCommand();
                    command.CommandText = string.Format("SELECT ProjectId FROM dbo.Projects WHERE Name='{0}' ", prj.Name);
                    var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync())
                    {
                        result.Add(Convert.ToInt32(reader["ProjectId"]));
                    }
                    reader.Close();
                    Disconnect(Connection);
                }
                return result;
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка проверки уникальности проекта в БД: {0}",ex.Message));
                return new List<int>();
            }
        }

        #endregion

        #region Application

        /// <summary>
        /// Получить перечень приложений для указанного проекта
        /// </summary>
        /// <param name="prj"></param>
        /// <returns></returns>
        public async static Task<ControlledProject> GetAppsForProject(ControlledProject prj)
        {
            try
            {
                using (var Connection = await ConnectAsync())
                {
                    if (Connection != null)
                    {
                        if (!await CheckTableExistance(Connection, "Applications"))
                        {
                            await CreateAppTable();
                            Disconnect(Connection);
                            return prj;
                        }
                        var command = Connection.CreateCommand();
                        command.CommandText = string.Format("SELECT AppId, ProjectId, Name, ReleaseDirectory, SourceDirectory, DocumentDirectory,Description,MainFileName" +
                            ",MainFileReleaseHash,MainFileReleaseVersion,MainFileReleaseDate,Status,CompatibleOSs,CompatibleScadas,CompatibleSZI,IdentificationType,Installer,Report" +
                            ",BuildingComponents,FunctionalComponents,DataStoringMechanism,SUBD,LocalData,AuthorizationType,Platform,OtherSoft,IsInReestr, UserCategories  FROM dbo.Applications WHERE ProjectId={0}", prj.Id);
                        var reader = await command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            prj.Apps.Add(new ControlledApp
                            {
                                Parent = prj,
                                Id = Convert.ToInt32(reader["AppId"]),
                                Name = reader["Name"].ToString(),
                                ReleaseDirectory = reader["ReleaseDirectory"].ToString(),
                                SourceDirectory = reader["SourceDirectory"].ToString(),
                                DocumentDirectory = reader["DocumentDirectory"].ToString(),
                                Description = reader["Description"].ToString(),
                                MainFileName = reader["MainFileName"].ToString(),
                                MainFileReleaseHash = reader["MainFileReleaseHash"].ToString(),
                                MainFileReleaseVersion = reader["MainFileReleaseVersion"].ToString(),
                                MainFileReleaseDate = reader["MainFileReleaseDate"].ToString(),
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

                foreach (var app in prj.Apps)
                {
                    await app.UpdateMainFileInfoAsync();                    
                }
                prj.UpdateState();
                return prj;
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Не удалось получить перечень приложений для проекта {0} из БД: {1}", prj.Name, ex.Message));
                return prj;
            }
        }

        /// <summary>
        /// Сохранить данные по приложениям для указанного проекта
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public async static Task AddApp(ControlledApp app)
        {
            try
            {
                using (var Connection = await ConnectAsync())
                {
                    if (Connection != null)
                    {
                        if (!await CheckTableExistance(Connection, "Applications"))
                        {
                            await CreateAppTable();                            
                        }
                        var command = Connection.CreateCommand();
                        command.CommandText = string.Format("INSERT INTO dbo.Applications (AppId, ProjectId, Name, ReleaseDirectory, SourceDirectory, DocumentDirectory,Description,MainFileName" +
                                ",MainFileReleaseHash,MainFileReleaseVersion,MainFileReleaseDate,Status,CompatibleOSs,CompatibleScadas,CompatibleSZI,IdentificationType,Installer,Report" +
                                ",BuildingComponents,FunctionalComponents,DataStoringMechanism,SUBD,LocalData,AuthorizationType,Platform,OtherSoft,IsInReestr, UserCategories) VALUES (" +
                                "{0},{1},'{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',{11},'{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}',{26},'{27}')",
                                app.Id, app.Parent.Id, app.Name, app.ReleaseDirectory, app.SourceDirectory, app.DocumentDirectory, app.Description, app.MainFileName, app.MainFileReleaseHash, app.MainFileReleaseVersion
                                , app.MainFileReleaseDate, (int)app.Status, app.CompatibleOSs, app.CompatibleScadas, app.CompatibleSZI, app.IdentificationType, app.Installer, app.Report
                                , app.BuildingComponents, app.FunctionalComponents, app.DataStoringMechanism, app.SUBD, app.LocalData, app.AuthorizationType, app.Platform, app.OtherSoft, app.IsInReestr ? 1 : 0, app.UserCategories);
                        await command.ExecuteNonQueryAsync();
                        Disconnect(Connection);
                    }
                }
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Не удалось сохранить данные по приложению {0} в БД: {1}", app.Name, ex.Message));
            }
        }

        /// <summary>
        /// Обновить данные по приложению
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public async static Task UpdateApp(ControlledApp app)
        {
            try
            {
                using (var Connection = await ConnectAsync())
                {
                    if (Connection != null)
                    {
                        var command = Connection.CreateCommand();
                        command.CommandText = string.Format("UPDATE dbo.Applications SET Name = '{2}', ReleaseDirectory = '{3}', SourceDirectory = '{4}', DocumentDirectory = '{5}',Description = '{6}',MainFileName = '{7}'" +
                                ",MainFileReleaseHash = '{8}',MainFileReleaseVersion = '{9}',MainFileReleaseDate = '{10}',Status = {11},CompatibleOSs = '{12}',CompatibleScadas = '{13}',CompatibleSZI = '{14}',IdentificationType = '{15}',Installer = '{16}',Report = '{17}'" +
                                ",BuildingComponents = '{18}',FunctionalComponents = '{19}',DataStoringMechanism = '{20}',SUBD = '{21}',LocalData = '{22}',AuthorizationType = '{23}',Platform = '{24}',OtherSoft = '{25}',IsInReestr = '{26}', UserCategories='{27}' WHERE AppId={0} AND ProjectId={1} ",
                                app.Id, app.Parent.Id, app.Name, app.ReleaseDirectory, app.SourceDirectory, app.DocumentDirectory, app.Description, app.MainFileName, app.MainFileReleaseHash, app.MainFileReleaseVersion
                                , app.MainFileReleaseDate, (int)app.Status, app.CompatibleOSs, app.CompatibleScadas, app.CompatibleSZI, app.IdentificationType, app.Installer, app.Report
                                , app.BuildingComponents, app.FunctionalComponents, app.DataStoringMechanism, app.SUBD, app.LocalData, app.AuthorizationType, app.Platform, app.OtherSoft, app.IsInReestr, app.UserCategories);
                        await command.ExecuteNonQueryAsync();
                        Disconnect(Connection);
                    }
                }
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Не удалось обновить данные по приложению {0} в БД: {1}", app.Name, ex.Message));
            }
        }

        /// <summary>
        /// Удалить приложение
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public async static Task DeleteApp(ControlledApp app)
        {
            try
            {
                using (var Connection = await ConnectAsync())
                {
                    var command = Connection.CreateCommand();
                    command.CommandText = string.Format("DELETE FROM dbo.Applications WHERE AppId={0} AND ProjectId={1}", app.Id, app.Parent.Id);
                    await command.ExecuteNonQueryAsync();
                    Disconnect(Connection);
                }
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Не удалось удалить данные по приложению {0} в БД: {1}", app.Name, ex.Message));
            }
        }

        public async static Task<List<int>> CheckForEqual(ControlledApp app)
        {
            try
            {
                List<int> result = new List<int>();
                using (var Connection = await ConnectAsync())
                {
                    var command = Connection.CreateCommand();
                    command.CommandText = string.Format("SELECT AppId FROM dbo.Applications WHERE Name='{0}' AND ProjectId={1}", app.Name,  app.Parent.Id);
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        result.Add(Convert.ToInt32(reader["AppId"]));
                    }
                    reader.Close();
                    Disconnect(Connection);
                }
                return result;
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка проверки уникальности проекта в БД: {0}", ex.Message));
                return new List<int>();
            }
        }

        #endregion


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
