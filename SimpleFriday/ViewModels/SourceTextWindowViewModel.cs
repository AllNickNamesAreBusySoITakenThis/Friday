using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FridayLib;

namespace SimpleFriday.ViewModels
{
    public class SourceTextWindowViewModel:ViewModelBase
    {


        private bool? dialogresult;
        public bool? DialogResult
        {
            get { return dialogresult; }
            set
            {
                dialogresult = value;
                RaisePropertyChanged("DialogResult");
            }
        }

        private ControlledProject project;
        public ControlledProject Project
        {
            get { return project; }
            set
            {
                project = value;
                RaisePropertyChanged("Project");
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

        public SourceTextWindowViewModel()
        {
            Project = Models.Model.Project;
            Project.LoadSourceTexts();
            Title = string.Format("Список исходных текстов для проекта {0}", Project.Name);
        }

        #region Commands


        public ICommand SaveAsTextCommand
        {
            get { return new RelayCommand(ExecuteSaveAsText); }
        }

        private void ExecuteSaveAsText()
        {
            Project.SaveSourceTextsAsText();
        }

        public ICommand ReloadCommand
        {
            get { return new RelayCommand(ExecuteReload); }
        }

        private void ExecuteReload()
        {
            Project.SourceTextFiles = new System.Collections.ObjectModel.ObservableCollection<SourceTextFile>();
            Project.LoadSourceTexts();
        }

        public ICommand CloseCommand
        {
            get { return new RelayCommand(ExecuteClose); }
        }

        private void ExecuteClose()
        {
            DialogResult = false;
        }

        public ICommand SaveAsExcelCommand
        {
            get { return new RelayCommand(ExecuteSaveAsExcel); }
        }

        private void ExecuteSaveAsExcel()
        {
            Project.SaveSourceTextsAsExcel();
        }

        #endregion
    }
}
