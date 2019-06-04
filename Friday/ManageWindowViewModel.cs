using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;

namespace Friday
{
    public class ManageWindowViewModel:ViewModelBase
    {
        private bool? _dialogResult;

        public bool? DialogResult
        {
            get { return _dialogResult; }
            protected set
            {
                _dialogResult = value;
                RaisePropertyChanged("DialogResult");
            }
        }

        //private FridayLib.CFile file;
        public FridayLib.CFile File
        {
            get { return MainModel.WorkingFile; }
            set
            {
                if(MainModel.WorkingFile!=value)
                {
                    MainModel.WorkingFile = value;
                    RaisePropertyChanged("File");
                }
            }
        }
        public ManageWindowViewModel()
        {
            MainModel.StaticPropertyChanged += MainModel_StaticPropertyChanged;
        }

        private void MainModel_StaticPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            File = MainModel.WorkingFile;
        }

        public ICommand ConfirmCommand
        {
            get { return new RelayCommand(ExecuteConfirm); }
        }

        private void ExecuteConfirm()
        {
            DialogResult = true;
        }

        public ICommand DiscardCommand
        {
            get { return new RelayCommand(ExecuteDiscard); }
        }

        private void ExecuteDiscard()
        {
            DialogResult = false;
        }
    }
}
