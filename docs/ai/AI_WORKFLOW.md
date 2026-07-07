# AI Working Policy

Khi thực hiện bất kỳ nhiệm vụ nào, hãy ưu tiên sử dụng các công cụ đã được cấu hình nếu chúng giúp đưa ra kết quả chính xác hơn.

## Nguyên tắc

Không gọi công cụ nếu chúng không mang lại giá trị cho nhiệm vụ hiện tại.

Ưu tiên sử dụng công cụ phù hợp thay vì suy đoán.

---

## CodeGraph

Sử dụng khi cần:

- Hiểu kiến trúc dự án
- Phân tích dependency
- Tìm nơi một hàm hoặc class được sử dụng
- Phân tích ảnh hưởng trước khi sửa
- Call Graph
- Reference Graph

Không cần dùng nếu chỉ sửa lỗi nhỏ trong một file độc lập.

---

## Context7

Ưu tiên sử dụng khi:

- Framework
- Library
- API
- Cấu hình
- Best Practice

Luôn ưu tiên tài liệu chính thức nếu có.

---

## Sequential Thinking

Sử dụng khi:

- Bài toán lớn
- Refactor
- Thiết kế
- Audit
- Debug phức tạp

Lập kế hoạch trước khi sửa.

---

## Playwright

Sử dụng khi:

- Thay đổi giao diện
- Luồng người dùng
- Form
- Authentication
- Navigation
- Checkout
- Dashboard

Sau khi sửa, nếu có thể hãy xác minh bằng Playwright.

---

## GitHub MCP

Chỉ sử dụng khi nhiệm vụ liên quan đến:

- Issue
- Pull Request
- Review
- GitHub Actions
- Release

Không sử dụng GitHub MCP chỉ để commit hoặc push.

---

## Superpowers

Áp dụng các workflow phù hợp khi:

- Debug
- Refactor
- Performance
- Documentation
- Testing

---

## Andrej Karpathy Skills

Áp dụng cho mọi nhiệm vụ.

Luôn:

- Phân tích trước
- Chia nhỏ vấn đề
- Kiểm tra lại
- Tự review kết quả
- Đánh giá rủi ro

---

## Quy trình làm việc

1. Hiểu yêu cầu.

2. Xác định công cụ nào giúp ích.

3. Chỉ gọi những công cụ cần thiết.

4. Không gọi công cụ dư thừa.

5. Kiểm tra lại kết quả.

6. Nếu sửa code:

- Build
- Test
- Xác minh
- Sau đó mới kết luận.
