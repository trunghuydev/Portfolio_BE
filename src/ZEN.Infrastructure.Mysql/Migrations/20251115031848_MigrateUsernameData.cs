using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZEN.Infrastructure.Mysql.Migrations
{
    /// <inheritdoc />
    public partial class MigrateUsernameData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migrate existing users: Create username from email or UserName
            migrationBuilder.Sql(@"
                UPDATE ""AspNetUsers""
                SET ""username"" = LOWER(SUBSTRING(""Email"" FROM 1 FOR POSITION('@' IN ""Email"") - 1))
                WHERE ""username"" IS NULL AND ""Email"" IS NOT NULL;
            ");

            // For users without email, use UserName
            migrationBuilder.Sql(@"
                UPDATE ""AspNetUsers""
                SET ""username"" = LOWER(""UserName"")
                WHERE ""username"" IS NULL AND ""UserName"" IS NOT NULL;
            ");

            // For remaining users, use Id prefix
            migrationBuilder.Sql(@"
                UPDATE ""AspNetUsers""
                SET ""username"" = LOWER('user_' || SUBSTRING(""Id"" FROM 1 FOR 8))
                WHERE ""username"" IS NULL;
            ");

            // Generate slugs from usernames
            migrationBuilder.Sql(@"
                UPDATE ""AspNetUsers""
                SET ""slug"" = LOWER(REPLACE(""username"", '_', '-'))
                WHERE ""slug"" IS NULL AND ""username"" IS NOT NULL;
            ");

            // Set all existing users as public by default (if not already set)
            migrationBuilder.Sql(@"
                UPDATE ""AspNetUsers""
                SET ""is_public"" = true
                WHERE ""is_public"" IS NULL OR ""is_public"" = false;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert username migration (set to NULL)
            migrationBuilder.Sql(@"
                UPDATE ""AspNetUsers""
                SET ""username"" = NULL, ""slug"" = NULL
                WHERE ""username"" IS NOT NULL;
            ");
        }
    }
}
