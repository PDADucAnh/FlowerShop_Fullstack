Bạn là Tech Lead và Principal Software Architect chịu trách nhiệm đánh giá chất lượng toàn bộ dự án.

Hãy thực hiện một cuộc kiểm tra (full project audit) trên toàn bộ source code trước khi đưa ra bất kỳ kết luận nào.

Yêu cầu:

1. Đọc và hiểu toàn bộ cấu trúc dự án bằng cách sử dụng các công cụ phân tích mã nguồn hiện có.
2. Xây dựng sơ đồ kiến trúc và luồng dữ liệu của dự án trước khi đánh giá.
3. Không chỉ tìm lỗi compile, mà phải đánh giá chất lượng tổng thể của hệ thống.

Kiểm tra các nội dung sau:

## 1. Kiến trúc
- Kiến trúc tổng thể
- Dependency giữa các module
- Circular dependency
- Module coupling
- Khả năng mở rộng
- Khả năng bảo trì
- Thiết kế chưa hợp lý

## 2. Backend (ASP.NET Core)
- Controller
- Service
- Repository
- Entity Framework Core
- Middleware
- Authentication
- Authorization
- Validation
- Exception Handling
- Logging
- Async/Await
- Transaction
- Dependency Injection
- Business Logic
- API Design

Phát hiện:
- Bug
- Logic sai
- Null reference
- Race condition
- Deadlock
- Memory leak
- Blocking thread
- N+1 Query
- Query không tối ưu
- Thiếu validation
- Code smell
- Duplicate code
- Dead code

## 3. Frontend (Next.js)

Kiểm tra:

- App Router
- Server Component
- Client Component
- React Hooks
- State Management
- Rendering
- Routing
- Fetching
- Caching
- SEO
- Accessibility

Phát hiện:

- Re-render không cần thiết
- useEffect sai dependency
- Infinite render
- Memory leak
- Bundle quá lớn
- Chưa lazy loading
- Hydration issue
- Duplicate request
- Chưa tối ưu Image
- Chưa tối ưu Font

## 4. Database

Kiểm tra:

- Schema
- Relationship
- Constraint
- Foreign Key
- Index
- Entity Mapping

Phát hiện:

- Missing Index
- Full Table Scan
- SELECT *
- Duplicate Query
- Data redundancy
- Query chậm
- Thiết kế chưa tối ưu

## 5. API

Kiểm tra:

- RESTful
- Status Code
- Response Format
- Pagination
- Filtering
- Sorting
- Versioning
- Authentication
- Authorization

## 6. Security

Đánh giá theo OWASP Top 10.

Kiểm tra:

- SQL Injection
- XSS
- CSRF
- CORS
- JWT
- Secret
- File Upload
- Input Validation
- Output Encoding

## 7. Performance

Phân tích toàn bộ nguyên nhân có thể làm hệ thống chạy chậm.

Bao gồm:

Frontend

Backend

Database

API

Network

Rendering

Bundle Size

Caching

Entity Framework

Database Query

## 8. Maintainability

Đánh giá:

- Clean Architecture
- SOLID
- Code Smell
- Readability
- Scalability
- Testability
- Reusability

## 9. Documentation

Kiểm tra:

- Thiếu tài liệu
- Thiếu comment cần thiết
- Thiếu README
- Thiếu hướng dẫn deploy
- Thiếu cấu hình

## 10. Những phần còn thiếu

Hãy phát hiện:

- Feature còn dang dở
- TODO/FIXME
- API chưa hoàn chỉnh
- Validation còn thiếu
- Error Handling còn thiếu
- Logging còn thiếu
- Test còn thiếu
- Security còn thiếu
- Monitoring còn thiếu

## 11. Báo cáo

Đối với mỗi vấn đề, trình bày theo định dạng:

Severity:
(Critical / High / Medium / Low)

Category:

File:

Vị trí:

Nguyên nhân:

Ảnh hưởng:

Cách khắc phục:

Độ ưu tiên sửa:

## 12. Tổng kết

Đưa ra:

- Top 20 vấn đề nghiêm trọng nhất.
- Top 20 vấn đề ảnh hưởng hiệu năng.
- Top 20 vấn đề bảo mật.
- Top 20 vấn đề về kiến trúc.
- Top 20 cơ hội tối ưu.

## 13. Chấm điểm

Đánh giá theo thang điểm 10:

Architecture

Backend

Frontend

Database

Performance

Security

Maintainability

Scalability

Testing

Documentation

Overall

## 14. Roadmap

Lập kế hoạch cải thiện theo thứ tự ưu tiên:

Phase 1 (Critical)

Phase 2 (High)

Phase 3 (Medium)

Phase 4 (Low)

Không được đưa ra kết luận khi chưa phân tích toàn bộ dự án.

Nếu phát hiện vấn đề nhưng chưa đủ bằng chứng, hãy ghi rõ rằng đó là "nghi ngờ cần xác minh" thay vì khẳng định.

Ưu tiên sử dụng các công cụ phân tích mã nguồn, tài liệu chính thức và khả năng suy luận để đưa ra đánh giá chính xác.

# Sau khi hoàn thành Audit

Không chỉ hiển thị kết quả trong cuộc trò chuyện.

Hãy cập nhật các tài liệu sau nếu chúng đã tồn tại, hoặc tạo mới nếu chưa có:

- docs/audits/reports/latest.md
- docs/audits/FIX_PLAN.md
- docs/audits/TECH_DEBT.md
- docs/audits/CHANGELOG_AUDIT.md

Yêu cầu:

1. latest.md

- Ghi kết quả đầy đủ của lần audit hiện tại.
- Thay thế báo cáo cũ bằng báo cáo mới.

2. FIX_PLAN.md

- Giữ lại các lỗi chưa được sửa.
- Loại bỏ các lỗi đã được khắc phục.
- Thêm các lỗi mới phát hiện.
- Nhóm theo Critical, High, Medium và Low.
- Sử dụng checklist để dễ theo dõi.

3. TECH_DEBT.md

- Chỉ ghi các khoản Technical Debt còn tồn tại.
- Không liệt kê các lỗi đã được sửa.
- Nhóm theo Frontend, Backend, Database, Infrastructure.

4. CHANGELOG_AUDIT.md

Thêm một mục mới bao gồm:

- Ngày audit
- Số lượng Critical
- Số lượng High
- Số lượng Medium
- Số lượng Low
- Các thay đổi quan trọng
- Các lỗi đã được khắc phục kể từ lần audit trước

Nếu không có thay đổi thì ghi rõ "Không có thay đổi".

Không được làm mất lịch sử của CHANGELOG_AUDIT.md.

Sau khi cập nhật xong các tài liệu, hãy hiển thị tóm tắt kết quả audit trong cuộc trò chuyện.