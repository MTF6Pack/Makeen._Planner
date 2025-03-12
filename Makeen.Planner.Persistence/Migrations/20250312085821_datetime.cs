using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Datetime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Tasks SET StartTime = DATEADD(HOUR, -DATEDIFF(HOUR, GETUTCDATE(), GETDATE()), StartTime)");
            migrationBuilder.Sql("UPDATE Tasks SET DeadLine = DATEADD(HOUR, -DATEDIFF(HOUR, GETUTCDATE(), GETDATE()), DeadLine)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
