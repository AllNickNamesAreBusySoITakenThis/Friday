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
        public static string ConnectionString { get; set; } = string.Format("Data Source = {0}; Integrated Security = False; Initial catalog = {1}; User = {2}; Password={3}; Connection Timeout=3",
            Configuration.Get("Server").ToString(), Configuration.Get("Database").ToString(), Configuration.Get("User").ToString(), Configuration.Get("Password").ToString());
        
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
    }
}
