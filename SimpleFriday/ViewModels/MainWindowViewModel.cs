using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FridayLib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SimpleFriday.ViewModels
{
    public class MainWindowViewModel:ViewModelBase
    {

        private ObservableCollection<ControlledProject> projects = new ObservableCollection<ControlledProject>();
        private bool processing = false;
        private int maxProgress = 100;
        private int currentProgress = 0;
        private string status = "Подождите";

        /// <summary>
        /// Коллекция проектов
        /// </summary>
        public ObservableCollection<ControlledProject> Projects
        {
            get { return projects; }
            set
            {
                projects = value;
                RaisePropertyChanged("Projects");
            }
        }
        /// <summary>
        /// Флаг заработы алгоритмов приложения
        /// </summary>
        public bool Processing
        {
            get { return processing; }
            set
            {
                processing = value;
                RaisePropertyChanged("Processing");
            }
        }
        /// <summary>
        /// Макимальный прогресс
        /// </summary>
        public int MaxProgress
        {
            get { return maxProgress; }
            set
            {
                maxProgress = value;
                RaisePropertyChanged("MaxProgress");
            }
        }
        /// <summary>
        /// Текущий прогресс
        /// </summary>
        public int CurrentProgress
        {
            get { return currentProgress; }
            set
            {
                currentProgress = value;
                RaisePropertyChanged("CurrentProgress");
            }
        }
        /// <summary>
        /// Текущий статус
        /// </summary>
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                RaisePropertyChanged("Status");
            }
        }

        public MainWindowViewModel()
        {
            StartApp();
        }


        public async void StartApp()
        {
            Processing = true;
            Status = "Получение данных о проектах из БД";
            Projects = await DatabaseClass.GetProjects();
            for (int i = 0; i < Projects.Count; i++)
            {
                Status = string.Format("Получение перечня приложений для проекта :{0}", Projects[i].Name);
                Projects[i] = await DatabaseClass.GetAppsForProject(Projects[i]);
                Status = "Подождите";
            }
            Processing = false;
        }

        #region Команды

        #region Project

        /// <summary>
        /// Отобразить окно с информацией по проекту
        /// </summary>
        public ICommand ShowProjectInfoCommand
        {
            get { return new RelayCommand<object>(ExecuteShowProjectInfo); }
        }
        private async void ExecuteShowProjectInfo(object prj)
        {
            var temp = await Models.Model.UpdateProjectInfo(prj as ControlledProject);
            for (int i = 0; i < Projects.Count; i++)
            {
                if (Projects[i].Id == temp.Id)
                {
                    Projects[i] = temp;
                    await DatabaseClass.UpdateProject(Projects[i]);
                    break;
                }
            }
        }

        /// <summary>
        /// Отобразить окно добавления нового проекта
        /// </summary>
        public ICommand AddProjectCommand
        {
            get { return new RelayCommand(ExecuteAddProject); }
        }
        private async void ExecuteAddProject()
        {
            var temp = await Models.Model.AddNewProject();
            if (temp != null)
            {
                int id = 0;
                foreach (var prj in Projects)
                {
                    if (prj.Id > id)
                    {
                        id = prj.Id;
                    }
                }
                temp.Id = id + 1;
                Projects.Add(temp);
                await DatabaseClass.AddProject(Projects.Last());
            }
        }

        public ICommand ShowSourceTextsCommand
        {
            get { return new RelayCommand<object>(ExecuteShowSourceTexts); }
        }

        private void ExecuteShowSourceTexts(object project)
        {
            Models.Model.Project = (project as ControlledProject).Clone() as ControlledProject;
            Views.SourceTextWindow window = new Views.SourceTextWindow();
            window.Owner = App.Current.MainWindow;
            window.ShowDialog();
        }

        public ICommand RemoveProjectCommand
        {
            get { return new RelayCommand<object>(ExecuteRemoveProject); }
        }

        private async void ExecuteRemoveProject(object project)
        {
            if(System.Windows.MessageBox.Show(string.Format("Действительно удалить проект {0}?",(project as ControlledProject).Name),"Подтвердите действие",System.Windows.MessageBoxButton.YesNo,System.Windows.MessageBoxImage.Question)==System.Windows.MessageBoxResult.Yes)
            {
                Projects.Remove((project as ControlledProject));
                await DatabaseClass.DeleteProject(project as ControlledProject);
            }
        }

        public ICommand PrepareDocForProjectCommand
        {
            get { return new RelayCommand<object>(ExecutePrepareDocForProject); }
        }

        private async void ExecutePrepareDocForProject(object project)
        {
            await (project as ControlledProject).PrepareDocumentation();
        }

        #endregion

        #region Application

        /// <summary>
        /// Добавить новое приложение в проект
        /// </summary>
        public ICommand AddAppCommand
        {
            get { return new RelayCommand<object>(ExecuteAddApp); }
        }
        private async void ExecuteAddApp(object project)
        {
            var temp = await Models.Model.AddNewApp();
            if (temp != null)
            {
                temp.Id = (project as ControlledProject).Apps.Count;
                temp.Parent = (project as ControlledProject);
                (project as ControlledProject).Apps.Add(temp);
                await (project as ControlledProject).Apps.Last().UpdateMainFileInfoAsync();
                (project as ControlledProject).UpdateState();
                await DatabaseClass.AddApp((project as ControlledProject).Apps.Last());
                GoogleScriptsClass.AddDataToSheet((project as ControlledProject).Apps.Last());
            }
        }

        /// <summary>
        /// Отобразить окно с информацией по приложению
        /// </summary>
        public ICommand ShowAppInfoCommand
        {
            get { return new RelayCommand<object>(ExecuteShowAppInfo); }
        }
        private async void ExecuteShowAppInfo(object app)
        {
            var temp = Models.Model.UpdateAppInfo(app as ControlledApp).Result;
            for (int i = 0; i < Projects.Count; i++)
            {
                if (Projects[i].Id == temp.Parent.Id)
                {
                    for (int j = 0; j < Projects[i].Apps.Count; j++)
                    {
                        if (Projects[i].Apps[j].Id == temp.Id)
                        {
                            Projects[i].Apps[j] = temp;
                            await Projects[i].Apps[j].UpdateMainFileInfoAsync();
                            Projects[i].UpdateState();
                            await DatabaseClass.UpdateApp(Projects[i].Apps[j]);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Актуализировать релиз конкретного приложения
        /// </summary>
        public ICommand ActualizeReleaseCommand
        {
            get { return new RelayCommand<object>(ExecuteActualizeRelease); }
        }
        private async void ExecuteActualizeRelease(object application)
        {
            await Task.Run(() => (application as ControlledApp).CopyToFolder((application as ControlledApp).SourceDirectory, (application as ControlledApp).ReleaseDirectory));
            await (application as ControlledApp).UpdateMainFileInfoAsync();
            await DatabaseClass.UpdateApp((application as ControlledApp));
            (application as ControlledApp).Parent.UpdateState();
        }
        /// <summary>
        /// Обновить данные по приложению
        /// </summary>
        public ICommand UpdateAppMainFileInfoCommand
        {
            get { return new RelayCommand<object>(ExecuteUpdateAppMainFileInfo); }
        }
        private async void ExecuteUpdateAppMainFileInfo(object application)
        {
            await (application as ControlledApp).UpdateMainFileInfoAsync();
            await DatabaseClass.UpdateApp((application as ControlledApp));
            (application as ControlledApp).Parent.UpdateState();
        }

        public ICommand PrepareDocForAppCommand
        {
            get { return new RelayCommand<object>(ExecutePrepareDocForApp); }
        }

        private async void ExecutePrepareDocForApp(object application)
        {
            await (application as ControlledApp).PrepareDocumentation();
        }

        /// <summary>
        /// Удалить приложение
        /// </summary>
        public ICommand RemoveAppCommand
        {
            get { return new RelayCommand<object>(ExecuteRemoveApp); }
        }
        private async void ExecuteRemoveApp(object application)
        {
            for (int i = 0; i < (application as ControlledApp).Parent.Apps.Count; i++)
            {
                if ((application as ControlledApp).Parent.Apps[i].Id.Equals((application as ControlledApp).Id))
                {
                    await DatabaseClass.DeleteApp(application as ControlledApp);
                    (application as ControlledApp).Parent.Apps.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Common

        /// <summary>
        /// Команда "Актуализировать все"
        /// </summary>
        public ICommand UpdateAllCommand
        {
            get { return new RelayCommand(ExecuteUpdateAll); }
        }
        private async void ExecuteUpdateAll()
        {
            Processing = true;
            await Task.Run(async () =>
            {
                Status = "Актуализация всего ПО";
                foreach (var prj in Projects)
                {
                    await prj.Update();
                }
            });
            
            System.Windows.MessageBox.Show(App.Current.MainWindow, "Завершена актуализация всех приложений!", "Операция завершена!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            Processing = false;
        }

        /// <summary>
        /// Обновить данные по всем проектам
        /// </summary>
        public ICommand RefreshDataCommand
        {
            get { return new RelayCommand(ExecuteRefreshData); }
        }
        private async void ExecuteRefreshData()
        {
            Processing = true;
            Status = "Получение данных о проектах из БД";
            Projects = await DatabaseClass.GetProjects();
            for (int i = 0; i < Projects.Count; i++)
            {
                Status = string.Format("Получение перечня приложений для проекта :{0}", Projects[i].Name);
                Projects[i] = await DatabaseClass.GetAppsForProject(Projects[i]);
                Status = "Подождите";
            }
            System.Windows.MessageBox.Show(App.Current.MainWindow, "Завершено получение данных по всем приложениям!", "Операция завершена!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            Processing = false;
        }


        public ICommand ShowSettingsCommand
        {
            get { return new RelayCommand(ExecuteShowSettings); }
        }

        private void ExecuteShowSettings()
        {
            Views.SettingWindow window = new Views.SettingWindow();
            window.Owner = App.Current.MainWindow;
            window.ShowDialog();
        }

        #endregion

        #endregion
    }
}
