# Plan A: FlashSale Admin MVC Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Create the missing FlashSale admin MVC controller and views, plus sidebar link.

**Architecture:** Follow exact pattern of existing `PromotionController` / `CouponController` — same auth attributes, same DI pattern, same view design tokens, same notification pattern.

**Tech Stack:** ASP.NET Core 8 MVC, Razor views, Tailwind CSS, Material Symbols

## Global Constraints

- All admin controllers use `[Authorize(Policy = "StaffOnly")]` at class level
- CRUD actions requiring admin use `[Authorize(Policy = "AdminOnly")]`
- Inject `IFlashSaleService` + `INotificationService` via constructor
- Use `CreateFlashSaleDTO` / `UpdateFlashSaleDTO` from existing `FlashSaleDTOs.cs`
- Views use `_LayoutAdmin` layout
- Vietnamese labels in UI
- `TempData["Success"]` / `TempData["Error"]` for flash messages
- Notify entity change: `_notificationService.NotifyEntityChanged("FlashSale")`
- Follow Promotion's pattern: use `TempData` for messages, `RedirectToAction("Index")` after POST, check `ModelState.IsValid`

---

### Task 1: Create FlashSaleController

**Files:**
- Create: `Flower.Backend/Controllers/FlashSaleController.cs`

**Interfaces:**
- Consumes: `IFlashSaleService` (existing interface with `GetAll()`, `GetById()`, `Create()`, `Update()`, `Delete()`), `INotificationService`
- Produces: Controller actions for Index, Create, Edit, Delete, ToggleActive

- [ ] **Step 1: Create FlashSaleController**

```csharp
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class FlashSaleController : Controller
    {
        private readonly IFlashSaleService _flashSaleService;
        private readonly INotificationService _notificationService;

        public FlashSaleController(IFlashSaleService flashSaleService, INotificationService notificationService)
        {
            _flashSaleService = flashSaleService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _flashSaleService.GetAll();
            return View(items);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(CreateFlashSaleDTO model, string? productIdsCsv)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(productIdsCsv))
            {
                model.Products = productIdsCsv
                    .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => { int.TryParse(s.Trim(), out var id); return id; })
                    .Where(id => id > 0)
                    .Select(id => new CreateFlashSaleProductDTO { ProductId = id, SalePrice = 0 })
                    .ToList();
            }

            try
            {
                await _flashSaleService.Create(model);
                await _notificationService.NotifyEntityChanged("FlashSale");
                TempData["Success"] = "Flash Sale đã được tạo thành công.";
                return RedirectToAction("Index");
            }
            catch (System.InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _flashSaleService.GetById(id);
            if (item == null) return NotFound();

            var model = new UpdateFlashSaleDTO
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                IsActive = item.IsActive
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(UpdateFlashSaleDTO model, string? productIdsCsv)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            var existing = await _flashSaleService.GetById(model.Id);
            if (existing == null)
            {
                TempData["Error"] = "Flash Sale không tồn tại.";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrWhiteSpace(productIdsCsv))
            {
                model.Products = productIdsCsv
                    .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => { int.TryParse(s.Trim(), out var id); return id; })
                    .Where(id => id > 0)
                    .Select(id => new CreateFlashSaleProductDTO { ProductId = id, SalePrice = 0 })
                    .ToList();
            }
            else
            {
                model.Products = null;
            }

            var updated = await _flashSaleService.Update(model.Id, model);
            if (!updated)
            {
                TempData["Error"] = "Không thể cập nhật Flash Sale. Vui lòng thử lại.";
                return View(model);
            }

            await _notificationService.NotifyEntityChanged("FlashSale");
            TempData["Success"] = "Flash Sale đã được cập nhật.";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _flashSaleService.Delete(id);
            if (!deleted)
            {
                TempData["Error"] = "Không thể xóa Flash Sale.";
                return RedirectToAction("Index");
            }
            await _notificationService.NotifyEntityChanged("FlashSale");
            TempData["Success"] = "Flash Sale đã được xóa.";
            return RedirectToAction("Index");
        }
    }
}
```

- [ ] **Step 2: Verify build**

```bash
dotnet build
```

Expected: Build succeeds with no errors.

---

### Task 2: Create FlashSale Index View

**Files:**
- Create: `Flower.Backend/Views/FlashSale/Index.cshtml`

**Interfaces:**
- Consumes: `IEnumerable<FlashSaleDTO>` from controller

- [ ] **Step 1: Create Index.cshtml**

```html
@using Flower.Backend.Utils
@model IEnumerable<FlashSaleDTO>
@{
    ViewData["Title"] = "Flash Sale";
    Layout = "_LayoutAdmin";
    var now = DateTimeUtils.GetVietnamTime();
}

<div class="space-y-stack-lg">
    <div class="flex flex-col md:flex-row md:items-end justify-between gap-4">
        <div>
            <h2 class="font-display-lg text-display-lg text-on-background mb-2">Flash Sale</h2>
            <p class="font-body-lg text-body-lg text-on-surface-variant max-w-2xl">Quản lý các chương trình Flash Sale.</p>
        </div>
        @if (User.IsInRole("Admin"))
        {
            <div>
                <a asp-action="Create" class="flex items-center gap-2 px-4 py-2 bg-primary text-on-primary rounded-lg font-label-md text-label-md hover:bg-primary/90 transition-colors shadow-sm no-underline">
                    <span class="material-symbols-outlined text-[18px]">add</span>
                    Tạo Flash Sale
                </a>
            </div>
        }
    </div>

    <div class="bg-surface-container-lowest border border-outline-variant/30 rounded-xl overflow-hidden">
        <table class="w-full text-left">
            <thead>
                <tr class="border-b border-outline-variant/20">
                    <th class="px-4 py-4 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-widest">Tên</th>
                    <th class="px-4 py-4 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-widest">Ngày bắt đầu</th>
                    <th class="px-4 py-4 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-widest">Ngày kết thúc</th>
                    <th class="px-4 py-4 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-widest">Trạng thái</th>
                    @if (User.IsInRole("Admin"))
                    {
                        <th class="px-4 py-4 font-label-sm text-label-sm text-on-surface-variant uppercase tracking-widest text-right">Thao tác</th>
                    }
                </tr>
            </thead>
            <tbody>
                @foreach (var item in Model)
                {
                    <tr class="border-b border-outline-variant/10 hover:bg-surface-container-low transition-colors">
                        <td class="px-4 py-4">
                            <p class="font-body-md text-body-md text-on-surface font-semibold">@item.Name</p>
                            @if (!string.IsNullOrEmpty(item.Description))
                            {
                                <p class="font-label-sm text-label-sm text-on-surface-variant line-clamp-1">@item.Description</p>
                            }
                        </td>
                        <td class="px-4 py-4 font-label-sm text-label-sm text-on-surface-variant">
                            @item.StartDate.ToString("dd/MM/yyyy HH:mm")
                        </td>
                        <td class="px-4 py-4 font-label-sm text-label-sm text-on-surface-variant">
                            @item.EndDate.ToString("dd/MM/yyyy HH:mm")
                        </td>
                        <td class="px-4 py-4">
                            @if (item.IsActive && item.StartDate <= now && item.EndDate >= now)
                            {
                                <span class="inline-flex items-center gap-1 px-3 py-1 bg-[#E8F5E9] text-[#2E7D32] rounded-full font-label-sm text-label-sm">
                                    <span class="w-1.5 h-1.5 rounded-full bg-[#2E7D32]"></span>
                                    Đang chạy
                                </span>
                            }
                            else if (item.IsActive && item.StartDate > now)
                            {
                                <span class="inline-flex items-center gap-1 px-3 py-1 bg-blue-50 text-blue-700 rounded-full font-label-sm text-label-sm">
                                    <span class="w-1.5 h-1.5 rounded-full bg-blue-500"></span>
                                    Sắp diễn ra
                                </span>
                            }
                            else if (item.IsActive && item.EndDate < now)
                            {
                                <span class="inline-flex items-center gap-1 px-3 py-1 bg-yellow-50 text-yellow-700 rounded-full font-label-sm text-label-sm">
                                    <span class="w-1.5 h-1.5 rounded-full bg-yellow-500"></span>
                                    Đã kết thúc
                                </span>
                            }
                            else
                            {
                                <span class="inline-flex items-center gap-1 px-3 py-1 bg-surface-variant text-on-surface-variant rounded-full font-label-sm text-label-sm">
                                    <span class="w-1.5 h-1.5 rounded-full bg-on-surface-variant"></span>
                                    Tạm dừng
                                </span>
                            }
                        </td>
                        @if (User.IsInRole("Admin"))
                        {
                            <td class="px-4 py-4 text-right">
                                <div class="flex gap-2 justify-end">
                                    <a asp-action="Edit" asp-route-id="@item.Id" class="px-3 py-2 border border-outline-variant rounded-lg text-primary font-label-sm text-label-sm hover:bg-primary-container/10 transition-colors no-underline">Sửa</a>
                                    <a asp-action="Delete" asp-route-id="@item.Id" class="px-3 py-2 border border-outline-variant rounded-lg text-error font-label-sm text-label-sm hover:bg-error-container/30 transition-colors no-underline" onclick="return confirm('Xóa Flash Sale này?')">Xóa</a>
                                </div>
                            </td>
                        }
                    </tr>
                }
                @if (!Model.Any())
                {
                    <tr>
                        <td colspan="5" class="px-4 py-12 text-center">
                            <span class="material-symbols-outlined text-4xl text-outline mb-2 block">local_fire_department</span>
                            <p class="font-body-md text-body-md text-on-surface-variant">Chưa có chương trình Flash Sale nào.</p>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>
```

- [ ] **Step 2: Verify build**

```bash
dotnet build
```

Expected: Build succeeds.

---

### Task 3: Create FlashSale Create View

**Files:**
- Create: `Flower.Backend/Views/FlashSale/Create.cshtml`

**Interfaces:**
- Consumes: `CreateFlashSaleDTO` from controller

- [ ] **Step 1: Create Create.cshtml**

```html
@model CreateFlashSaleDTO
@{
    ViewData["Title"] = "Thêm Flash Sale";
    Layout = "_LayoutAdmin";
}

<div class="space-y-lg max-w-4xl mx-auto pb-12">
    <div class="flex justify-between items-center">
        <div>
            <h3 class="text-label-sm uppercase tracking-[0.3em] text-secondary">Flash Sale</h3>
            <p class="serif text-3xl font-bold">Flash Sale mới</p>
        </div>
        <a asp-action="Index" class="text-label-sm uppercase tracking-widest font-bold flex items-center gap-2 hover:text-primary transition-colors">
            <span class="material-symbols-outlined text-lg">arrow_back</span>
            Quay lại
        </a>
    </div>

    <form asp-action="Create" method="post" id="flashsale-form" class="bg-surface-container-lowest border border-outline-variant p-xl space-y-xl rounded-xl shadow-sm">
        <div class="space-y-md">
            <label class="text-label-sm uppercase tracking-widest text-secondary font-bold">Tên chương trình</label>
            <input asp-for="Name" class="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg focus:ring-1 focus:ring-primary px-lg py-4 serif text-2xl font-bold placeholder:text-outline-variant" placeholder="Flash Sale Cuối Tuần..." required />
            <span asp-validation-for="Name" class="text-error text-xs uppercase tracking-widest block mt-1"></span>
        </div>

        <div class="space-y-md">
            <label class="text-label-sm uppercase tracking-widest text-secondary font-bold">Mô tả</label>
            <textarea asp-for="Description" rows="3" class="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg focus:ring-1 focus:ring-primary px-lg py-4 text-body-md italic leading-relaxed placeholder:text-outline-variant" placeholder="Mô tả chương trình Flash Sale..."></textarea>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-xl">
            <div class="space-y-md">
                <label class="text-label-sm uppercase tracking-widest text-secondary font-bold">Trạng thái</label>
                <select asp-for="IsActive" class="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg focus:ring-1 focus:ring-primary px-lg py-3">
                    <option value="true">Active (Hoạt động)</option>
                    <option value="false">Draft (Bản nháp)</option>
                </select>
            </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-xl">
            <div class="space-y-md">
                <label class="text-label-sm uppercase tracking-widest text-secondary font-bold">Bắt đầu</label>
                <input asp-for="StartDate" type="datetime-local" class="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg focus:ring-1 focus:ring-primary px-lg py-3" />
                <span asp-validation-for="StartDate" class="text-error text-xs uppercase tracking-widest block mt-1"></span>
            </div>
            <div class="space-y-md">
                <label class="text-label-sm uppercase tracking-widest text-secondary font-bold">Kết thúc</label>
                <input asp-for="EndDate" type="datetime-local" class="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg focus:ring-1 focus:ring-primary px-lg py-3" />
                <span asp-validation-for="EndDate" class="text-error text-xs uppercase tracking-widest block mt-1"></span>
            </div>
        </div>

        <div class="space-y-md">
            <div class="flex justify-between items-center">
                <label class="text-label-sm uppercase tracking-widest text-secondary font-bold">Sản phẩm áp dụng</label>
            </div>
            <input type="hidden" id="productIdsCsv" name="productIdsCsv" value="" />
            <div id="product-selection-container" class="space-y-sm">
                <div class="relative">
                    <span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant text-[20px]">search</span>
                    <input type="text" id="product-search-input" class="w-full pl-10 pr-4 py-3 bg-surface-container-low border border-outline-variant/30 rounded-lg focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary placeholder:text-outline-variant" placeholder="Nhập tên sản phẩm hoặc SKU để tìm..." />
                    <div id="product-search-results" class="absolute left-0 right-0 mt-1 max-h-60 overflow-y-auto bg-surface border border-outline-variant rounded-lg shadow-lg z-30 hidden no-scrollbar"></div>
                </div>
                <div class="bg-surface-container-low/60 border border-outline-variant/30 rounded-lg p-sm">
                    <div class="flex justify-between items-center mb-2">
                        <p class="text-xs text-on-surface-variant/80 font-bold uppercase tracking-wider">Sản phẩm đã chọn (<span id="selected-count">0</span>)</p>
                        <button type="button" id="clear-all-products" class="text-error hover:text-error/80 text-xs font-bold uppercase tracking-wider flex items-center gap-1 transition-colors hidden">
                            <span class="material-symbols-outlined text-sm">close</span> Xóa tất cả
                        </button>
                    </div>
                    <div id="selected-products-list" class="divide-y divide-outline-variant/20 max-h-80 overflow-y-auto no-scrollbar">
                        <p id="no-products-placeholder" class="text-sm text-on-surface-variant/60 italic py-3 text-center">Chưa có sản phẩm nào được chọn.</p>
                    </div>
                </div>
            </div>
        </div>

        <div class="pt-lg border-t border-outline-variant flex gap-md">
            <button type="submit" id="submit-btn" class="bg-primary text-on-primary px-xl py-4 text-label-sm uppercase tracking-[0.2em] font-bold hover:bg-neutral-800 transition-all flex-1 rounded-lg">
                Tạo Flash Sale
            </button>
            <a asp-action="Index" class="border border-outline px-xl py-4 text-label-sm uppercase tracking-[0.2em] font-bold hover:bg-surface-container transition-all rounded-lg">
                Hủy
            </a>
        </div>
    </form>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}

    <script>
        $(document).ready(function () {
            if ($.validator) {
                $.validator.addMethod("endDateAfterStartDate", function(value, element) {
                    var startDate = $('#StartDate').val();
                    if (!startDate || !value) return true;
                    return new Date(value) > new Date(startDate);
                }, "Ngày kết thúc phải lớn hơn ngày bắt đầu.");
                $('#EndDate').rules('add', { endDateAfterStartDate: true });
                $('#StartDate').on('change', function() {
                    if ($('#EndDate').val()) { $('#EndDate').valid(); }
                });
            }

            $('#flashsale-form').on('submit', function (e) {
                var form = $(this);
                if (form.valid()) {
                    var btn = $('#submit-btn');
                    btn.prop('disabled', true);
                    btn.addClass('opacity-70 cursor-not-allowed');
                    btn.html('<span class="flex items-center justify-center gap-2"><span class="animate-spin inline-block w-4 h-4 border-2 border-current border-t-transparent rounded-full"></span> Đang tạo...</span>');
                }
            });

            var allProducts = [];
            var selectedProductIds = [];
            var initialCsv = $('#productIdsCsv').val().trim();
            if (initialCsv) {
                selectedProductIds = initialCsv.split(',').map(x => parseInt(x.trim())).filter(x => !isNaN(x) && x > 0);
            }

            fetch('/api/Products').then(res => res.json()).then(data => { allProducts = data; renderSelectedProducts(); }).catch(err => console.error(err));

            var searchInput = $('#product-search-input');
            var searchResults = $('#product-search-results');
            searchInput.on('input focus', function () {
                var query = $(this).val().trim().toLowerCase();
                if (!query) { searchResults.addClass('hidden').empty(); return; }
                var matches = allProducts.filter(p => (p.name.toLowerCase().includes(query) || (p.sku && p.sku.toLowerCase().includes(query))) && !selectedProductIds.includes(p.id));
                renderSearchResults(matches);
            });

            $(document).on('click', function (e) {
                if (!$(e.target).closest('.relative').length) { searchResults.addClass('hidden'); }
            });

            function renderSearchResults(items) {
                searchResults.empty();
                if (items.length === 0) {
                    searchResults.append('<div class="p-3 text-sm text-on-surface-variant/60 italic text-center">Không tìm thấy sản phẩm phù hợp.</div>');
                    searchResults.removeClass('hidden'); return;
                }
                items.forEach(p => {
                    var priceFormatted = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p.price);
                    var imgHtml = p.imageUrl ? `<img src="${p.imageUrl}" class="w-8 h-8 rounded object-cover" />` : `<div class="w-8 h-8 rounded bg-secondary-container flex items-center justify-center text-primary"><span class="material-symbols-outlined text-sm">local_florist</span></div>`;
                    var skuText = p.sku ? `<span class="bg-surface-container-high px-1.5 py-0.5 rounded text-[10px] text-on-surface-variant/80 font-mono font-bold">SKU: ${p.sku}</span>` : '';
                    searchResults.append(`
                        <div class="flex items-center gap-3 p-3 hover:bg-primary-container/20 cursor-pointer transition-colors border-b border-outline-variant/10" data-id="${p.id}">
                            ${imgHtml}
                            <div class="flex-1 min-w-0">
                                <p class="text-sm font-semibold truncate text-on-surface">${p.name}</p>
                                <div class="flex items-center gap-2 mt-0.5">${skuText}<span class="text-xs text-secondary font-bold">${priceFormatted}</span></div>
                            </div>
                            <span class="material-symbols-outlined text-primary text-lg">add_circle</span>
                        </div>
                    `);
                });
                searchResults.removeClass('hidden');
                searchResults.find('[data-id]').on('click', function () {
                    var id = parseInt($(this).data('id'));
                    if (id && !selectedProductIds.includes(id)) { selectedProductIds.push(id); updateProductIdsCsv(); renderSelectedProducts(); }
                    searchInput.val(''); searchResults.addClass('hidden').empty();
                });
            }

            function renderSelectedProducts() {
                var container = $('#selected-products-list');
                var placeholder = $('#no-products-placeholder');
                var countSpan = $('#selected-count');
                var clearBtn = $('#clear-all-products');
                container.find('.product-selected-item').remove();
                countSpan.text(selectedProductIds.length);
                if (selectedProductIds.length === 0) { placeholder.removeClass('hidden'); clearBtn.addClass('hidden'); return; }
                placeholder.addClass('hidden'); clearBtn.removeClass('hidden');
                selectedProductIds.forEach(id => {
                    var p = allProducts.find(x => x.id === id); if (!p) return;
                    var priceFormatted = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p.price);
                    var imgHtml = p.imageUrl ? `<img src="${p.imageUrl}" class="w-10 h-10 rounded object-cover" />` : `<div class="w-10 h-10 rounded bg-secondary-container flex items-center justify-center text-primary"><span class="material-symbols-outlined text-[18px]">local_florist</span></div>`;
                    var skuText = p.sku ? `<span class="bg-surface-container-high px-1.5 py-0.5 rounded text-[10px] text-on-surface-variant/80 font-mono font-bold">SKU: ${p.sku}</span>` : '';
                    container.append(`
                        <div class="product-selected-item flex items-center gap-3 py-3 border-b border-outline-variant/10" data-selected-id="${p.id}">
                            ${imgHtml}
                            <div class="flex-1 min-w-0">
                                <p class="text-sm font-bold text-on-surface truncate">${p.name}</p>
                                <div class="flex items-center gap-2 mt-0.5">${skuText}<span class="text-xs text-secondary font-bold">${priceFormatted}</span></div>
                            </div>
                            <button type="button" class="remove-product-btn text-error hover:text-error/80 transition-colors p-1" data-remove-id="${p.id}"><span class="material-symbols-outlined text-[20px]">delete</span></button>
                        </div>
                    `);
                });
                $('.remove-product-btn').on('click', function () {
                    var removeId = parseInt($(this).data('remove-id'));
                    selectedProductIds = selectedProductIds.filter(id => id !== removeId);
                    updateProductIdsCsv(); renderSelectedProducts();
                });
            }

            $('#clear-all-products').on('click', function () { selectedProductIds = []; updateProductIdsCsv(); renderSelectedProducts(); });
            function updateProductIdsCsv() { $('#productIdsCsv').val(selectedProductIds.join(',')); }
        });
    </script>
}
```

---

### Task 4: Create FlashSale Edit View

**Files:**
- Create: `Flower.Backend/Views/FlashSale/Edit.cshtml`

**Interfaces:**
- Consumes: `UpdateFlashSaleDTO` from controller

- [ ] **Step 1: Create Edit.cshtml**

```html
@model UpdateFlashSaleDTO
@{
    ViewData["Title"] = "Sửa Flash Sale";
    Layout = "_LayoutAdmin";
}

<div class="space-y-lg max-w-4xl mx-auto pb-12">
    <div class="flex justify-between items-center">
        <div>
            <h3 class="text-label-sm uppercase tracking-[0.3em] text-secondary">Flash Sale</h3>
            <p class="serif text-3xl font-bold">Sửa Flash Sale</p>
        </div>
        <a asp-action="Index" class="text-label-sm uppercase tracking-widest font-bold flex items-center gap-2 hover:text-primary transition-colors">
            <span class="material-symbols-outlined text-lg">arrow_back</span>
            Quay lại
        </a>
    </div>

    <form asp-action="Edit" method="post" id="flashsale-form" class="bg-surface-container-lowest border border-outline-variant p-xl space-y-xl rounded-xl shadow-sm">
        <input asp-for="Id" type="hidden" />

        <div class="space-y-md">
            <label class="text-label-sm uppercase tracking-widest text-secondary font-bold">Tên chương trình</label>
            <input asp-for="Name" class="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg focus:ring-1 focus:ring-primary px-lg py-4 serif text-2xl font-bold placeholder:text-outline-variant" required />
            <span asp-validation-for="Name" class="text-error text-xs uppercase tracking-widest block mt-1"></span>
        </div>

        <div class="space-y-md">
            <label class="text-label-sm uppercase tracking-widest text-secondary font-bold">Mô tả</label>
            <textarea asp-for="Description" rows="3" class="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg focus:ring-1 focus:ring-primary px-lg py-4 text-body-md italic leading-relaxed placeholder:text-outline-variant"></textarea>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-xl">
            <div class="space-y-md">
                <label class="text-label-sm uppercase tracking-widest text-secondary font-bold">Trạng thái</label>
                <select asp-for="IsActive" class="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg focus:ring-1 focus:ring-primary px-lg py-3">
                    <option value="true">Active (Hoạt động)</option>
                    <option value="false">Draft (Bản nháp)</option>
                </select>
            </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-xl">
            <div class="space-y-md">
                <label class="text-label-sm uppercase tracking-widest text-secondary font-bold">Bắt đầu</label>
                <input asp-for="StartDate" type="datetime-local" class="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg focus:ring-1 focus:ring-primary px-lg py-3" />
            </div>
            <div class="space-y-md">
                <label class="text-label-sm uppercase tracking-widest text-secondary font-bold">Kết thúc</label>
                <input asp-for="EndDate" type="datetime-local" class="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg focus:ring-1 focus:ring-primary px-lg py-3" />
            </div>
        </div>

        <div class="pt-lg border-t border-outline-variant flex gap-md">
            <button type="submit" id="submit-btn" class="bg-primary text-on-primary px-xl py-4 text-label-sm uppercase tracking-[0.2em] font-bold hover:bg-neutral-800 transition-all flex-1 rounded-lg">
                Cập nhật
            </button>
            <a asp-action="Index" class="border border-outline px-xl py-4 text-label-sm uppercase tracking-[0.2em] font-bold hover:bg-surface-container transition-all rounded-lg">
                Hủy
            </a>
        </div>
    </form>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}

    <script>
        $(document).ready(function () {
            if ($.validator) {
                $.validator.addMethod("endDateAfterStartDate", function(value, element) {
                    var startDate = $('#StartDate').val();
                    if (!startDate || !value) return true;
                    return new Date(value) > new Date(startDate);
                }, "Ngày kết thúc phải lớn hơn ngày bắt đầu.");
                $('#EndDate').rules('add', { endDateAfterStartDate: true });
                $('#StartDate').on('change', function() {
                    if ($('#EndDate').val()) { $('#EndDate').valid(); }
                });
            }

            $('#flashsale-form').on('submit', function (e) {
                var form = $(this);
                if (form.valid()) {
                    var btn = $('#submit-btn');
                    btn.prop('disabled', true);
                    btn.addClass('opacity-70 cursor-not-allowed');
                    btn.html('<span class="flex items-center justify-center gap-2"><span class="animate-spin inline-block w-4 h-4 border-2 border-current border-t-transparent rounded-full"></span> Đang cập nhật...</span>');
                }
            });
        });
    </script>
}
```

---

### Task 5: Add FlashSale Link to Admin Sidebar

**File:**
- Modify: `Flower.Backend/Views/Shared/_LayoutAdmin.cshtml`

- [ ] **Step 1: Add FlashSale link after Coupon link**

Insert after line 239 (the Coupon link closing `</a>`):

```html
            <a class="flex items-center gap-3 px-4 py-3 rounded-lg font-label-md text-label-md @(controller == "FlashSale" ? "text-primary bg-secondary-container" : "text-on-surface-variant hover:text-primary hover:bg-primary-container/20") transition-all duration-200 no-underline"
               asp-controller="FlashSale" asp-action="Index">
                <span class="material-symbols-outlined @(controller == "FlashSale" ? "text-primary" : "")">local_fire_department</span>
                Flash Sale
            </a>
```

- [ ] **Step 2: Verify full build**

```bash
dotnet build
```

Expected: Build succeeds with no errors.
