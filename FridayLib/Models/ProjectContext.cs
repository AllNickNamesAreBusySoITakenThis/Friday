using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FridayLib
{
    public class ProjectContext:DbContext
    {
        //public ProjectContext():base(string.Format("Data Source = {0}; Integrated Security = False; Initial catalog = {1}; User = {2}; Password={3}; Connection Timeout=3",
        //            ServiceLib.Configuration.Configuration.Get("Server").ToString(), "ProjectsDB", ServiceLib.Configuration.Configuration.Get("User").ToString(), ServiceLib.Configuration.Configuration.Get("Password").ToString()))
        //{
        public ProjectContext():base("Data Source = 192.168.77.132\\SQLEXPRESS; Integrated Security = False; Initial catalog = ProjectsDB; User = ORPO; Password=Bzpa/123456789; Connection Timeout=3")
        { 
        }
        public DbSet<ControlledProject> Projects { get; set; }
        public DbSet<ControlledApp> Applications { get; set; }
        public DbSet<SourceTextFile> SourceTextFiles { get; set; }
    }
}
