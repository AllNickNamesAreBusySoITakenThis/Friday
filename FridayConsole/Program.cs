using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FridayLib;

namespace FridayConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = GoogleScriptsClass.GetSheetsData();
            foreach(var file in a)
            {
                DatabaseClass.AddCFile(file);
            }
            Console.Read();
        }
    }
}
