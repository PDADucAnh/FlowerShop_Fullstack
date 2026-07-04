# Tài liệu Đặc tả Thiết kế: Khắc phục lỗi hiển thị HTML trong phần Tóm tắt (Summary)

Tài liệu này đặc tả cơ chế loại bỏ hoàn toàn các thẻ HTML khỏi phần Tóm tắt (Summary) của bài viết ở cả Backend (khi lưu mới/cập nhật) và Frontend (khi hiển thị dữ liệu lịch sử) để đảm bảo không hiển thị mã nguồn thẻ thô dưới tiêu đề bài viết.

---

## 1. Xử lý tại Backend (C#)

Khi quản trị viên không nhập phần Tóm tắt, hệ thống sẽ tự động trích xuất từ nội dung bài viết. Để tránh việc trích xuất cả thẻ HTML (như `<h2>`, `<img>`), chúng ta bổ sung phương thức làm sạch thẻ HTML trong `MappingExtensions.cs`:

#### `MappingExtensions.cs` (`CMS.Backend/Models/DTOs/MappingExtensions.cs`)
```csharp
        private static string StripHtml(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            // Loại bỏ các thẻ HTML dạng <...>
            var clean = System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
            // Giải mã các thực thể HTML cơ bản
            clean = clean.Replace("&nbsp;", " ")
                         .Replace("&amp;", "&")
                         .Replace("&lt;", "<")
                         .Replace("&gt;", ">");
            return clean.Trim();
        }

        private static string? TruncateSummary(string? summary, string? content)
        {
            const int maxLength = 500;
            var text = !string.IsNullOrWhiteSpace(summary)
                ? summary
                : StripHtml(content ?? "");
                
            return text?.Length > maxLength ? text.Substring(0, maxLength) : text;
        }
```

---

## 2. Xử lý tại Frontend (React SPA)

Để xử lý triệt để các bài viết cũ đã lưu sẵn chuỗi HTML trong trường `Summary`, Frontend sẽ thực hiện loại bỏ thẻ HTML bằng trình phân tích của trình duyệt trước khi hiển thị:

#### `BlogDetail` (`cms.frontend/src/pages/blog-detail/index.tsx`)
```typescript
    const stripHtml = (html: string): string => {
        if (!html) return '';
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = html;
        // Lấy text thuần sau khi loại bỏ mọi thẻ HTML
        const text = tempDiv.textContent || tempDiv.innerText || '';
        return text.trim();
    };
```

Và render:
```tsx
{post.summary && <p className="font-body-lg text-body-lg text-white/80 max-w-2xl mx-auto">{stripHtml(post.summary)}</p>}
```
