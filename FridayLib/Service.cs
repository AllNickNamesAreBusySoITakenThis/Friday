using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib
{
    class Service
    {
        public static string ConnectionString { get; set; } = "Data Source = 192.0.0.165\\MYSERVER; Integrated Security = False; Initial catalog = TestDataBase; User = ORPO; Password=Bzpa/123456789; Connection Timeout=3";
        public static string AddressGoogle { get; set; }

        public static List<string> AllowedFileExtentions { get; set; } = new List<string>() { ".dll", ".exe", ".xml", ".txt",".png",".jpg", ".jpeg", ".bmp", ".ico", ".config", ".json" };
    }
}
