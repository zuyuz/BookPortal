namespace BookPortal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PathToFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "PathToFile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Books", "PathToFile");
        }
    }
}
