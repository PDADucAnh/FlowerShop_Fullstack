# Tài liệu Đặc tả Thiết kế: Hiển thị Nội dung HTML từ CKEditor

Tài liệu này đặc tả cơ chế hiển thị an toàn và chuẩn hóa giao diện các nội dung HTML (được soạn thảo bằng CKEditor) tại trang chi tiết bài viết, đảm bảo tính responsive trên mobile và phòng chống XSS.

---

## 1. Cơ chế An toàn Bảo mật (XSS Sanitization)

Trang chi tiết bài viết [BlogDetail.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/blog-detail/index.tsx) sử dụng thư viện `DOMPurify` để khử trùng nội dung HTML trước khi render thông qua thuộc tính `dangerouslySetInnerHTML`:

```tsx
dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(post.content) }}
```

---

## 2. Thiết kế CSS Scoping cho Nội dung Bài viết

Do mã HTML được kết xuất động, chúng ta sử dụng CSS Descendant Selector dưới lớp bọc ngoài `.blog-detail-content` để định dạng các thẻ HTML thô. 

Các định dạng này được lưu trữ trong khối `<style>` chung tại [public/index.html](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/public/index.html):

```css
        /* CKEditor HTML Rendering Styles */
        .blog-detail-content {
            font-family: 'Plus Jakarta Sans', sans-serif;
            font-size: 16px;
            line-height: 1.8;
            color: #3f001b; /* matching secondary color */
        }
        
        .blog-detail-content strong {
            font-weight: 700;
            color: #1b1c1c;
        }
        
        .blog-detail-content em {
            font-style: italic;
        }

        .blog-detail-content p {
            margin-bottom: 1.5rem;
            text-align: justify;
        }
        
        .blog-detail-content h2 {
            font-family: 'Playfair Display', serif;
            font-size: 24px;
            font-weight: 600;
            margin-top: 2rem;
            margin-bottom: 1rem;
            color: #ab2c5d; /* primary color */
        }

        .blog-detail-content h3 {
            font-family: 'Playfair Display', serif;
            font-size: 20px;
            font-weight: 600;
            margin-top: 1.75rem;
            margin-bottom: 0.75rem;
            color: #ab2c5d;
        }

        .blog-detail-content ul {
            list-style-type: disc;
            margin-left: 1.5rem;
            margin-bottom: 1.5rem;
        }

        .blog-detail-content ol {
            list-style-type: decimal;
            margin-left: 1.5rem;
            margin-bottom: 1.5rem;
        }

        .blog-detail-content li {
            margin-bottom: 0.5rem;
        }

        .blog-detail-content blockquote {
            border-left: 4px solid #ab2c5d;
            padding-left: 1rem;
            margin-left: 0;
            margin-right: 0;
            margin-bottom: 1.5rem;
            font-style: italic;
            color: #6b5a60;
        }

        /* Responsive image rendering and alignment */
        .blog-detail-content img {
            max-width: 100%;
            height: auto;
            display: block;
            margin: 20px auto;
            border-radius: 8px;
            box-shadow: 0 4px 20px rgba(171, 44, 93, 0.08);
        }

        .blog-detail-content figure.image {
            margin: 30px auto;
            text-align: center;
        }

        .blog-detail-content figure.image figcaption {
            font-size: 14px;
            font-style: italic;
            color: #8a7176;
            margin-top: 8px;
        }
```
