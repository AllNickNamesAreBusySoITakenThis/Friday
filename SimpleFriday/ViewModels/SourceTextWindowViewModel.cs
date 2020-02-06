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
            Title = string.Format("Список исходных текстов для проекта {0}", Project.Name);
        }
        #region Commands

        

        #endregion
    }
}
