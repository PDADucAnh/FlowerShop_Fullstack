### Task 4: Admin Views — Header Tab

**Files:**
- Create: `Flower.Backend/Views/Layout/Index.cshtml`
- Create: `Flower.Backend/Views/Layout/_HeaderTab.cshtml`

**Consumes:** `LayoutViewModel` (Task 3), `ViewBag.Pages`

- [ ] **Step 1: Create Index.cshtml**

```html
@model LayoutViewModel
@{
    ViewData["Title"] = "Quản lý giao diện";
    var activeTab = ViewBag.ActiveTab as string ?? "header";
}

<div class="max-w-6xl mx-auto py-6 px-4">
    <h1 class="text-3xl font-bold mb-6">Quản lý giao diện</h1>

    @if (TempData["Success"] != null)
    {
        <div class="bg-green-100 text-green-700 px-4 py-3 rounded-lg mb-4">@TempData["Success"]</div>
    }

    <!-- Tabs -->
    <div class="flex border-b border-outline-variant/30 mb-6">
        <a href="@Url.Action("Index", new { tab = "header" })"
           class="px-6 py-3 font-label-md @(activeTab == "header" ? "text-primary border-b-2 border-primary" : "text-on-surface-variant")">Trình đơn</a>
        <a href="@Url.Action("Index", new { tab = "footer" })"
           class="px-6 py-3 font-label-md @(activeTab == "footer" ? "text-primary border-b-2 border-primary" : "text-on-surface-variant")">Chân trang</a>
    </div>

    @if (activeTab == "header")
    {
        @await Html.PartialAsync("_HeaderTab", Model.Header)
    }
    else
    {
        @await Html.PartialAsync("_FooterTab", Model.Footer)
    }
</div>
```

- [ ] **Step 2: Create _HeaderTab.cshtml**

```html
@model HeaderLayoutDTO
@{
    var allComponents = new[] { "logo", "menu", "categories", "search", "cart", "wishlist", "account", "notification", "hotline", "ctaButton" };
    var zoneNames = new[] { "left", "center", "right" };
}

<form id="headerForm" method="post" action="@Url.Action("SaveHeader")">
    @Html.AntiForgeryToken()
    <input type="hidden" name="jsonPayload" id="headerJsonPayload" />

    <!-- Section 1: Top Bar -->
    <div class="bg-white rounded-xl shadow-sm border border-outline-variant/20 p-6 mb-6">
        <h3 class="text-lg font-bold mb-4">Thanh thông báo</h3>
        <label class="flex items-center gap-2 mb-4">
            <input type="checkbox" id="topBarActive" @(Model.TopBar.IsActive ? "checked" : "") class="w-4 h-4" />
            <span>Hiển thị thanh thông báo</span>
        </label>
        <div class="grid grid-cols-2 gap-4 topBar-fields" style="@(Model.TopBar.IsActive ? "" : "display:none")">
            <div>
                <label class="block font-label-md text-sm mb-1">Nội dung</label>
                <input type="text" id="topBarText" value="@Model.TopBar.Text" class="w-full px-3 py-2 border rounded-lg" />
            </div>
            <div>
                <label class="block font-label-md text-sm mb-1">Link (tùy chọn)</label>
                <input type="text" id="topBarUrl" value="@Model.TopBar.Url" class="w-full px-3 py-2 border rounded-lg" />
            </div>
        </div>
    </div>

    <!-- Section 2: Zones -->
    <div class="bg-white rounded-xl shadow-sm border border-outline-variant/20 p-6 mb-6">
        <h3 class="text-lg font-bold mb-4">Vùng hiển thị</h3>
        <div class="grid grid-cols-3 gap-6">
            @foreach (var zone in zoneNames)
            {
                var items = zone == "left" ? Model.Zones.Left : zone == "center" ? Model.Zones.Center : Model.Zones.Right;
                <div>
                    <label class="block font-label-md font-semibold mb-2">@(zone == "left" ? "Bên trái" : zone == "center" ? "Ở giữa" : "Bên phải")</label>
                    <div class="zone-sortable min-h-[120px] border-2 border-dashed border-outline-variant/30 rounded-lg p-3 space-y-2"
                         data-zone="@zone">
                        @foreach (var comp in items)
                        {
                            <div class="drag-item bg-primary-container text-primary px-3 py-2 rounded-lg text-sm cursor-move flex justify-between items-center" data-component="@comp">
                                <span>@comp</span>
                                <button type="button" class="remove-component text-red-500 hover:text-red-700">&times;</button>
                            </div>
                        }
                    </div>
                    <select class="add-component mt-2 w-full px-3 py-1.5 border rounded-lg text-sm">
                        <option value="">+ Thêm component</option>
                        @foreach (var comp in allComponents)
                        {
                            if (!items.Contains(comp))
                            {
                                <option value="@comp">@comp</option>
                            }
                        }
                    </select>
                </div>
            }
        </div>
    </div>

    <!-- Section 3: Component Configs -->
    <div class="bg-white rounded-xl shadow-sm border border-outline-variant/20 p-6 mb-6">
        <h3 class="text-lg font-bold mb-4">Cấu hình component</h3>

        <!-- Hotline config -->
        <div class="mb-4">
            <h4 class="font-semibold mb-2">Hotline</h4>
            <label class="flex items-center gap-2 mb-2">
                <input type="checkbox" id="hotlineUseDefault" @(Model.Hotline.UseDefault ? "checked" : "") class="w-4 h-4" />
                <span>Dùng số mặc định từ Thông tin cửa hàng</span>
            </label>
            <input type="text" id="hotlineCustom" value="@Model.Hotline.CustomText" placeholder="Số điện thoại riêng cho Header" class="w-full px-3 py-2 border rounded-lg" />
        </div>

        <!-- Search config -->
        <div class="mb-4">
            <h4 class="font-semibold mb-2">Tìm kiếm</h4>
            <label class="mr-4"><input type="radio" name="searchMode" value="popup" @(Model.Search.Mode == "popup" ? "checked" : "") class="mr-1" /> Popup</label>
            <label><input type="radio" name="searchMode" value="input" @(Model.Search.Mode == "input" ? "checked" : "") class="mr-1" /> Input inline</label>
        </div>

        <!-- CTA Button config -->
        <div class="mb-4">
            <h4 class="font-semibold mb-2">Button CTA</h4>
            <label class="flex items-center gap-2 mb-2">
                <input type="checkbox" id="ctaActive" @(Model.CtaButton.IsActive ? "checked" : "") class="w-4 h-4" />
                <span>Bật</span>
            </label>
            <div class="grid grid-cols-3 gap-4 cta-fields" style="@(Model.CtaButton.IsActive ? "" : "display:none")">
                <input type="text" id="ctaText" value="@Model.CtaButton.Text" placeholder="Nội dung" class="px-3 py-2 border rounded-lg" />
                <input type="text" id="ctaUrl" value="@Model.CtaButton.Url" placeholder="Link" class="px-3 py-2 border rounded-lg" />
                <select id="ctaVariant" class="px-3 py-2 border rounded-lg">
                    <option value="filled" @(Model.CtaButton.Variant == "filled" ? "selected" : "")>Filled</option>
                    <option value="outlined" @(Model.CtaButton.Variant == "outlined" ? "selected" : "")>Outlined</option>
                </select>
            </div>
        </div>
    </div>

    <!-- Section 4: Menu Tree -->
    <div class="bg-white rounded-xl shadow-sm border border-outline-variant/20 p-6 mb-6">
        <h3 class="text-lg font-bold mb-4">Menu điều hướng</h3>
        <div id="menuTree" class="dd">
            <ol class="dd-list">
                @foreach (var item in Model.MenuItems)
                {
                    <li class="dd-item" data-id="@item.Id">
                        <div class="dd-handle flex justify-between items-center px-3 py-2 border rounded-lg mb-1 bg-white">
                            <span class="menu-label">@item.Label</span>
                            <div class="flex gap-2">
                                <button type="button" class="edit-menu-item text-blue-500 text-sm">Sửa</button>
                                <button type="button" class="remove-menu-item text-red-500 text-sm">Xoá</button>
                            </div>
                        </div>
                        @if (item.Children.Any())
                        {
                            <ol class="dd-list">
                                @foreach (var child in item.Children)
                                {
                                    <li class="dd-item" data-id="@child.Id">
                                        <div class="dd-handle flex justify-between items-center px-3 py-2 border rounded-lg mb-1 bg-white ml-4">
                                            <span class="menu-label">@child.Label</span>
                                            <button type="button" class="remove-menu-item text-red-500 text-sm">Xoá</button>
                                        </div>
                                    </li>
                                }
                            </ol>
                        }
                    </li>
                }
            </ol>
        </div>
        <button type="button" id="addMenuItem" class="mt-3 px-4 py-2 bg-primary text-white rounded-lg text-sm">+ Thêm mục menu</button>
    </div>

    <div class="flex justify-end gap-3">
        <button type="reset" class="px-5 py-2.5 border border-outline rounded-lg text-primary bg-transparent">Làm lại</button>
        <button type="submit" class="px-5 py-2.5 bg-primary text-on-primary border-0 rounded-lg">Lưu cấu hình</button>
    </div>
</form>

@section Scripts {
    <script src="https://cdn.jsdelivr.net/npm/sortablejs@1.15.0/Sortable.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/nestable2@1.6.0/jquery.nestable.min.js"></script>
    <script>
        // SortableJS for zones
        document.querySelectorAll('.zone-sortable').forEach(el => {
            new Sortable(el, {
                group: 'zones',
                animation: 150,
                onEnd: updateAddComponentDropdowns
            });
        });

        // Add component to zone
        document.querySelectorAll('.add-component').forEach(select => {
            select.addEventListener('change', function() {
                const zone = this.closest('div').querySelector('.zone-sortable');
                const comp = this.value;
                if (!comp) return;
                const div = document.createElement('div');
                div.className = 'drag-item bg-primary-container text-primary px-3 py-2 rounded-lg text-sm cursor-move flex justify-between items-center';
                div.dataset.component = comp;
                div.innerHTML = '<span>' + comp + '</span><button type="button" class="remove-component text-red-500 hover:text-red-700">&times;</button>';
                zone.appendChild(div);
                div.querySelector('.remove-component').addEventListener('click', function() {
                    div.remove();
                    updateAddComponentDropdowns();
                });
                this.value = '';
                updateAddComponentDropdowns();
            });
        });

        // Remove component
        document.querySelectorAll('.remove-component').forEach(btn => {
            btn.addEventListener('click', function() {
                this.closest('.drag-item').remove();
                updateAddComponentDropdowns();
            });
        });

        function updateAddComponentDropdowns() {
            document.querySelectorAll('.zone-sortable').forEach(zone => {
                const select = zone.closest('div').querySelector('.add-component');
                const existing = new Set([...zone.querySelectorAll('.drag-item')].map(el => el.dataset.component));
                select.querySelectorAll('option').forEach(opt => {
                    if (opt.value) opt.style.display = existing.has(opt.value) ? 'none' : '';
                });
            });
        }

        // Nestable2 for menu tree
        $('#menuTree').nestable({
            maxDepth: 3,
            group: 1
        });

        // Serialize form on submit
        document.getElementById('headerForm').addEventListener('submit', function(e) {
            // Build JSON payload from DOM
            const payload = {
                topBar: { isActive: document.getElementById('topBarActive').checked, text: document.getElementById('topBarText').value, url: document.getElementById('topBarUrl').value || null },
                zones: {
                    left: [...document.querySelector('.zone-sortable[data-zone="left"]').querySelectorAll('.drag-item')].map(el => el.dataset.component),
                    center: [...document.querySelector('.zone-sortable[data-zone="center"]').querySelectorAll('.drag-item')].map(el => el.dataset.component),
                    right: [...document.querySelector('.zone-sortable[data-zone="right"]').querySelectorAll('.drag-item')].map(el => el.dataset.component)
                },
                ctaButton: { isActive: document.getElementById('ctaActive').checked, text: document.getElementById('ctaText').value || null, url: document.getElementById('ctaUrl').value || null, variant: document.getElementById('ctaVariant').value },
                hotline: { useDefault: document.getElementById('hotlineUseDefault').checked, customText: document.getElementById('hotlineCustom').value || null },
                search: { mode: document.querySelector('input[name="searchMode"]:checked').value },
                menuItems: buildMenuItemsFromTree()
            };
            document.getElementById('headerJsonPayload').value = JSON.stringify(payload);
        });

        function buildMenuItemsFromTree() {
            // Traverse Nestable2 tree and return array
            const items = [];
            document.querySelectorAll('#menuTree > ol.dd-list > li.dd-item').forEach(li => {
                items.push(buildMenuItem(li));
            });
            return items;
        }

        function buildMenuItem(li) {
            const labelEl = li.querySelector('.menu-label');
            const children = [];
            li.querySelectorAll(':scope > ol.dd-list > li.dd-item').forEach(childLi => {
                children.push(buildMenuItem(childLi));
            });
            return {
                id: li.dataset.id || crypto.randomUUID(),
                label: labelEl ? labelEl.textContent : '',
                url: li.dataset.url || '/',
                isExternal: li.dataset.external === 'true',
                children: children
            };
        }
    </script>
}
```

- [ ] **Step 3: Build to verify**

Run: `cd Flower.Backend && dotnet build`
Expected: `Build succeeded`

- [ ] **Step 4: Commit**

```bash
git add Flower.Backend/Views/Layout/
git commit -m "feat: add admin layout views (header tab) with SortableJS"
```

---


