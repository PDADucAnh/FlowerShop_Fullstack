# PLAN PREAMBLE (from docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md)

## Task 5: Data-preserving migration `RefactorAndRenameTables`

## Global Constraints
- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

---

## Task 5: Data-preserving migration `RefactorAndRenameTables`

**Files:**
- Generate: `Flower.Data/Migrations/<timestamp>_RefactorAndRenameTables.cs` (+ `.Designer.cs` + snapshot update)

**Interfaces:**
- Consumes: the Task 1 model (new entity names, `ProductVariant.Price`/`Sku`).
- Produces: a database where `PostCategories`, `ProductCategories`, `CustomerNotifications` exist with all rows preserved, and `ProductVariants` has `Price` + `Sku`.

- [ ] **Step 1: Generate the migration**

```powershell
dotnet ef migrations add RefactorAndRenameTables --project Flower.Data --startup-project Flower.Backend
```

> The generated `Up()` will (wrongly) `DropTable("Categories")` etc. because EF treats renamed CLR types as new entities. Do NOT run it yet.

- [ ] **Step 2: Replace the generated `Up()` body**

Open the generated `<timestamp>_RefactorAndRenameTables.cs` and replace the entire `Up()` body with:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameTable(
        name: "Categories",
        newName: "PostCategories");

    migrationBuilder.RenameColumn(
        name: "CategoryId",
        table: "Posts",
        newName: "PostCategoryId");

    migrationBuilder.RenameForeignKey(
        name: "FK_Posts_Categories_CategoryId",
        table: "Posts",
        newName: "FK_Posts_PostCategories_PostCategoryId");

    migrationBuilder.RenameIndex(
        name: "IX_Posts_CategoryId",
        table: "Posts",
        newName: "IX_Posts_PostCategoryId");

    migrationBuilder.RenameTable(
        name: "CategoriesProducts",
        newName: "ProductCategories");

    migrationBuilder.RenameColumn(
        name: "CategoryProductId",
        table: "Products",
        newName: "ProductCategoryId");

    migrationBuilder.RenameForeignKey(
        name: "FK_Products_CategoriesProducts_CategoryProductId",
        table: "Products",
        newName: "FK_Products_ProductCategories_ProductCategoryId");

    migrationBuilder.RenameIndex(
        name: "IX_Products_CategoryProductId",
        table: "Products",
        newName: "IX_Products_ProductCategoryId");

    migrationBuilder.RenameTable(
        name: "Notifications",
        newName: "CustomerNotifications");

    migrationBuilder.RenameForeignKey(
        name: "FK_Notifications_Customers_CustomerId",
        table: "CustomerNotifications",
        newName: "FK_CustomerNotifications_Customers_CustomerId");

    migrationBuilder.RenameIndex(
        name: "IX_Notifications_CustomerId",
        table: "CustomerNotifications",
        newName: "IX_CustomerNotifications_CustomerId");

    migrationBuilder.RenameIndex(
        name: "IX_Notifications_CustomerId_IsRead",
        table: "CustomerNotifications",
        newName: "IX_CustomerNotifications_CustomerId_IsRead");

    migrationBuilder.RenameColumn(
        name: "PriceAdjustment",
        table: "ProductVariants",
        newName: "Price");

    migrationBuilder.AddColumn<string>(
        name: "Sku",
        table: "ProductVariants",
        type: "character varying(50)",
        maxLength: 50,
        nullable: true);
}
```

> `RenameForeignKey`/`RenameIndex` take the table name as it exists at the time the operation runs — after the preceding `RenameTable`, the table is `CustomerNotifications`, so the FK/index renames above must pass `table: "CustomerNotifications"` (do NOT revert the note to the old name).

- [ ] **Step 3: Replace the generated `Down()` body**

```csharp
protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(
        name: "Sku",
        table: "ProductVariants");

    migrationBuilder.RenameColumn(
        name: "Price",
        table: "ProductVariants",
        newName: "PriceAdjustment");

    migrationBuilder.RenameIndex(
        name: "IX_CustomerNotifications_CustomerId_IsRead",
        table: "CustomerNotifications",
        newName: "IX_Notifications_CustomerId_IsRead");

    migrationBuilder.RenameIndex(
        name: "IX_CustomerNotifications_CustomerId",
        table: "CustomerNotifications",
        newName: "IX_Notifications_CustomerId");

    migrationBuilder.RenameForeignKey(
        name: "FK_CustomerNotifications_Customers_CustomerId",
        table: "CustomerNotifications",
        newName: "FK_Notifications_Customers_CustomerId");

    migrationBuilder.RenameTable(
        name: "CustomerNotifications",
        newName: "Notifications");

    migrationBuilder.RenameIndex(
        name: "IX_Products_ProductCategoryId",
        table: "Products",
        newName: "IX_Products_CategoryProductId");

    migrationBuilder.RenameForeignKey(
        name: "FK_Products_ProductCategories_ProductCategoryId",
        table: "Products",
        newName: "FK_Products_CategoriesProducts_CategoryProductId");

    migrationBuilder.RenameColumn(
        name: "ProductCategoryId",
        table: "Products",
        newName: "CategoryProductId");

    migrationBuilder.RenameTable(
        name: "ProductCategories",
        newName: "CategoriesProducts");

    migrationBuilder.RenameIndex(
        name: "IX_Posts_PostCategoryId",
        table: "Posts",
        newName: "IX_Posts_CategoryId");

    migrationBuilder.RenameForeignKey(
        name: "FK_Posts_PostCategories_PostCategoryId",
        table: "Posts",
        newName: "FK_Posts_Categories_CategoryId");

    migrationBuilder.RenameColumn(
        name: "PostCategoryId",
        table: "Posts",
        newName: "CategoryId");

    migrationBuilder.RenameTable(
        name: "PostCategories",
        newName: "Categories");
}
```

> Do NOT include the generated DropTable/CreateTable ops. Do NOT touch `ProductVariants.ProductId1` (a pre-existing harmless shadow FK — leave it).

- [ ] **Step 4: Review the generated `.Designer.cs`**

Confirm the designer's target model contains `Flower.Data.Entities.PostCategory`, `Flower.Data.Entities.ProductCategory`, `Flower.Data.Entities.CustomerNotification`, and `ProductVariant` with `Price` + `Sku`. If the designer still references old types, regenerate (`dotnet ef migrations remove` + re-add) rather than hand-editing.

- [ ] **Step 5: Back up DB, then apply**

```powershell
dotnet ef database update --project Flower.Data --startup-project Flower.Backend
```

- [ ] **Step 6: Verify data + schema**

Query Postgres and confirm: tables `PostCategories`, `ProductCategories`, `CustomerNotifications` exist; row counts match pre-rename; `Posts.PostCategoryId`/`Products.ProductCategoryId` populated; `ProductVariants` has `Price` (old values) and new nullable `Sku`.

- [ ] **Step 7: Commit**

```bash
git add Flower.Data/Migrations
git commit -m "feat: add RefactorAndRenameTables migration"
```

---