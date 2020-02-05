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
        public ObservableCollection<ControlledProject> Projects
        {
            get { return projects; }
            set
            {
                projects = value;
                RaisePropertyChanged("Projects");
            }
        }

        private bool processing=false;
        public bool Processing
        {
            get { return processing; }
            set
            {
                processing = value;
                RaisePropertyChanged("Processing");
            }
        }

        private int maxProgress=100;
        public int MaxProgress
        {
            get { return maxProgress; }
            set
            {
                maxProgress = value;
                RaisePropertyChanged("MaxProgress");
            }
        }

        private int currentProgress=0;
        public int CurrentProgress
        {
            get { return currentProgress; }
            set
            {
                currentProgress = value;
                RaisePropertyChanged("CurrentProgress");
            }
        }

        private string status="Подождите";
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


        public ICommand UpdateAllCommand
        {
            get { return new RelayCommand(ExecuteUpdateAll); }
        }

        private async void ExecuteUpdateAll()
        {
            Processing = true;
            Status = "Актуализация всего ПО";
            foreach(var prj in Projects)
            {
                await prj.Update();
            }
            Processing = false;
        }

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
            Processing = false;
        }
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

        public ICommand AddAppCommand
        {
            get { return new RelayCommand<object>(ExecuteAddApp); }
        }

        private async void ExecuteAddApp(object project)
        {
            var temp = await Models.Model.AddNewApp();
            if(temp!=null)
            {
                temp.Id = (project as ControlledProject).Apps.Count;
                temp.Parent = (project as ControlledProject);
                (project as ControlledProject).Apps.Add(temp);
                await (project as ControlledProject).Apps.Last().UpdateMainFileInfoAsync();
                (project as ControlledProject).UpdateState();
                await DatabaseClass.AddApp((project as ControlledProject).Apps.Last());
            }
        }

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

        public ICommand ActualizeReleaseCommand
        {
            get { return new RelayCommand<object>(ExecuteActualizeRelease); }
        }

        private async void ExecuteActualizeRelease(object application)
        {
            await Task.Run(()=>(application as ControlledApp).CopyToFolder((application as ControlledApp).SourceDirectory, (application as ControlledApp).ReleaseDirectory));
            await (application as ControlledApp).UpdateMainFileInfoAsync();
            await DatabaseClass.UpdateApp((application as ControlledApp));
            (application as ControlledApp).Parent.UpdateState();
        }

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

        public ICommand RemoveAppCommand
        {
            get { return new RelayCommand<object>(ExecuteRemoveApp); }
        }

        private async void ExecuteRemoveApp(object application)
        {
            for(int i=0;i<(application as ControlledApp).Parent.Apps.Count;i++)
            {
                if((application as ControlledApp).Parent.Apps[i].Id.Equals((application as ControlledApp).Id))
                {
                    await DatabaseClass.DeleteApp(application as ControlledApp);
                    (application as ControlledApp).Parent.Apps.RemoveAt(i);
                }
            }
        }

        #endregion
    }
}
