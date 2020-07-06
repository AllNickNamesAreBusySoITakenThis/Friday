using DevExpress.Xpf.Dialogs;
using FridayLib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
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

        public MainWindowViewModel()
        {
            Service.Init();
            ProjectMode = false;
            Collection = CollectionViewSource.GetDefaultView(Projects);
            BindingOperations.EnableCollectionSynchronization(Projects, _syncLock);
            StartApp();    
        }
        public async void StartApp()
        {
            Processing = true;
            ////Status = "Получение данных о проектах из БД";
            //Projects = await DatabaseClass.GetProjects();
            //for (int i = 0; i < Projects.Count; i++)
            //{
            //    Status = string.Format("Получение перечня приложений для проекта :{0}", Projects[i].Name);
            //    await Projects[i].GetApps();
            //    Status = "Подождите";
            //}
            await Task.Run(() =>
            {
                using (ProjectContext pc = new ProjectContext())
                {
                    //foreach (var prj in Projects)
                    //{
                    //    prj.LoadSourceTexts();
                    //    pc.Projects.Add(prj);
                    //    foreach (var stf in prj.SourceTextFiles)
                    //    {
                    //        pc.SourceTextFiles.Add(stf);
                    //    }
                    //}
                    //pc.SaveChanges();
                    foreach (ControlledProject prj in pc.Projects.Include(a => a.Apps).Include(s => s.SourceTextFiles))
                    {
                        lock (_syncLock)
                        {
                            Projects.Add(prj);
                            Projects.Last().UpdateState();
                        }
                    }
                }
            });           
            Processing = false;
        }

        public ICommand AddProjectCommand
        {
            get { return new RelayCommand(ExecuteAddProject); }
        }

        private void ExecuteAddProject()
        {
            Projects.Add(new ControlledProject() { Name = "Новый проект" });
            using (ProjectContext pc = new ProjectContext())
            {
                pc.Projects.Add(Projects.Last());
                pc.SaveChanges();
            }
        }

        public ICommand ShowProjectCommand
        {
            get { return new RelayCommand(ExecuteShowProject); }
        }

        private void ExecuteShowProject()
        {
            ProjectMode = true;
        }

        public ICommand UpdateCurrentProjectCommand
        {
            get { return new RelayCommand(ExecuteUpdateCurrentProject); }
        }

        private void ExecuteUpdateCurrentProject()
        {
            CurrentProject.UpdateState();
        }

        public ICommand CreateDocCommand
        {
            get { return new RelayCommand(ExecuteCreateDoc); }
        }

        private async void ExecuteCreateDoc()
        {
            await CurrentProject.PrepareDocumentation();
        }

        public ICommand CreateReestPackejeCommand
        {
            get { return new RelayCommand(ExecuteCreateReestPackeje); }
        }

        private void ExecuteCreateReestPackeje()
        {
            DXFolderBrowserDialog dialog = new DXFolderBrowserDialog();
            if(dialog.ShowDialog()==true)
            {
                CurrentProject.PrepareReestrPackeje(dialog.SelectedPath);
            }
        }

        public ICommand ActualizeReleaseCommand
        {
            get { return new RelayCommand(ExecuteActualizeRelease); }
        }

        private void ExecuteActualizeRelease()
        {
            CurrentProject.ActualizeRelease();
        }

        public ICommand ActualizeReestrCommand
        {
            get { return new RelayCommand(ExecuteActualizeReestr); }
        }

        private void ExecuteActualizeReestr()
        {
            CurrentProject.ActualizeReestr();
        }

        public ICommand RemoveProjectCommand
        {
            get { return new RelayCommand(ExecuteRemoveProject); }
        }

        private void ExecuteRemoveProject()
        {

        }
    }
}
