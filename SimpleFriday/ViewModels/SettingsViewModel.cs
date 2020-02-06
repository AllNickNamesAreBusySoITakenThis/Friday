using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace SimpleFriday.ViewModels
{
    public class SettingsViewModel:ViewModelBase
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


        private int spreadsheetId;
        public int SpreadsheetId
        {
            get { return spreadsheetId; }
            set
            {
                spreadsheetId = value;
                RaisePropertyChanged("SpreadsheetId");
            }
        }

        private string spreadsheetAddress;
        public string SpreadsheetAddress
        {
            get { return spreadsheetAddress; }
            set
            {
                spreadsheetAddress = value;
                RaisePropertyChanged("SpreadsheetAddress");
            }
        }

        private string server;
        public string Server
        {
            get { return server; }
            set
            {
                server = value;
                RaisePropertyChanged("Server");
            }
        }

        private string database;
        public string Database
        {
            get { return database; }
            set
            {
                database = value;
                RaisePropertyChanged("Database");
            }
        }

        private string user;
        public string User
        {
            get { return user; }
            set
            {
                user = value;
                RaisePropertyChanged("User");
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                RaisePropertyChanged("Password");
            }
        }

        private string allowedExtentions;
        public string AllowedExtentions
        {
            get { return allowedExtentions; }
            set
            {
                allowedExtentions = value;
                RaisePropertyChanged("AllowedExtentions");
            }
        }

        public SettingsViewModel()
        {
            ServiceLib.Configuration.Configuration.Load(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Friday"));
            AllowedExtentions = ServiceLib.Configuration.Configuration.Get("AllowedExtentions").ToString();
            Server = ServiceLib.Configuration.Configuration.Get("Server").ToString();
            Database = ServiceLib.Configuration.Configuration.Get("Database").ToString();
            User = ServiceLib.Configuration.Configuration.Get("User").ToString();
            Password = ServiceLib.Configuration.Configuration.Get("Password").ToString();
            SpreadsheetAddress = ServiceLib.Configuration.Configuration.Get("SpreadsheetAddress").ToString();
            SpreadsheetId = Convert.ToInt32(ServiceLib.Configuration.Configuration.Get("SpreadsheetId"));
        }


        public ICommand ConfirmCommand
        {
            get { return new RelayCommand(ExecuteConfirm); }
        }

        private void ExecuteConfirm()
        {
            ServiceLib.Configuration.Configuration.Add("AllowedExtentions", ServiceLib.Configuration.SettingType.String, AllowedExtentions);
            ServiceLib.Configuration.Configuration.Add("Server", ServiceLib.Configuration.SettingType.String, Server);
            ServiceLib.Configuration.Configuration.Add("Database", ServiceLib.Configuration.SettingType.String, Database);
            ServiceLib.Configuration.Configuration.Add("User", ServiceLib.Configuration.SettingType.String, User);
            ServiceLib.Configuration.Configuration.Add("Password", ServiceLib.Configuration.SettingType.String, Password);
            ServiceLib.Configuration.Configuration.Add("SpreadsheetAddress", ServiceLib.Configuration.SettingType.String,SpreadsheetAddress);
            ServiceLib.Configuration.Configuration.Add("SpreadsheetId", ServiceLib.Configuration.SettingType.Integer, SpreadsheetId);
            ServiceLib.Configuration.Configuration.Save(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Friday"));
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
