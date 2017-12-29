namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SampleTesting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Web.SampleTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Web.SampleTables");
        }
    }
}
