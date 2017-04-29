namespace BookPortal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Some : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Author = c.String(nullable: false),
                        Country = c.String(nullable: false),
                        Language = c.String(nullable: false),
                        Genre = c.String(nullable: false),
                        Publisher = c.String(),
                        Published = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Books");
        }
    }
}
