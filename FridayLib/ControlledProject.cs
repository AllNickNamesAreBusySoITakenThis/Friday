using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib
{

    public class ControlledProject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private int id;
        private string name;
        private string workingDirectory;
        private PPOCategories category = PPOCategories.Other;
        private string releaseDirectory;
        private PPOTasks task = PPOTasks.SCADA_Addons;
        private bool allAppsAreUpToDate;
        private string docDirectory;
        private bool allAppsAreInReestr;
        private ObservableCollection<ControlledApp> apps = new ObservableCollection<ControlledApp>();
        private ObservableCollection<SourceTextFile> sourceTextFiles = new ObservableCollection<SourceTextFile>();


        /// <summary>
        /// Идентификатор проекта
        /// </summary>
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        /// <summary>
        /// Имя контролируемого приложения
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        
        /// <summary>
        /// Директория для релиза проекта
        /// </summary>
        public string ReleaseDirectory
        {
            get { return releaseDirectory; }
            set
            {
                releaseDirectory = value;
                OnPropertyChanged("ReleaseDirectory");
            }
        }
        
        /// <summary>
        /// Рабочая директория проекта
        /// </summary>
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set
            {
                workingDirectory = value;
                OnPropertyChanged("WorkingDirectory");
            }
        }
                
        /// <summary>
        /// Директория с документацией на проект
        /// </summary>
        public string DocumentDirectory
        {
            get { return docDirectory; }
            set
            {
                docDirectory = value;
                OnPropertyChanged("DocumentDirectory");
            }
        }

        /// <summary>
        /// Категория ППО
        /// </summary>
        public PPOCategories Category
        {
            get { return category; }
            set
            {
                category = value;
                OnPropertyChanged("Category");
            }
        }

        /// <summary>
        /// Задача ППО (по классификации реестра)
        /// </summary>
        public PPOTasks Task
        {
            get { return task; }
            set
            {
                task = value;
                OnPropertyChanged("Task");
            }
        }

        /// <summary>
        /// Все приложения проекта обновлены
        /// </summary>
        public bool AllApрsAreUpToDate
        {
            get { return allAppsAreUpToDate; }
            set
            {
                allAppsAreUpToDate = value;
                OnPropertyChanged("AllApрsAreUpToDate");
            }
        }
        
        /// <summary>
        /// Все приложения проекта прошли в реестр ППО
        /// </summary>
        public bool AllAppsAreInReestr
        {
            get { return allAppsAreInReestr; }
            set
            {
                allAppsAreInReestr = value;
                OnPropertyChanged("AllAppsAreInReestr");
            }
        }

        /// <summary>
        /// Коллекция приложений данного проекта
        /// </summary>
        public ObservableCollection<ControlledApp> Apps
        {
            get { return apps; }
            set
            {
                apps = value;
                OnPropertyChanged("Apps");
            }
        }

        /// <summary>
        /// Коллекция для списка исходных текстов
        /// </summary>
        public ObservableCollection<SourceTextFile> SourceTextFiles
        {
            get { return sourceTextFiles; }
            set
            {
                sourceTextFiles = value;
                OnPropertyChanged("SourceTextFiles");
            }
        }
    }
}
