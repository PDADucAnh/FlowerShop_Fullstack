# Task 1: Backend Entities, DTOs & Migration

## Global Constraints
- All new DB fields must be nullable (optional image/avatar)
- Follow existing entity patterns (see CategoryProduct.cs, User.cs, Customer.cs)
- No changes to existing fields or relationships

## Context
This task adds image/avatar fields to 3 entities and updates their DTOs + mapping. The project is a .NET 8/C# app with EF Core.

## Files to Modify

### Entities
1. `Flower.Data/Entities/CategoryProduct.cs` — Add `[MaxLength(2000)] public string? ImageUrl { get; set; }` after `Slug`
2. `Flower.Data/Entities/User.cs` — Add `[MaxLength(2000)] public string? Avatar { get; set; }`
3. `Flower.Data/Entities/Customer.cs` — Add `[MaxLength(2000)] public string? Avatar { get; set; }`

### DTOs
4. `Flower.Backend/Models/DTOs/CategoryProductDTOs.cs`:
   - `CategoryProductDTO`: add `public string? ImageUrl { get; set; }`
   - `CreateCategoryProductDTO`: add `[MaxLength(2000)] public string? ImageUrl { get; set; }`
   - `UpdateCategoryProductDTO`: add `[MaxLength(2000)] public string? ImageUrl { get; set; }`

5. `Flower.Backend/Models/DTOs/UserDTOs.cs`:
   - `UserDTO`: add `public string? Avatar { get; set; }`
   - `CreateUserRequest`: add `public string? Avatar { get; set; }`
   - `UpdateUserRequest`: add `public string? Avatar { get; set; }`

6. `Flower.Backend/Models/DTOs/CustomerDTOs.cs`:
   - `CustomerDTO`: add `public string? Avatar { get; set; }`

### Mapping Extensions
7. `Flower.Backend/Models/DTOs/MappingExtensions.cs`:
   - `CategoryProduct ToDTO()`: add `ImageUrl = categoryProduct.ImageUrl`
   - `CategoryProduct ToEntity(CreateCategoryProductDTO)`: add `ImageUrl = dto.ImageUrl`
   - `UpdateEntity(UpdateCategoryProductDTO, CategoryProduct)`: add `entity.ImageUrl = dto.ImageUrl;`
   - `UserDTO ToDTO(User)`: add `Avatar = user.Avatar`
   - `User ToEntity(CreateUserDTO)`: add `Avatar = dto.Avatar`
   - `UpdateEntity(UpdateUserDTO, User)`: add `entity.Avatar = dto.Avatar;`
   - `CustomerDTO ToDTO(Customer)`: add `Avatar = customer.Avatar`

### Migration (auto-generated)
8. Run: `cd Flower.Backend && dotnet ef migrations add AddCategoryImageAndAvatar`
9. Run: `dotnet build` — verify 0 errors

## Deliverables
- All entities updated with new fields
- All DTOs updated
- Mapping extensions updated
- Migration created and building
- Commit message: `feat: add ImageUrl to CategoryProduct, Avatar to User/Customer`
