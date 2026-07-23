### Task 1: Backend DTOs — LayoutDTOs.cs

**Files:**
- Create: `Flower.Backend/Models/DTOs/LayoutDTOs.cs`

**Produces:** `HeaderLayoutDTO`, `FooterColumnDTO`, `MenuItemDTO`, `FooterLinkDTO` — consumed by Tasks 2, 3, 4.

- [ ] **Step 1: Create LayoutDTOs.cs**

```csharp
using System.Text.Json.Serialization;

namespace Flower.Backend.Models.DTOs;

public class HeaderLayoutDTO
{
    public TopBarDTO TopBar { get; set; } = new();
    public ZonesDTO Zones { get; set; } = new();
    public CtaButtonDTO CtaButton { get; set; } = new();
    public HotlineConfigDTO Hotline { get; set; } = new();
    public SearchConfigDTO Search { get; set; } = new();
    public List<MenuItemDTO> MenuItems { get; set; } = new();
}

public class TopBarDTO
{
    public bool IsActive { get; set; }
    public string? Text { get; set; }
    public string? Url { get; set; }
}

public class ZonesDTO
{
    [JsonPropertyName("left")]
    public List<string> Left { get; set; } = new();
    [JsonPropertyName("center")]
    public List<string> Center { get; set; } = new();
    [JsonPropertyName("right")]
    public List<string> Right { get; set; } = new();
}

public class CtaButtonDTO
{
    public bool IsActive { get; set; }
    public string? Text { get; set; }
    public string? Url { get; set; }
    public string? Variant { get; set; }
}

public class HotlineConfigDTO
{
    public bool UseDefault { get; set; } = true;
    public string? CustomText { get; set; }
}

public class SearchConfigDTO
{
    public string Mode { get; set; } = "popup";
}

public class MenuItemDTO
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsExternal { get; set; }
    public List<MenuItemDTO> Children { get; set; } = new();
}

public class FooterColumnDTO
{
    public string Title { get; set; } = string.Empty;
    public string Align { get; set; } = "left";
    public int SortOrder { get; set; }
    public string Type { get; set; } = "links";
    public bool IsActive { get; set; } = true;
    public List<FooterLinkDTO> Links { get; set; } = new();
}

public class FooterLinkDTO
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "custom"; // "page" | "custom" | "text_block"
    public int? PageId { get; set; }
    public string? Url { get; set; }
}
```

- [ ] **Step 2: Build to verify compilation**

Run: `cd Flower.Backend && dotnet build`
Expected: `Build succeeded`

- [ ] **Step 3: Commit**

```bash
git add Flower.Backend/Models/DTOs/LayoutDTOs.cs
git commit -m "feat: add LayoutDTOs for dynamic header/footer"
```

---


