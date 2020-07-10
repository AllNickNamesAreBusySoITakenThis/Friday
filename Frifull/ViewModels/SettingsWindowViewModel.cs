using FridayLib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Frifull.ViewModels
{
    public class SettingsWindowViewModel:ViewModelBase
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

        private string mongoServer = ServiceLib.Configuration.Configuration.Get("MongoServer").ToString();
        public string MongoServer
        {
            get { return mongoServer; }
            set
            {
                mongoServer = value;
                ServiceLib.Configuration.Configuration.Set("MongoServer", value);
                RaisePropertyChanged("MongoServer");
            }
        }

        private string mongoDatabase = ServiceLib.Configuration.Configuration.Get("MongoDatabase").ToString();
        public string MongoDatabase
        {
            get { return mongoDatabase; }
            set
            {
                mongoDatabase = value;
                ServiceLib.Configuration.Configuration.Set("MongoDatabase", value);
                RaisePropertyChanged("MongoDatabase");
            }
        }
        private string mongoCollection = ServiceLib.Configuration.Configuration.Get("MongoCollection").ToString();
        public string MongoCollection
        {
            get { return mongoCollection; }
            set
            {
                mongoCollection = value;
                ServiceLib.Configuration.Configuration.Set("MongoCollection", value);
                RaisePropertyChanged("MongoCollection");
            }
        }
        private string spreafsheetAddress = ServiceLib.Configuration.Configuration.Get("SpreadsheetAddress").ToString();
        public string SpreadsheetAddress
        {
            get { return spreafsheetAddress; }
            set
            {
                spreafsheetAddress = value;
                ServiceLib.Configuration.Configuration.Set("SpreadsheetAddress", value);
                RaisePropertyChanged("SpreadsheetAddress");
            }
        }
        private int sheet = Convert.ToInt32(ServiceLib.Configuration.Configuration.Get("SpreadsheetId"));
        public int Sheet
        {
            get { return sheet; }
            set
            {
                sheet = value;
                ServiceLib.Configuration.Configuration.Set("Sheet", value);
                RaisePropertyChanged("Sheet");
            }
        }

        private string allowedExt = ServiceLib.Configuration.Configuration.Get("AllowedExtentions").ToString();
        public string AExt
        {
            get { return allowedExt; }
            set
            {
                allowedExt = value;
                ServiceLib.Configuration.Configuration.Set("AExt", value);
                RaisePropertyChanged("AExt");
            }
        }

        private string sheetName = ServiceLib.Configuration.Configuration.Get("SheetName").ToString();
        public string SheetName
        {
            get { return sheetName; }
            set
            {
                sheetName = value;
                ServiceLib.Configuration.Configuration.Set("SheetName", value);
                RaisePropertyChanged("SheetName");
            }
        }

        public ICommand ConfirmCommand
        {
            get { return new RelayCommand(ExecuteConfirm); }
        }

        private void ExecuteConfirm()
        {
            ServiceLib.Configuration.Configuration.Save(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Friday"));
            DialogResult = true;
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(ExecuteCancel); }
        }

        private void ExecuteCancel()
        {
            Service.Init();
            DialogResult = false;
        }
    }
}
