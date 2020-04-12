using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLib.Configuration;

namespace FridayLib
{
    public class Service
    {
        public delegate void EventInLibraryDelegate(string message);
        public static event EventInLibraryDelegate ErrorInLibrary;
        public static void OnErrorInLibrary(string message)
        {
            ErrorInLibrary?.Invoke(message);
        }

        
        public static string ConnectionString { get; set; }
        //public static List<string> AllowedFileExtentions { get; set; } = new List<string>() { ".dll", ".exe", ".xml", ".txt",".png",".jpg", ".jpeg", ".bmp", ".ico", ".config", ".json" };

        public static string GetStringFromCollecction(IEnumerable<string> collection)
        {
            string str = "";
            for(int i=0;i<collection.Count(); i++)
            {
                str += string.Format("{0};", collection.ElementAt(i));
            }
            return str;
        }
        public static List<String> GetListFromString(string source)
        {
            var data = source.Split(new char[] { ';' });
            return data.ToList();
        }

        public static void Init()
        {
            try
            {
                //ServiceLib.Configuration.Configuration.Add("AllowedExtentions", ServiceLib.Configuration.SettingType.String, AllowedExtentions);
                ServiceLib.Configuration.Configuration.Add("Server", ServiceLib.Configuration.SettingType.String, "192.168.77.132\\SQLEXPRESS");
                ServiceLib.Configuration.Configuration.Add("Database", ServiceLib.Configuration.SettingType.String, "TestDataBase");
                ServiceLib.Configuration.Configuration.Add("User", ServiceLib.Configuration.SettingType.String, "ORPO");
                ServiceLib.Configuration.Configuration.Add("Password", ServiceLib.Configuration.SettingType.String, "Bzpa/123456789");
                ServiceLib.Configuration.Configuration.Add("SpreadsheetAddress", ServiceLib.Configuration.SettingType.String, "");
                ServiceLib.Configuration.Configuration.Add("SpreadsheetId", ServiceLib.Configuration.SettingType.Integer, "");
                ServiceLib.Configuration.Configuration.Load(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Friday"));
                ConnectionString = string.Format("Data Source = {0}; Integrated Security = False; Initial catalog = {1}; User = {2}; Password={3}; Connection Timeout=3",
                    Configuration.Get("Server").ToString(), Configuration.Get("Database").ToString(), Configuration.Get("User").ToString(), Configuration.Get("Password").ToString());
            }
            catch (Exception ex)
            {
                
            }

        }
    }
}
