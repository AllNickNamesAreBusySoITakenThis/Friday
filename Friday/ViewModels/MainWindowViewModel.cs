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
    public class MainWindowViewModel:ViewModelBase
    {

        //private ObservableCollection<FridayLib.CFile> files;
        public ObservableCollection<FridayLib.CFile> AllFiles
        {
            get { return FridayLib.MainClass.Files; }
            set
            {
                if(FridayLib.MainClass.Files != value)
                {
                    FridayLib.MainClass.Files = value;
                    RaisePropertyChanged("AllFiles");
                }
            }
        }

        public MainWindowViewModel()
        {
            MainModel.StaticPropertyChanged += MainModel_StaticPropertyChanged;
            FridayLib.MainClass.StaticPropertyChanged += MainClass_StaticPropertyChanged;
            MainModel.StartApp();
        }

        private void MainClass_StaticPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AllFiles = FridayLib.MainClass.Files;
        }

        private void MainModel_StaticPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }

        public ICommand AddFileCommand
        {
            get { return new RelayCommand(ExecuteAddFile); }
        }

        private void ExecuteAddFile()
        {
            MainModel.AddFile();
        }


        public ICommand UpdateAllCommand
        {
            get { return new RelayCommand(ExecuteUpdateAll); }
        }

        private void ExecuteUpdateAll()
        {
            MainModel.UpdateAll();
        }


        public ICommand UpdateSingleCommand
        {
            get { return new RelayCommand<object>(ExecuteUpdateSingle); }
        }

        private void ExecuteUpdateSingle(object args)
        {
            MainModel.UpdateSingle(args as FridayLib.CFile);
        }


        public ICommand CheckSingleCommand
        {
            get { return new RelayCommand<object>(ExecuteCheckSingle); }
        }

        private void ExecuteCheckSingle(object arg)
        {
            MainModel.CheckSingle(arg as FridayLib.CFile);
        }


        public ICommand CheckAllCommand
        {
            get { return new RelayCommand(ExecuteCheckAll); }
        }

        private void ExecuteCheckAll()
        {
            MainModel.CheckAll();
        }

        public ICommand EditCommand
        {
            get { return new RelayCommand<object>(ExecuteEdit); }
        }

        private void ExecuteEdit(object arg)
        {
            MainModel.EditFile(arg);
        }

        public ICommand AddCommand
        {
            get { return new RelayCommand(ExecuteAdd); }
        }

        private void ExecuteAdd()
        {
            MainModel.AddFile();
        }
    }
}
