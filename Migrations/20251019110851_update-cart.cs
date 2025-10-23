using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBookStore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update Pages table - this can stay as is
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // === Cart Table Reconstruction ===

            // Step 1: Create temporary table with new structure
            migrationBuilder.Sql(@"
                CREATE TABLE [dbo].[Carts_Temp] (
                    [ID] INT IDENTITY(1,1) NOT NULL,
                    [ProductID] INT NOT NULL,
                    [UserID] INT NOT NULL,
                    [Quantity] INT NOT NULL,
                    CONSTRAINT [PK_Carts_Temp] PRIMARY KEY ([ID])
                );
            ");

            // Step 2: Copy existing data to temp table
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Carts')
                BEGIN
                    INSERT INTO [dbo].[Carts_Temp] ([ProductID], [UserID], [Quantity])
                    SELECT [ProductID], [UserID], [Quantity]
                    FROM [dbo].[Carts];
                END
            ");

            // Step 3: Drop old table (this will also drop foreign keys)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Carts')
                BEGIN
                    DROP TABLE [dbo].[Carts];
                END
            ");

            // Step 4: Rename temp table to original name
            migrationBuilder.Sql(@"
                EXEC sp_rename '[dbo].[Carts_Temp]', 'Carts';
            ");

            // Step 5: Recreate foreign keys
            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[Carts]
                ADD CONSTRAINT [FK_Carts_Products_ProductID] 
                FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Products]([ID]) ON DELETE CASCADE;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[Carts]
                ADD CONSTRAINT [FK_Carts_Users_UserID] 
                FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([ID]) ON DELETE CASCADE;
            ");

            // Step 6: Create index on UserID
            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserID",
                table: "Carts",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert Pages table change
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // === Revert Cart Table to Composite Key ===

            // Drop index
            migrationBuilder.DropIndex(
                name: "IX_Carts_UserID",
                table: "Carts");

            // Create temp table with composite key structure
            migrationBuilder.Sql(@"
                CREATE TABLE [dbo].[Carts_Temp] (
                    [ProductID] INT NOT NULL,
                    [UserID] INT NOT NULL,
                    [Quantity] INT NOT NULL,
                    CONSTRAINT [PK_Carts_Temp] PRIMARY KEY ([UserID], [ProductID])
                );
            ");

            // Copy data back (excluding ID)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Carts')
                BEGIN
                    INSERT INTO [dbo].[Carts_Temp] ([ProductID], [UserID], [Quantity])
                    SELECT [ProductID], [UserID], [Quantity]
                    FROM [dbo].[Carts];
                END
            ");

            // Drop new table
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Carts')
                BEGIN
                    DROP TABLE [dbo].[Carts];
                END
            ");

            // Rename temp to original
            migrationBuilder.Sql(@"
                EXEC sp_rename '[dbo].[Carts_Temp]', 'Carts';
            ");

            // Recreate foreign keys
            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[Carts]
                ADD CONSTRAINT [FK_Carts_Products_ProductID] 
                FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Products]([ID]) ON DELETE CASCADE;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[Carts]
                ADD CONSTRAINT [FK_Carts_Users_UserID] 
                FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([ID]) ON DELETE CASCADE;
            ");
        }
    }
}