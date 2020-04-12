using FridayLib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Frifull.ViewModels
{
    public class MainWindowViewModel:ViewModelBase
    {
        private bool processing = false;
        private string status = "Подождите";
        private ObservableCollection<ControlledProject> projects = new ObservableCollection<ControlledProject>();
        private ControlledProject currentProject;


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

        public MainWindowViewModel()
        {
            Service.Init();
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
                Projects[i].GetApps();
                Status = "Подождите";
            }
            Processing = false;
        }

        public ICommand NavigatingCommand
        {
            get { return new RelayCommand<object>(ExecuteNavigating); }
        }

        private void ExecuteNavigating(object param)
        {

        }
    }
}
