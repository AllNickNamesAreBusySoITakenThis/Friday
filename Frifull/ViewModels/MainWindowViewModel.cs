using DevExpress.Xpf.Dialogs;
using FridayLib;
using Frifull.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Frifull.ViewModels
{
    public class MainWindowViewModel:ViewModelBase
    {
        private bool processing = false;
        private string status = "Подождите";
        private ObservableCollection<ControlledProject> projects = new ObservableCollection<ControlledProject>();
        private ControlledProject currentProject;
        private object _syncLock = new Object();

        public ICollectionView Collection { get; set; }

        public ObservableCollection<ControlledProject> Projects
        {
            get { return projects; }
            set
            {
                projects = value;
                RaisePropertyChanged("Projects");
            }
        }

        public ControlledProject CurrentProject
        {
            get { return currentProject; }
            set
            {
                currentProject = value;
                RaisePropertyChanged("CurrentProject");
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
        private bool projectMode=true;
        public bool ProjectMode
        {
            get { return projectMode; }
            set
            {
                projectMode = value;
                RaisePropertyChanged("ProjectMode");
            }
        }

        private double process=0;
        public double BarProcess
        {
            get { return process; }
            set
            {
                process = value;
                //Console.WriteLine(value);
                RaisePropertyChanged("BarProcess");
            }
        }

        public MainWindowViewModel()
        {
            Service.ErrorInLibrary += Service_ErrorInLibrary;
            Service.Init();
            ProjectMode = false;

            //Collection = CollectionViewSource.GetDefaultView(Projects);
            //BindingOperations.EnableCollectionSynchronization(Projects, _syncLock);
            StartApp();    
        }

        private void Service_ErrorInLibrary(string message)
        {
            NLog.LogManager.GetCurrentClassLogger().Error(message);
            App.Current.Dispatcher.Invoke(()=>DevExpress.Xpf.Core.DXMessageBox.Show(App.Current.MainWindow, message, "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error));
        }

        public async void StartApp()
        {
            Processing = true;
            //Projects = await Task.Run(()=>ControlledProject.GetProjects());
            Projects = await ControlledProject.GetProjectsAsync(new Progress<double>(state=>BarProcess=state));
            Processing = false;
        }
        /// <summary>
        /// Обновить данныео всех проектах
        /// </summary>
        public ICommand RefreshCommand
        {
            get { return new RelayCommand(ExecuteRefresh); }
        }

        private async void ExecuteRefresh()
        {
            Processing = true;
            CurrentProject = null;
            Projects = await ControlledProject.GetProjectsAsync(new Progress<double>(state => BarProcess = state));
            Processing = false;
        }

        /// <summary>
        /// Добавить проект
        /// </summary>
        public ICommand AddProjectCommand
        {
            get { return new RelayCommand(ExecuteAddProject); }
        }

        private void ExecuteAddProject()
        {
            ControlledProject.CreateProject(Projects);
        }

        /// <summary>
        /// Сохранить все проекты
        /// </summary>
        public ICommand SaveAllCommand
        {
            get { return new RelayCommand(ExecuteSaveAll); }
        }

        private async void ExecuteSaveAll()
        {
            await ControlledProject.SaveProjects(Projects);
        }

        /// <summary>
        /// Отобразить данные по выбранному проекту
        /// </summary>
        public ICommand ShowProjectCommand
        {
            get { return new RelayCommand(ExecuteShowProject); }
        }

        private void ExecuteShowProject()
        {
            ProjectMode = true;
        }

        /// <summary>
        /// Обновить данные по проекту (актуальность приложений)
        /// </summary>
        public ICommand UpdateCurrentProjectCommand
        {
            get { return new RelayCommand(ExecuteUpdateCurrentProject); }
        }

        private void ExecuteUpdateCurrentProject()
        {
            CurrentProject.UpdateState();
        }

        /// <summary>
        /// Подготовить документацию на проект
        /// </summary>
        public ICommand CreateDocCommand
        {
            get { return new RelayCommand(ExecuteCreateDoc); }
        }

        private async void ExecuteCreateDoc()
        {
            await CurrentProject.PrepareDocumentation();
        }

        /// <summary>
        /// Подготовить пакет документации в реестр
        /// </summary>
        public ICommand CreateReestPackejeCommand
        {
            get { return new RelayCommand(ExecuteCreateReestPackeje); }
        }

        private async void ExecuteCreateReestPackeje()
        {
            DXFolderBrowserDialog dialog = new DXFolderBrowserDialog();
            if(dialog.ShowDialog()==true)
            {
                await Task.Run(() => CurrentProject.PrepareReestrPackeje(dialog.SelectedPath));
            }
        }

        /// <summary>
        /// Актуализировать релиз для выбранного проекта
        /// </summary>
        public ICommand ActualizeReleaseCommand
        {
            get { return new RelayCommand(ExecuteActualizeRelease); }
        }

        private async void ExecuteActualizeRelease()
        {
            await Task.Run(() => CurrentProject.ActualizeRelease());
        }

        /// <summary>
        /// Актуализировать реестр в выбранном проекте
        /// </summary>
        public ICommand ActualizeReestrCommand
        {
            get { return new RelayCommand(ExecuteActualizeReestr); }
        }

        private async void ExecuteActualizeReestr()
        {
            await Task.Run(() => CurrentProject.ActualizeReestr());
        }

        /// <summary>
        /// Удалить выбранный проект
        /// </summary>
        public ICommand RemoveProjectCommand
        {
            get { return new RelayCommand(ExecuteRemoveProject); }
        }

        private void ExecuteRemoveProject()
        {
            if (DevExpress.Xpf.Core.DXMessageBox.Show(App.Current.MainWindow,string.Format("Действительно удалить {0}?",CurrentProject.Name),"Подтвердите удаление",System.Windows.MessageBoxButton.YesNo,System.Windows.MessageBoxImage.Question)==System.Windows.MessageBoxResult.Yes)
            {
                ControlledProject.RemoveProject(CurrentProject);
                Projects.Remove(CurrentProject); 
            }
        }


        public ICommand CheckCurrentAppCommand
        {
            get { return new RelayCommand(ExecuteCheckCurrentApp); }
        }

        private async void ExecuteCheckCurrentApp()
        {
            await CurrentProject.CurrentApp.UpdateFileInfoAsync();
        }

        public ICommand ActualizeCurrentAppReleaseCommand
        {
            get { return new RelayCommand(ExecuteActualizeCurrentAppRelease); }
        }

        private async void ExecuteActualizeCurrentAppRelease()
        {
            await Task.Run(() => CurrentProject.CurrentApp.ActualizeRelease());
        }


        public ICommand ActualizeCurrentAppReestrCommand
        {
            get { return new RelayCommand(ExecuteActualizeCurrentAppReestr); }
        }

        private async void ExecuteActualizeCurrentAppReestr()
        {
            await Task.Run(() => CurrentProject.CurrentApp.ActualizeReestr());
        }


        public ICommand PrepareCurrentAppDocCommand
        {
            get { return new RelayCommand(ExecutePrepareCurrentAppDoc); }
        }

        private async void ExecutePrepareCurrentAppDoc()
        {
            await Task.Run(() =>
            {
                CurrentProject.CurrentApp.PrepareFormular();
                CurrentProject.CurrentApp.PrepareRequest();
            });           
        }


        public ICommand RemoveCurrentAppCommand
        {
            get { return new RelayCommand(ExecuteRemoveCurrentApp); }
        }

        private void ExecuteRemoveCurrentApp()
        {
            CurrentProject.RemoveApp(CurrentProject.CurrentApp);
        }

        public ICommand RefreshSourceTextsCommand
        {
            get { return new RelayCommand(ExecuteRefreshSourceTexts); }
        }

        private void ExecuteRefreshSourceTexts()
        {
            SourceTextCreation.UpdateSourceTextList(CurrentProject);
        }


        public ICommand SaveSourceTextCommand
        {
            get { return new RelayCommand(ExecuteSaveSourceText); }
        }

        private async void ExecuteSaveSourceText()
        {
            CurrentProject.SaveSourceTextsAsText();
            await CurrentProject.SaveProjectAsync();
        }


        public ICommand SaveSourceTextsAsExcelCommand
        {
            get { return new RelayCommand(ExecuteSaveSourceTextsAsExcel); }
        }

        private void ExecuteSaveSourceTextsAsExcel()
        {
            CurrentProject.SaveSourceTextsAsExcel();
        }

        /// <summary>
        /// Показать настройки
        /// </summary>
        public ICommand ShowSettingsCommand
        {
            get { return new RelayCommand(ExecuteShowSettings); }
        }

        private void ExecuteShowSettings()
        {
            new SettingsScreen() { Owner = App.Current.MainWindow }.ShowDialog();
        }
        
    }
}
