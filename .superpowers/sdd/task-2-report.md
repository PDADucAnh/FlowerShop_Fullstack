# Task 2 Report: Backend DTOs, Mappers, Service, UploadController, Image Endpoints

**Status:** DONE

**Commits:**
- `7d0a748` feat: extend product DTOs, add image upload/association endpoints

**Changes:**
- `ProductDTOs.cs`: Added ProductImageDTO, UploadImageResponse, AddProductImageRequest; extended CreateProductDTO, UpdateProductDTO with IsActive/FlowerMeaning/Origin/CareInstruction/NewImages; extended ProductDTO with Images/IsActive/FlowerMeaning/Origin/CareInstruction
- `MappingExtensions.cs`: Added ProductImage ToDTO mapping; updated Product ToDTO with Images + new fields; updated Create ToEntity, UpdateEntity with new fields
- `ProductService.cs`: BuildQuery includes Images; Create handles NewImages batch; Update handles NewImages append
- `UploadController.cs`: New — POST /api/Upload with image validation + Cloudinary upload
- `ProductsController.cs`: Added GET/POST/DELETE /{id}/images endpoints for image CRUD

**Build:** 0 errors

**Concerns:** None

**Next:** Task 3 — Frontend types + API functions
