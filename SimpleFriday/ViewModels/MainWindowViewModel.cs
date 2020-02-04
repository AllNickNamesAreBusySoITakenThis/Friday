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


        public ICommand ShowProjectInfoCommand
        {
            get { return new RelayCommand<object>(ExecuteShowProjectInfo); }
        }

        private async void ExecuteShowProjectInfo(object prj)
        {
            var temp = Models.Model.UpdateProjectInfo(prj as ControlledProject).Result;
            for(int i=0;i<Projects.Count;i++)
            {
                if(Projects[i].Id==temp.Id)
                {
                    Projects[i] = temp;
                    await DatabaseClass.UpdateProject(Projects[i]);
                    break;
                }
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
                    for(int j=0;j<Projects[i].Apps.Count;j++)
                    {
                        if (Projects[i].Apps[j].Id==temp.Id)
                        {
                            Projects[i].Apps[j] = temp;
                            await DatabaseClass.UpdateApp(Projects[i].Apps[j]);
                            break;
                        }                       
                    }
                    break;
                }
            }
        }
    }
}
