using FridayLib;
using SimpleFriday.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFriday.Models
{

    public class Model
    {
        public static event PropertyChangedEventHandler StaticPropertyChanged;
        static void OnStaticPropertyChanged(string name)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }

        static ControlledProject project;
        public static ControlledProject Project
        {
            get { return project; }
            set
            {
                project = value;
                OnStaticPropertyChanged("Project");
            }
        }

        static ControlledApp app;
        public static ControlledApp CApp
        {
            get { return app; }
            set
            {
                app = value;
                OnStaticPropertyChanged("CApp");
            }
        }
        internal static Task<ControlledProject> AddNewProject()
        {
            Project = new ControlledProject();
            TaskCompletionSource<ControlledProject> tsc = new TaskCompletionSource<ControlledProject>();
            ProjectInfoWindow window = new ProjectInfoWindow() { Owner = App.Current.MainWindow };
            window.Closing += delegate
            {
                if (window.DialogResult == true)
                {
                    tsc.SetResult(Project);
                }
                else
                    tsc.SetResult(null);
            };
            window.ShowDialog();
            return tsc.Task;
        }
        internal static Task<ControlledProject> UpdateProjectInfo(ControlledProject prj)
        {
            Project = prj.Clone() as ControlledProject;
            //Project = ObjectCopier.CloneJson(prj);
            TaskCompletionSource<ControlledProject> tsc = new TaskCompletionSource<ControlledProject>();
            ProjectInfoWindow window = new ProjectInfoWindow() { Owner = App.Current.MainWindow };
            window.Closing += delegate
            {
                if (window.DialogResult == true)
                {
                    tsc.SetResult(Project);
                }
                else
                    tsc.SetResult(prj);
            };
            window.ShowDialog();
            return tsc.Task;
        }

        internal static Task<ControlledApp> AddNewApp()
        {
            CApp = new ControlledApp();
            //Project = ObjectCopier.CloneJson(prj);
            TaskCompletionSource<ControlledApp> tsc = new TaskCompletionSource<ControlledApp>();
            AppInfoWindow window = new AppInfoWindow() { Owner = App.Current.MainWindow };
            window.Closing += delegate
            {
                if (window.DialogResult == true)
                {
                    tsc.SetResult(CApp);
                }
                else
                    tsc.SetResult(null);
            };
            window.ShowDialog();
            return tsc.Task;
        }
        internal static Task<ControlledApp> UpdateAppInfo(ControlledApp app)
        {
            CApp = app.Clone() as ControlledApp;
            //Project = ObjectCopier.CloneJson(prj);
            TaskCompletionSource<ControlledApp> tsc = new TaskCompletionSource<ControlledApp>();
            AppInfoWindow window = new AppInfoWindow() { Owner = App.Current.MainWindow };
            window.Closing += delegate
            {
                if (window.DialogResult == true)
                {
                    tsc.SetResult(CApp);
                }
                else
                    tsc.SetResult(app);
            };
            window.ShowDialog();
            return tsc.Task;
        }
        
    }
}
