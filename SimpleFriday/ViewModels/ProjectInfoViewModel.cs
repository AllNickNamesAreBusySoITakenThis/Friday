using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FridayLib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace SimpleFriday.ViewModels
{
    public class ProjectInfoViewModel:ViewModelBase
    {

        private bool? dialogResult;
        public bool? DialogResult
        {
            get { return dialogResult; }
            set
            {
                dialogResult = value;
                RaisePropertyChanged("DialogResult");
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string releaseDirectory;
        public string ReleaseDirectory
        {
            get { return releaseDirectory; }
            set
            {
                releaseDirectory = value;
                RaisePropertyChanged("ReleaseDirectory");
            }
        }

        private string sourceDirectory;
        public string SourceDirectory
        {
            get { return sourceDirectory; }
            set
            {
                sourceDirectory = value;
                RaisePropertyChanged("SourceDirectory");
            }
        }

        private string documentDirectory;
        public string DocumentDirectory
        {
            get { return documentDirectory; }
            set
            {
                documentDirectory = value;
                RaisePropertyChanged("DocumentDirectory");
            }
        }

        private PPOCategories category;
        public PPOCategories Category
        {
            get { return category; }
            set
            {
                category = value;
                RaisePropertyChanged("Category");
            }
        }

        private PPOTasks task;
        public PPOTasks Task
        {
            get { return task; }
            set
            {
                task = value;
                RaisePropertyChanged("Task");
            }
        }


        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        public ProjectInfoViewModel()
        {
            Name = Models.Model.Project.Name;
            ReleaseDirectory = Models.Model.Project.ReleaseDirectory;
            SourceDirectory = Models.Model.Project.WorkingDirectory;
            DocumentDirectory = Models.Model.Project.DocumentDirectory;
            Category = Models.Model.Project.Category;
            Task = Models.Model.Project.Task;
            Title = string.Format("Информация о проекте: {0}", Models.Model.Project.Name);
        }


        public ICommand ConfirmCommand
        {
            get { return new RelayCommand(ExecuteConfirm); }
        }

        private async void ExecuteConfirm()
        {
            Models.Model.Project.Name = Name;
            Models.Model.Project.ReleaseDirectory = ReleaseDirectory;
            Models.Model.Project.WorkingDirectory = SourceDirectory;
            Models.Model.Project.DocumentDirectory = DocumentDirectory;
            Models.Model.Project.Category = Category;
            Models.Model.Project.Task = Task;
            if(await Models.Model.Project.CheckEquals())
                DialogResult = true;
            else
            {
                System.Windows.MessageBox.Show("Данные по проекту не уникальны!");
            }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(ExecuteCancel); }
        }

        private void ExecuteCancel()
        {
            DialogResult = false;
        }
    }
}
