namespace FridayLib.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ControlledApps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        SourceDirectory = c.String(),
                        ReleaseDirectory = c.String(),
                        DocumentDirectory = c.String(),
                        ReestrDirectory = c.String(),
                        MainFileName = c.String(),
                        CurrentFile_Address = c.String(),
                        CurrentFile_Hash = c.String(),
                        CurrentFile_Version = c.String(),
                        CurrentFile_ChangeDate = c.String(),
                        ReleaseFile_Address = c.String(),
                        ReleaseFile_Hash = c.String(),
                        ReleaseFile_Version = c.String(),
                        ReleaseFile_ChangeDate = c.String(),
                        ReestrFile_Address = c.String(),
                        ReestrFile_Hash = c.String(),
                        ReestrFile_Version = c.String(),
                        ReestrFile_ChangeDate = c.String(),
                        Platform = c.String(),
                        CompatibleOSs = c.String(),
                        CompatibleScadas = c.String(),
                        CompatibleSZI = c.String(),
                        OtherSoft = c.String(),
                        IdentificationType = c.String(),
                        AuthorizationType = c.String(),
                        UserCategories = c.String(),
                        LocalData = c.String(),
                        SUBD = c.String(),
                        SUBDExt = c.String(),
                        DataStoringMechanism = c.String(),
                        IDE = c.String(),
                        FunctionalComponents = c.String(),
                        BuildingComponents = c.String(),
                        Report = c.String(),
                        Propagation = c.String(),
                        Installer = c.String(),
                        LicenseType = c.String(),
                        ParentId = c.Int(nullable: false),
                        Selected = c.Boolean(nullable: false),
                        UpToDate = c.Boolean(nullable: false),
                        IsInReestr = c.Boolean(nullable: false),
                        Blocked = c.Boolean(nullable: false),
                        WorkingStatus = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ControlledProjects", t => t.ParentId, cascadeDelete: true)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.ControlledProjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ReleaseDirectory = c.String(),
                        WorkingDirectory = c.String(),
                        DocumentDirectory = c.String(),
                        Category = c.Int(nullable: false),
                        Task = c.Int(nullable: false),
                        AllApрsAreUpToDate = c.Boolean(nullable: false),
                        AllAppsAreInReestr = c.Boolean(nullable: false),
                        Blocked = c.Boolean(nullable: false),
                        WorkStatus = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SourceTextFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        Owner = c.String(),
                        Size = c.String(),
                        Hash = c.String(),
                        Version = c.String(),
                        CreationDate = c.String(),
                        ParentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ControlledProjects", t => t.ParentId, cascadeDelete: true)
                .Index(t => t.ParentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SourceTextFiles", "ParentId", "dbo.ControlledProjects");
            DropForeignKey("dbo.ControlledApps", "ParentId", "dbo.ControlledProjects");
            DropIndex("dbo.SourceTextFiles", new[] { "ParentId" });
            DropIndex("dbo.ControlledApps", new[] { "ParentId" });
            DropTable("dbo.SourceTextFiles");
            DropTable("dbo.ControlledProjects");
            DropTable("dbo.ControlledApps");
        }
    }
}
