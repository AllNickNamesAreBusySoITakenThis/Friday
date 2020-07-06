namespace FridayLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeFilesData : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ControlledApps", "CurrentFile_Address");
            DropColumn("dbo.ControlledApps", "CurrentFile_Hash");
            DropColumn("dbo.ControlledApps", "CurrentFile_Version");
            DropColumn("dbo.ControlledApps", "CurrentFile_ChangeDate");
            DropColumn("dbo.ControlledApps", "ReleaseFile_Address");
            DropColumn("dbo.ControlledApps", "ReleaseFile_Hash");
            DropColumn("dbo.ControlledApps", "ReleaseFile_Version");
            DropColumn("dbo.ControlledApps", "ReleaseFile_ChangeDate");
            DropColumn("dbo.ControlledApps", "ReestrFile_Address");
            DropColumn("dbo.ControlledApps", "ReestrFile_Hash");
            DropColumn("dbo.ControlledApps", "ReestrFile_Version");
            DropColumn("dbo.ControlledApps", "ReestrFile_ChangeDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ControlledApps", "ReestrFile_ChangeDate", c => c.String());
            AddColumn("dbo.ControlledApps", "ReestrFile_Version", c => c.String());
            AddColumn("dbo.ControlledApps", "ReestrFile_Hash", c => c.String());
            AddColumn("dbo.ControlledApps", "ReestrFile_Address", c => c.String());
            AddColumn("dbo.ControlledApps", "ReleaseFile_ChangeDate", c => c.String());
            AddColumn("dbo.ControlledApps", "ReleaseFile_Version", c => c.String());
            AddColumn("dbo.ControlledApps", "ReleaseFile_Hash", c => c.String());
            AddColumn("dbo.ControlledApps", "ReleaseFile_Address", c => c.String());
            AddColumn("dbo.ControlledApps", "CurrentFile_ChangeDate", c => c.String());
            AddColumn("dbo.ControlledApps", "CurrentFile_Version", c => c.String());
            AddColumn("dbo.ControlledApps", "CurrentFile_Hash", c => c.String());
            AddColumn("dbo.ControlledApps", "CurrentFile_Address", c => c.String());
        }
    }
}
