using FluentMigrator;

namespace Persistence.Migrations
{
    [Migration(2025050901, "Create Employees Table")]
    public class CreateEmployeeTable : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
               CREATE TABLE IF NOT EXISTS Employees (
                    EmployeeID BIGSERIAL PRIMARY KEY,
                    LastName VARCHAR(20) NOT NULL,
                    FirstName VARCHAR(10) NOT NULL,
                    Title VARCHAR(30),
                    TitleOfCourtesy VARCHAR(25),
                    BirthDate TIMESTAMP,
                    HireDate TIMESTAMP,
                    Address VARCHAR(60),
                    City VARCHAR(15),
                    Region VARCHAR(15),
                    PostalCode VARCHAR(10),
                    Country VARCHAR(15),
                    HomePhone VARCHAR(24),
                    Extension VARCHAR(4),
                    Photo BYTEA,
                    Notes TEXT,
                    ReportsTo INT,
                    PhotoPath VARCHAR(255)
                );
            ");
        }

        public override void Down()
        {
            Execute.Sql("DROP TABLE IF EXISTS Employees;");
        }
    }
}