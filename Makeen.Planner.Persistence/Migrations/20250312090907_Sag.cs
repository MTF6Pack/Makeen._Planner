using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Task SET StartTime = DATEADD(HOUR, -DATEDIFF(HOUR, GETUTCDATE(), GETDATE()), StartTime)");
            migrationBuilder.Sql("UPDATE Task SET DeadLine = DATEADD(HOUR, -DATEDIFF(HOUR, GETUTCDATE(), GETDATE()), DeadLine)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
