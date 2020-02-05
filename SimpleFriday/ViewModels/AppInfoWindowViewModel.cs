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
    public class AppInfoWindowViewModel : ViewModelBase
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

        private ControlledApp appl;
        public ControlledApp Appl
        {
            get { return appl; }
            set
            {
                appl = value;
                RaisePropertyChanged("Appl");
            }
        }

        public string Title
        {
            get
            {
                return string.Format("Список настроек приложения: {0}", Appl.Name);
            }
        }

        public AppInfoWindowViewModel()
        {
            Appl = Models.Model.CApp;
        }


        public ICommand ConfirmCommand
        {
            get { return new RelayCommand(ExecuteConfirm); }
        }

        private async void ExecuteConfirm()
        {
            if(await Appl.CheckEquals())
                DialogResult = true;
            else
                System.Windows.MessageBox.Show("Данные по приложению не уникальны!");
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
