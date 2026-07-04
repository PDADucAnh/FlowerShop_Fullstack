```markdown
# Workflow: Hiển thị Nội dung HTML từ Rich Text Editor (CKEditor)

Tài liệu này đặc tả luồng xử lý dữ liệu từ khâu quản trị viên soạn thảo bài viết bằng CKEditor, lưu trữ chuỗi mã HTML an toàn vào SQL Server, và hiển thị đầy đủ định dạng (chữ đậm, nghiêng, hình ảnh căn giữa) trên giao diện Frontend Next.js mà không bị lộ thẻ code thô.

---

## I. Các website lớn họ xử lý như thế nào?

**Họ xử lý giống bạn, nhưng có thêm 2 chốt chặn: Làm sạch mã độc (XSS Sanitization) và Định dạng CSS toàn cục (Scoping CSS).**

Các trang web lớn (như VnExpress, Medium, hay trang Blog của các hãng lớn) đều lưu nội dung bài viết dưới dạng một chuỗi HTML thuần (`string`) trong Database. Khi hiển thị ở Frontend, họ bắt buộc phải dùng cơ chế đổ dữ liệu HTML trực tiếp (trong React là `dangerouslySetInnerHTML`).

Tuy nhiên, để đạt độ thẩm mỹ và an toàn cao, họ xử lý thêm 2 vấn đề:
1. **Chống tấn công XSS (Cross-Site Scripting):** Nếu hacker cố tình chèn một đoạn code độc vào bài viết (Ví dụ: `<script>steal_cookie()</script>`), khi React render thẳng ra màn hình, đoạn code đó sẽ chạy và hack tài khoản người dùng. Các trang lớn luôn lọc bỏ các thẻ nguy hiểm này trước khi hiển thị.
2. **CSS Typography (Định dạng Layout bài viết):** CKEditor sinh ra các thẻ HTML thô như `<figure class="image"><img></figure>` hay `<p style="text-align:center">`. Nếu không viết CSS riêng cho các thẻ này, hình ảnh có thể bị tràn màn hình, vỡ bố cục trên giao diện Mobile.

---

## II. Sơ đồ Luồng Hoạt động (Workflow Diagram)


```

[Admin soạn bài trên CKEditor] ──► [CKEditor tự dịch thành chuỗi HTML String]
│
▼ (Gửi API lưu bài viết)
[Backend ASP.NET Core tiếp nhận]
│
▼ (Lưu kiểu NVARCHAR(MAX))
[SQL Server lưu trữ chuỗi HTML gốc]
│
▼ (Next.js gọi API chi tiết bài viết)
[Backend trả về dữ liệu JSON thuần]
│
▼
[Lớp Chốt Chặn 1: Làm sạch HTML bằng DOMPurify (Chống XSS)]
│
▼
[Lớp Chốt Chặn 2: Đổ HTML vào dangerouslySetInnerHTML]
│
▼
[Lớp Chốt Chặn 3: Áp CSS Scoping để bo góc, căn giữa hình ảnh]
│
▼
[Người dùng xem bài viết chuẩn định dạng đẹp mắt]

```

---

## III. Chi tiết Triển khai Kỹ thuật trên Stack Công nghệ

### 1. Lưu trữ dữ liệu tại Backend (ASP.NET Core & SQL Server)

* **SQL Server:** Cột nội dung bài viết (`Content`) trong bảng `Articles` hoặc `Blogs` bắt buộc phải để kiểu dữ liệu **`NVARCHAR(MAX)`** để có thể lưu trữ chuỗi văn bản dài kèm theo các thẻ HTML, class, style và mã hóa ký tự tiếng Việt.
* **ASP.NET Core API:** Backend chỉ đóng vai trò trung chuyển, nhận chuỗi String HTML từ trang Admin và lưu thẳng vào DB. Khi API chi tiết bài viết được gọi (`GET /api/v1/articles/{id}`), Backend trả về JSON nguyên bản không chỉnh sửa.

### 2. Xử lý hiển thị tại Frontend (Next.js / ReactJS)

Tại trang chi tiết bài viết (Ví dụ: `pages/blog/[id].jsx`), chúng ta thực hiện 3 bước xử lý để đảm bảo an toàn và thẩm mỹ.

* **Bước A: Cài đặt thư viện làm sạch mã độc (Sanitize HTML)**
  Để chặn đứng nguy cơ XSS, bạn cài đặt thư viện `dompurify` (Thư viện tiêu chuẩn ngành được các ông lớn tin dùng):
  ```bash
  npm install dompurify

```

* **Bước B: Cấu trúc Code hiển thị nội dung tại `PostDetail.jsx**`
```javascript
import { useEffect, useState } from 'react';
import DOMPurify from 'dompurify';

export default function PostDetail({ article }) {
  const [cleanHtml, setCleanHtml] = useState('');

  useEffect(() => {
    if (article && article.content) {
      // Chốt chặn 1: Loại bỏ toàn bộ thẻ nguy hiểm <script>, <iframe> độc hại
      const sanitized = DOMPurify.sanitize(article.content);
      setCleanHtml(sanitized);
    }
  }, [article]);

  return (
    <div className="blog-detail-container">
      <h1 className="blog-title">{article.title}</h1>
      <div className="blog-meta">Ngày đăng: {article.createdAt}</div>

      {/* Chốt chặn 2: Sử dụng dangerouslySetInnerHTML và bọc trong một class chung để viết CSS */}
      <div 
        className="ck-content blog-post-body"
        dangerouslySetInnerHTML={{ __html: cleanHtml }} 
      />
    </div>
  );
}

```



### 3. Thiết kế CSS Scoping (Định dạng chữ, căn lề và hình ảnh nằm giữa)

Khi sử dụng `dangerouslySetInnerHTML`, bạn không thể viết class trực tiếp cho các thẻ `<p>`, `<img>` bên trong bài viết từ code React. Bạn phải viết CSS kế thừa thông qua lớp bọc ngoài là `.blog-post-body`.

Hãy thêm đoạn CSS sau vào file style chung (Ví dụ: `BlogDetail.css` hoặc `globals.css`):

```css
/* Khung bọc toàn bộ bài viết */
.blog-post-body {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  font-size: 18px;
  line-height: 1.8;
  color: #333;
}

/* Định dạng chữ đậm, chữ nghiêng mặc định */
.blog-post-body strong {
  font-weight: 700;
  color: #000;
}

.blog-post-body em {
  font-style: italic;
}

/* Định dạng căn lề văn bản */
.blog-post-body p {
  margin-bottom: 1.5rem;
  text-align: justify; /* Căn lề đều 2 bên cho giống trang báo lớn */
}

/* CHỐT CHẶN 3: Xử lý hình ảnh từ CKEditor (Căn giữa và responsive) */
.blog-post-body img {
  max-width: 100%;    /* Chống tràn màn hình điện thoại */
  height: auto;       /* Giữ nguyên tỷ lệ ảnh không bị móp */
  display: block;     /* Biến ảnh thành block để căn giữa */
  margin: 20px auto;  /* Tự động căn giữa hình ảnh theo chiều ngang */
  border-radius: 8px; /* Bo góc nhẹ cho hình ảnh thêm hiện đại */
  box-shadow: 0 4px 12px rgba(0,0,0,0.08); /* Đổ bóng nhẹ cho ảnh nổi bật */
}

/* Xử lý thẻ figure bọc ngoài ảnh của CKEditor5 */
.blog-post-body figure.image {
  margin: 30px auto;
  text-align: center;
}

/* Định dạng phần chú thích dưới ảnh (Caption) */
.blog-post-body figure.image figcaption {
  font-size: 14px;
  font-style: italic;
  color: #666;
  margin-top: 8px;
}

```

---

## IV. Tóm tắt Tiêu chuẩn An toàn & Hiệu năng (Best Practices)

| Thành phần | Công nghệ áp dụng | Mục đích |
| --- | --- | --- |
| **Database** | `NVARCHAR(MAX)` | Lưu trữ trọn vẹn mã HTML gốc, giữ nguyên dấu tiếng Việt và các thuộc tính inline style. |
| **Frontend** | Thư viện `DOMPurify` | Chốt chặn an ninh tối cao, lọc sạch mã độc tấn công XSS trước khi đưa vào hàm của React. |
| **CSS** | CSS Descendant Selector | Viết CSS kế thừa (Ví dụ: `.blog-post-body img`) giúp tự động căn giữa và co giãn toàn bộ hình ảnh trong bài viết một cách đồng bộ. |

```

```