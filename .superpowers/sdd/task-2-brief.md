### Task 2: Create IImportService + ImportResult Models

**Files:**
- Create: `Flower.Backend/Services/Interfaces/IImportService.cs`
- Create: `Flower.Backend/Models/DTOs/ImportDTOs.cs`

**Interfaces:**
- Produces: `IImportService` (interface), `ImportResult`, `ImportError` (models)

- [ ] **Step 1: Create ImportDTOs**

`Flower.Backend/Models/DTOs/ImportDTOs.cs`:

```csharp
namespace Flower.Backend.Models.DTOs;

public class ImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<ImportError> Errors { get; set; } = new();
    public List<string> SkippedSkus { get; set; } = new();
}

public class ImportError
{
    public int RowIndex { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class ImportViewModel
{
    public ImportResult? Result { get; set; }
}
```

- [ ] **Step 2: Create IImportService interface**

`Flower.Backend/Services/Interfaces/IImportService.cs`:

```csharp
using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace Flower.Backend.Services.Interfaces;

public interface IImportService
{
    Task<ImportResult> ImportProductsAsync(
        IFormFile excelFile,
        IFormFile? zipFile,
        string onDuplicate);
}
```

- [ ] **Step 3: Commit**

```bash
git add Flower.Backend/Services/Interfaces/IImportService.cs Flower.Backend/Models/DTOs/ImportDTOs.cs
git commit -m "feat: add IImportService interface and ImportResult models"
```

---


