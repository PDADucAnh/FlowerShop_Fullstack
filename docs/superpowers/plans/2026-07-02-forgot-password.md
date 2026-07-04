# Forgot Password Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement a secure One-Time Reset Token forgot password workflow for Customers, using Gmail SMTP to send HTML recovery emails, and React SPA for the user interface.

**Architecture:** Extended database fields on Customer entity, backend API endpoints for request generation and token verification/resetting, and React views for entry and updates.

**Tech Stack:** ASP.NET Core 8, EF Core, React, Axios, Tailwind CSS, Gmail SMTP.

## Global Constraints
- **Timezone**: Asia/Ho_Chi_Minh
- **Token TTL**: 15 minutes
- **Hashing**: Use ASP.NET PasswordHasher<Customer>

---

### Task 1: Database Migration for Customer Reset Token

**Files:**
- Modify: `CMS.Data/Entities/Customer.cs`

**Interfaces:**
- Produces: `Customer.ResetToken` (string) and `Customer.ResetTokenExpiry` (DateTime?) properties in the model.

- [ ] **Step 1: Modify Customer Entity**

Update [Customer.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Data/Entities/Customer.cs) to declare new properties:
```csharp
        [MaxLength(100)]
        public string? ResetToken { get; set; }

        public DateTime? ResetTokenExpiry { get; set; }
```

- [ ] **Step 2: Add EF Core Migration**

Run: `dotnet ef migrations add AddCustomerResetToken --project CMS.Data --startup-project CMS.Backend`
Expected: Success. A migration file is created in `CMS.Data/Migrations`.

- [ ] **Step 3: Apply Database Migration**

Run: `dotnet ef database update --project CMS.Data --startup-project CMS.Backend`
Expected: Success. SQLite / SQL Server is updated with new columns `ResetToken` and `ResetTokenExpiry`.

- [ ] **Step 4: Commit**

```bash
git add CMS.Data/Entities/Customer.cs CMS.Data/Migrations/
git commit -m "feat: add ResetToken and ResetTokenExpiry to Customer entity and run migrations"
```

---

### Task 2: Implement Reset Password Email in EmailService

**Files:**
- Modify: `CMS.Backend/Services/Interfaces/IEmailService.cs`
- Modify: `CMS.Backend/Services/EmailService.cs`

**Interfaces:**
- Produces: `Task IEmailService.SendResetPasswordEmailAsync(string email, string name, string resetLink)`

- [ ] **Step 1: Update IEmailService interface**

Add the declaration to [IEmailService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/Interfaces/IEmailService.cs):
```csharp
        Task SendResetPasswordEmailAsync(string email, string name, string resetLink);
```

- [ ] **Step 2: Implement SendResetPasswordEmailAsync**

Add implementation to [EmailService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/EmailService.cs):
```csharp
        public async Task SendResetPasswordEmailAsync(string email, string name, string resetLink)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'><style>");
                sb.AppendLine("body { font-family: 'Georgia', serif; background: #f5f2ed; color: #1a1a1a; padding: 40px 20px; }");
                sb.AppendLine(".container { max-width: 500px; margin: 0 auto; background: #fff; border: 1px solid #d4cfc7; }");
                sb.AppendLine(".header { background: #ab2c5d; color: #fff; padding: 30px; text-align: center; }");
                sb.AppendLine(".header h1 { margin: 0; font-size: 20px; letter-spacing: 2px; text-transform: uppercase; }");
                sb.AppendLine(".content { padding: 30px; text-align: center; font-size: 15px; line-height: 1.6; }");
                sb.AppendLine(".btn { display: inline-block; background: #ab2c5d; color: #fff; padding: 12px 24px; text-decoration: none; font-weight: bold; margin: 20px 0; letter-spacing: 1px; }");
                sb.AppendLine(".footer { padding: 20px; background: #f5f2ed; text-align: center; font-size: 11px; color: #666; }");
                sb.AppendLine("</style></head><body>");
                sb.AppendLine("<div class='container'>");
                sb.AppendLine("<div class='header'><h1>AnhCMS Boutique</h1></div>");
                sb.AppendLine("<div class='content'>");
                sb.AppendLine($"<p>Xin chào <strong>{WebUtility.HtmlEncode(name)}</strong>,</p>");
                sb.AppendLine("<p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản của mình tại AnhCMS Boutique. Vui lòng bấm vào nút bên dưới để tiến hành thiết lập mật khẩu mới (liên kết có giá trị trong vòng 15 phút):</p>");
                sb.AppendLine($"<a class='btn' href='{resetLink}'>ĐẶT LẠI MẬT KHẨU</a>");
                sb.AppendLine("<p style='color: #888; font-size: 12px;'>Nếu bạn không yêu cầu hành động này, vui lòng bỏ qua email.</p>");
                sb.AppendLine("</div>");
                sb.AppendLine("<div class='footer'><p>© 2026 AnhCMS Boutique. All rights reserved.</p></div>");
                sb.AppendLine("</div></body></html>");

                var body = sb.ToString();
                var senderEmail = !string.IsNullOrEmpty(_settings.SenderEmail) && _settings.SenderEmail != "test@gmail.com"
                    ? _settings.SenderEmail
                    : _settings.Username;

                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, _settings.SenderName),
                    Subject = "Đặt lại mật khẩu của bạn - AnhCMS Boutique",
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(email, name));

                using var client = CreateSmtpClient();
                await client.SendMailAsync(message);
                _logger.LogInformation("Password reset email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                throw;
            }
        }
```

- [ ] **Step 3: Compile & Verify**

Run: `dotnet build CMS.Backend/CMS.Backend.csproj`
Expected: Build successfully without any errors.

- [ ] **Step 4: Commit**

```bash
git add CMS.Backend/Services/Interfaces/IEmailService.cs CMS.Backend/Services/EmailService.cs
git commit -m "feat: declare and implement SendResetPasswordEmailAsync in EmailService"
```

---

### Task 3: Implement Forgot/Reset Password Methods in AuthService

**Files:**
- Modify: `CMS.Backend/Services/Interfaces/IAuthService.cs`
- Modify: `CMS.Backend/Services/AuthService.cs`

**Interfaces:**
- Consumes: `IEmailService.SendResetPasswordEmailAsync`
- Produces:
  - `Task<(bool Success, string Message)> IAuthService.ForgotPassword(string email, string clientUrl)`
  - `Task<(bool Success, string Message)> IAuthService.ResetPassword(string token, string newPassword)`

- [ ] **Step 1: Declare methods in IAuthService.cs**

Update [IAuthService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/Interfaces/IAuthService.cs):
```csharp
        Task<(bool Success, string Message)> ForgotPassword(string email, string clientUrl);
        Task<(bool Success, string Message)> ResetPassword(string token, string newPassword);
```

- [ ] **Step 2: Inject IEmailService into AuthService**

Modify [AuthService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/AuthService.cs) to accept `IEmailService` in constructor:
```csharp
        private readonly IEmailService _emailService;

        public AuthService(IApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
            _userPasswordHasher = new PasswordHasher<User>();
            _customerPasswordHasher = new PasswordHasher<Customer>();
        }
```

- [ ] **Step 3: Implement ForgotPassword and ResetPassword**

Add implementations to [AuthService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/AuthService.cs):
```csharp
        public async Task<(bool Success, string Message)> ForgotPassword(string email, string clientUrl)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null)
            {
                // Anti email enumeration fallback response
                return (true, "Nếu email tồn tại trên hệ thống, một liên kết đặt lại mật khẩu đã được gửi đi. Vui lòng kiểm tra hộp thư.");
            }

            var token = Guid.NewGuid().ToString("N");
            customer.ResetToken = token;
            customer.ResetTokenExpiry = DateTime.Now.AddMinutes(15);

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            var resetLink = $"{clientUrl.TrimEnd('/')}/reset-password?token={token}";
            await _emailService.SendResetPasswordEmailAsync(customer.Email, customer.FullName, resetLink);

            return (true, "Yêu cầu đặt lại mật khẩu đã được gửi đi thành công.");
        }

        public async Task<(bool Success, string Message)> ResetPassword(string token, string newPassword)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ResetToken == token);
            if (customer == null)
            {
                return (false, "Mã xác thực đặt lại mật khẩu không hợp lệ.");
            }

            if (!customer.ResetTokenExpiry.HasValue || customer.ResetTokenExpiry.Value < DateTime.Now)
            {
                return (false, "Liên kết đặt lại mật khẩu đã hết hạn. Vui lòng yêu cầu lại.");
            }

            customer.PasswordHash = _customerPasswordHasher.HashPassword(customer, newPassword);
            customer.ResetToken = null;
            customer.ResetTokenExpiry = null;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return (true, "Đổi mật khẩu thành công!");
        }
```

- [ ] **Step 4: Compile & Verify**

Run: `dotnet build CMS.Backend/CMS.Backend.csproj`
Expected: Build successfully.

- [ ] **Step 5: Commit**

```bash
git add CMS.Backend/Services/Interfaces/IAuthService.cs CMS.Backend/Services/AuthService.cs
git commit -m "feat: implement ForgotPassword and ResetPassword logic in AuthService"
```

---

### Task 4: Expose Forgot/Reset Endpoints in AuthController

**Files:**
- Create: `CMS.Backend/Models/DTOs/ForgotPasswordRequests.cs`
- Modify: `CMS.Backend/Controllers/Api/AuthController.cs`

**Interfaces:**
- Produces:
  - `POST /api/Auth/forgot-password` (accepts `{ email }`)
  - `POST /api/Auth/reset-password` (accepts `{ token, newPassword }`)

- [ ] **Step 1: Create Request Models**

Create [ForgotPasswordRequests.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Models/DTOs/ForgotPasswordRequests.cs):
```csharp
using System.ComponentModel.DataAnnotations;

namespace CMS.Backend.Models.DTOs
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải dài tối thiểu 6 ký tự.")]
        public string NewPassword { get; set; }
    }
}
```

- [ ] **Step 2: Add Endpoints in AuthController**

Modify [AuthController.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Controllers/Api/AuthController.cs):
```csharp
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var clientUrl = Request.Headers["Origin"].ToString();
            if (string.IsNullOrEmpty(clientUrl))
            {
                clientUrl = "http://localhost:5173"; // Fallback development port
            }

            var (success, message) = await _authService.ForgotPassword(request.Email, clientUrl);
            return Ok(new { message });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var (success, message) = await _authService.ResetPassword(request.Token, request.NewPassword);
            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
```

- [ ] **Step 3: Compile & Verify**

Run: `dotnet build CMS.Backend/CMS.Backend.csproj`
Expected: Build successfully.

- [ ] **Step 4: Commit**

```bash
git add CMS.Backend/Models/DTOs/ForgotPasswordRequests.cs CMS.Backend/Controllers/Api/AuthController.cs
git commit -m "feat: expose forgot-password and reset-password endpoints in AuthController"
```

---

### Task 5: Add ForgotPassword & ResetPassword Views on Frontend

**Files:**
- Create: `cms.frontend/src/pages/forgot-password/index.tsx`
- Create: `cms.frontend/src/pages/reset-password/index.tsx`
- Modify: `cms.frontend/src/App.tsx`
- Modify: `cms.frontend/src/services/authService.ts`

**Interfaces:**
- Consumes: `/api/Auth/forgot-password` and `/api/Auth/reset-password` API endpoints.
- Produces: `/forgot-password` and `/reset-password` routes on the React UI app.

- [ ] **Step 1: Add API calls to Frontend authService**

Modify [authService.ts](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/services/authService.ts):
```typescript
    forgotPassword: async (email: string) => {
        try {
            const response = await axiosClient.post('/Auth/forgot-password', { email });
            return response.data || response;
        } catch (error) {
            console.error("Lỗi yêu cầu đặt lại mật khẩu:", error);
            throw error;
        }
    },
    resetPassword: async (data: { token: string; newPassword: newPassword }) => {
        try {
            const response = await axiosClient.post('/Auth/reset-password', data);
            return response.data || response;
        } catch (error) {
            console.error("Lỗi đặt lại mật khẩu:", error);
            throw error;
        }
    }
```
*(Wait, typescript parameter: change `newPassword: newPassword` to `newPassword: string`)*:
```typescript
    forgotPassword: async (email: string) => {
        try {
            const response = await axiosClient.post('/Auth/forgot-password', { email });
            return response.data || response;
        } catch (error) {
            console.error("Lỗi yêu cầu đặt lại mật khẩu:", error);
            throw error;
        }
    },
    resetPassword: async (data: { token: string; newPassword: string }) => {
        try {
            const response = await axiosClient.post('/Auth/reset-password', data);
            return response.data || response;
        } catch (error) {
            console.error("Lỗi đặt lại mật khẩu:", error);
            throw error;
        }
    }
```

- [ ] **Step 2: Create Forgot Password Page**

Create [cms.frontend/src/pages/forgot-password/index.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/forgot-password/index.tsx):
```tsx
import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import authService from '../../services/authService';

export default function ForgotPassword() {
    const [email, setEmail] = useState('');
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setMessage('');
        setError('');
        try {
            const res = await authService.forgotPassword(email);
            setMessage(res.message || 'Yêu cầu đặt lại mật khẩu đã được gửi, vui lòng kiểm tra email của bạn.');
        } catch (err: any) {
            setError(err.response?.data?.message || 'Có lỗi xảy ra, vui lòng thử lại.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-rose-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
            <div className="sm:mx-auto sm:w-full sm:max-w-md">
                <h2 className="mt-6 text-center text-3xl font-extrabold text-[#ab2c5d] font-playfair">
                    Quên Mật Khẩu
                </h2>
                <p className="mt-2 text-center text-sm text-gray-600">
                    Nhập email của bạn và chúng tôi sẽ gửi liên kết đặt lại mật khẩu.
                </p>
            </div>

            <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
                <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10 border border-rose-100">
                    <form className="space-y-6" onSubmit={handleSubmit}>
                        <div>
                            <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                                Địa chỉ Email
                            </label>
                            <div className="mt-1">
                                <input
                                    id="email"
                                    name="email"
                                    type="email"
                                    required
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                    className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-rose-500 focus:border-rose-500 sm:text-sm"
                                />
                            </div>
                        </div>

                        {message && (
                            <div className="rounded-md bg-green-50 p-4">
                                <div className="text-sm font-medium text-green-800">{message}</div>
                            </div>
                        )}

                        {error && (
                            <div className="rounded-md bg-red-50 p-4">
                                <div className="text-sm font-medium text-red-800">{error}</div>
                            </div>
                        )}

                        <div>
                            <button
                                type="submit"
                                disabled={loading}
                                className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-[#ab2c5d] hover:bg-[#8f244e] focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-rose-500 disabled:opacity-50"
                            >
                                {loading ? 'Đang gửi...' : 'Gửi yêu cầu'}
                            </button>
                        </div>
                    </form>

                    <div className="mt-6 flex items-center justify-between text-sm">
                        <Link to="/login" className="font-medium text-[#ab2c5d] hover:text-[#8f244e]">
                            Quay lại đăng nhập
                        </Link>
                    </div>
                </div>
            </div>
        </div>
    );
}
```

- [ ] **Step 3: Create Reset Password Page**

Create [cms.frontend/src/pages/reset-password/index.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/reset-password/index.tsx):
```tsx
import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import authService from '../../services/authService';

export default function ResetPassword() {
    const [searchParams] = useSearchParams();
    const token = searchParams.get('token') || '';
    const navigate = useNavigate();

    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    useEffect(() => {
        if (!token) {
            setError('Mã token đặt lại mật khẩu không tìm thấy hoặc không hợp lệ.');
        }
    }, [token]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (password !== confirmPassword) {
            setError('Mật khẩu nhập lại không trùng khớp.');
            return;
        }
        if (password.length < 6) {
            setError('Mật khẩu phải dài tối thiểu 6 ký tự.');
            return;
        }

        setLoading(true);
        setMessage('');
        setError('');
        try {
            const res = await authService.resetPassword({ token, newPassword: password });
            setMessage(res.message || 'Mật khẩu của bạn đã được đặt lại thành công. Đang chuyển hướng...');
            setTimeout(() => {
                navigate('/login');
            }, 3000);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Có lỗi xảy ra, vui lòng thử lại.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-rose-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
            <div className="sm:mx-auto sm:w-full sm:max-w-md">
                <h2 className="mt-6 text-center text-3xl font-extrabold text-[#ab2c5d] font-playfair">
                    Đặt Lại Mật Khẩu
                </h2>
                <p className="mt-2 text-center text-sm text-gray-600">
                    Vui lòng nhập mật khẩu mới của bạn bên dưới.
                </p>
            </div>

            <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
                <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10 border border-rose-100">
                    <form className="space-y-6" onSubmit={handleSubmit}>
                        <div>
                            <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                                Mật khẩu mới
                            </label>
                            <div className="mt-1">
                                <input
                                    id="password"
                                    name="password"
                                    type="password"
                                    required
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-rose-500 focus:border-rose-500 sm:text-sm"
                                />
                            </div>
                        </div>

                        <div>
                            <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700">
                                Xác nhận mật khẩu mới
                            </label>
                            <div className="mt-1">
                                <input
                                    id="confirmPassword"
                                    name="confirmPassword"
                                    type="password"
                                    required
                                    value={confirmPassword}
                                    onChange={(e) => setConfirmPassword(e.target.value)}
                                    className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-rose-500 focus:border-rose-500 sm:text-sm"
                                />
                            </div>
                        </div>

                        {message && (
                            <div className="rounded-md bg-green-50 p-4">
                                <div className="text-sm font-medium text-green-800">{message}</div>
                            </div>
                        )}

                        {error && (
                            <div className="rounded-md bg-red-50 p-4">
                                <div className="text-sm font-medium text-red-800">{error}</div>
                            </div>
                        )}

                        <div>
                            <button
                                type="submit"
                                disabled={loading || !token}
                                className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-[#ab2c5d] hover:bg-[#8f244e] focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-rose-500 disabled:opacity-50"
                            >
                                {loading ? 'Đang thực hiện...' : 'Đặt lại mật khẩu'}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}
```

- [ ] **Step 4: Register routes in App.tsx**

Modify [cms.frontend/src/App.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/App.tsx):
```tsx
import ForgotPassword from './pages/forgot-password';
import ResetPassword from './pages/reset-password';

// inside Routes element
<Route path="/forgot-password" element={<ForgotPassword />} />
<Route path="/reset-password" element={<ResetPassword />} />
```

- [ ] **Step 5: Add Forgot Password Link to Login Page**

Modify [cms.frontend/src/pages/login/index.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/login/index.tsx):
Add a "Quên mật khẩu?" link:
```tsx
                            <div className="text-sm">
                                <Link to="/forgot-password" className="font-medium text-[#ab2c5d] hover:text-[#8f244e]">
                                    Quên mật khẩu?
                                </Link>
                            </div>
```

- [ ] **Step 6: Verify TypeScript types**

Run: `npx tsc --noEmit` inside `cms.frontend/` directory.
Expected: Compilation completes without any type errors.

- [ ] **Step 7: Commit**

```bash
git add cms.frontend/src/pages/forgot-password/ cms.frontend/src/pages/reset-password/ cms.frontend/src/App.tsx cms.frontend/src/services/authService.ts cms.frontend/src/pages/login/index.tsx
git commit -m "feat: implement frontend forgot password and reset password flows and views"
```
