### Task 1: Add EPPlus NuGet Package + Create Excel Template

**Files:**
- Modify: `Flower.Backend/Flower.Backend.csproj`
- Create: `Flower.Backend/wwwroot/templates/product_import_template.xlsx`

- [ ] **Step 1: Add EPPlus package**

```bash
cd D:\TrenLop\ThucTapTaiTruong\FlowerShop
dotnet add Flower.Backend/Flower.Backend.csproj package EPPlus
```

Expected output: `PackageReference for 'EPPlus' added to file 'Flower.Backend/Flower.Backend.csproj'`

- [ ] **Step 2: Create template directory**

```bash
New-Item -ItemType Directory -Path "D:\TrenLop\ThucTapTaiTruong\FlowerShop\Flower.Backend\wwwroot\templates" -Force
```

- [ ] **Step 3: Create the Excel template programmatically via a small script**

Create `Flower.Backend/wwwroot/templates/product_import_template.xlsx` using EPPlus with headers:
`STT`, `TenSanPham`, `MaSanPham`, `GiaBan`, `SoLuongKho`, `DanhMucSlug`, `TenFileAnh`, `MoTa`

- In the controller, add a `DownloadTemplate` action that serves this file.

- [ ] **Step 4: Commit**

```bash
git add Flower.Backend/Flower.Backend.csproj Flower.Backend/wwwroot/templates/product_import_template.xlsx
git commit -m "chore: add EPPlus package and product import Excel template"
```

---


