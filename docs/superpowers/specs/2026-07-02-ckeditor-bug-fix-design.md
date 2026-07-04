# Tài liệu Đặc tả Thiết kế: Sửa lỗi hiển thị HTML và hình ảnh CKEditor

Tài liệu này đặc tả cơ chế khắc phục lỗi hiển thị thẻ HTML thô và hình ảnh bị lỗi trên trang chi tiết bài viết của khách hàng.

---

## 1. Nguyên nhân lỗi

1. **Hiển thị thẻ HTML thô**: Nội dung HTML lưu trữ trong database hoặc trả về từ API có thể đã bị mã hóa thực thể (HTML Entities) thành dạng `&lt;p&gt;hoa xinh đep&lt;/p&gt;`. Khi render qua `dangerouslySetInnerHTML`, trình duyệt giải mã các thực thể này và hiển thị chúng dưới dạng văn bản thô thay vì biên dịch thành thẻ HTML.
2. **Hình ảnh bị lỗi**: Đường dẫn hình ảnh chèn từ CKEditor có dạng tương đối `/uploads/ckeditor/...`. Khi tải ở trang khách hàng (React SPA chạy ở port 3000), trình duyệt cố tải ảnh từ host của Frontend (`http://localhost:3000/uploads/...`) thay vì Backend API (`https://localhost:7224/uploads/...`), dẫn đến lỗi 404.

---

## 2. Giải pháp khắc phục (Frontend)

Chúng ta xử lý an toàn tại trang chi tiết bài viết [BlogDetail.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/blog-detail/index.tsx) theo thứ tự bảo mật nghiêm ngặt:

1. **Giải mã thực thể HTML (HTML Entity Decoding)**: 
   Chuyển đổi các chuỗi đã bị mã hóa thực thể (nếu có) trở lại dạng mã HTML gốc.
   ```typescript
   const decodeHtmlEntities = (str: string): string => {
       const txt = document.createElement('textarea');
       txt.innerHTML = str;
       return txt.value;
   };
   ```

2. **Chuyển đổi URL tương đối sang tuyệt đối (Absolute URL Replacement)**:
   Thay thế đường dẫn tương đối `/uploads/` thành URL tuyệt đối trỏ tới Backend API `API_BASE_URL/uploads/`.
   ```typescript
   const contentWithAbsoluteUrls = decodedContent.replace(
       /src=["']\/uploads\/(.*?)["']/g,
       `src="${API_BASE_URL}/uploads/$1"`
   );
   ```

3. **Làm sạch mã độc (Sanitization)**:
   Thực hiện `DOMPurify.sanitize()` **SAU KHI** đã giải mã thực thể HTML để đảm bảo loại bỏ hoàn toàn các thẻ độc hại (ví dụ: `<script>`) trước khi render vào DOM.

---

## 3. Mã nguồn chi tiết tại Frontend

Cập nhật hàm render bài viết trong [BlogDetail.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/blog-detail/index.tsx):

```typescript
    const decodeHtmlEntities = (str: string): string => {
        const txt = document.createElement('textarea');
        txt.innerHTML = str;
        return txt.value;
    };

    const getProcessedContent = (content: string) => {
        if (!content) return '';
        const decoded = decodeHtmlEntities(content);
        const withAbsoluteUrls = decoded.replace(
            /src=["']\/uploads\/(.*?)["']/g,
            `src="${API_BASE_URL}/uploads/$1"`
        );
        return DOMPurify.sanitize(withAbsoluteUrls);
    };
```
Và render:
```tsx
    dangerouslySetInnerHTML={{ __html: getProcessedContent(post.content) }}
```
