# Implementation Plan: Fix and Enable Profile Edit Functionality

This plan defines the steps to add profile edit capabilities to both the ASP.NET backend and React frontend.

## Goal
Enable the "Chỉnh sửa" (Edit) button on the client Profile page. Allow users to update their FullName, Phone, and Address, save changes to the SQL Server database, update the React state/JWT token, and return feedback to the user.

---

### Task 1: Backend API Implementation

- [x] **Step 1: Add UpdateProfileRequest DTO**
  Add `UpdateProfileRequest` class inside [AuthDTOs.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Models/DTOs/AuthDTOs.cs):
  ```csharp
  public class UpdateProfileRequest
  {
      [Required(ErrorMessage = "Họ tên không được để trống")]
      public string FullName { get; set; } = string.Empty;

      public string? Phone { get; set; }

      public string? Address { get; set; }
  }
  ```

- [x] **Step 2: Update IAuthService Interface**
  Add `UpdateProfile` method to [IAuthService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/Interfaces/IAuthService.cs):
  ```csharp
  Task<(bool Success, string Message, LoginResult? Result)> UpdateProfile(string identifier, string authType, string fullName, string? phone, string? address);
  ```

- [x] **Step 3: Implement UpdateProfile in AuthService**
  Implement the method in [AuthService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/AuthService.cs):
  ```csharp
  public async Task<(bool Success, string Message, LoginResult? Result)> UpdateProfile(string identifier, string authType, string fullName, string? phone, string? address)
  {
      if (string.IsNullOrWhiteSpace(fullName))
          return (false, "Họ tên không được để trống", null);

      if (authType == "User")
      {
          var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == identifier);
          if (user == null) return (false, "Không tìm thấy người dùng", null);

          user.FullName = fullName;
          user.Phone = phone;
          user.Address = address;

          await _context.SaveChangesAsync();
          return (true, "Cập nhật thông tin thành công", MapUserToResult(user));
      }

      if (authType == "Customer")
      {
          var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == identifier);
          if (customer == null) return (false, "Không tìm thấy khách hàng", null);

          customer.FullName = fullName;
          customer.Phone = phone;
          customer.Address = address;

          await _context.SaveChangesAsync();
          return (true, "Cập nhật thông tin thành công", MapCustomerToResult(customer));
      }

      return (false, "Vai trò không hợp lệ", null);
  }
  ```

- [x] **Step 4: Create PUT endpoint in AuthController**
  Add `UpdateProfile` action to [AuthController.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Controllers/Api/AuthController.cs):
  ```csharp
  [Authorize]
  [HttpPut("profile")]
  public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
  {
      var username = User.Identity?.Name;
      if (string.IsNullOrEmpty(username))
          return Unauthorized(new { message = "Invalid token" });

      var authType = User.FindFirst("AuthType")?.Value ?? "User";

      var (success, message, result) = await _authService.UpdateProfile(username, authType, request.FullName, request.Phone, request.Address);
      if (!success || result == null)
          return BadRequest(new { message });

      // Generate a new JWT token to update claims
      var jwtKey = _configuration["Jwt:SecretKey"]
          ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
      var issuer = _configuration["Jwt:Issuer"]
          ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
      var audience = _configuration["Jwt:Audience"]
          ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
      if (!int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var expiryMinutes))
          expiryMinutes = 60;

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.UTF8.GetBytes(jwtKey);
      var expiration = DateTime.UtcNow.AddMinutes(expiryMinutes);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
          Subject = new ClaimsIdentity(BuildUserClaims(result)),
          Expires = expiration,
          Issuer = issuer,
          Audience = audience,
          SigningCredentials = new SigningCredentials(
              new SymmetricSecurityKey(key),
              SecurityAlgorithms.HmacSha256)
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      var tokenString = tokenHandler.WriteToken(token);

      return Ok(new
      {
          token = tokenString,
          user = new
          {
              id = result.Id,
              username = result.Username,
              fullName = result.FullName,
              email = result.Email,
              phone = result.Phone,
              address = result.Address,
              role = result.Role
          },
          message
      });
  }
  ```

- [x] **Step 5: Verify Backend Build**
  Run `dotnet build` inside the root or `CMS.Backend/` directory.

---

### Task 2: Frontend Client Implementation

- [x] **Step 6: Update authService.ts API Call**
  Add `updateProfile` method to [authService.ts](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/services/authService.ts):
  ```typescript
  updateProfile: async (profileData: { fullName: string; phone: string; address: string }) => {
      try {
          const response = await axiosClient.put('/Auth/profile', profileData);
          return response.data || response;
      } catch (error) {
          console.error("Lỗi cập nhật hồ sơ:", error);
          throw error;
      }
  }
  ```

- [x] **Step 7: Expose updateProfile in AuthContext**
  Modify [AuthContext.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/context/AuthContext.tsx) to support updating profile state:
  ```typescript
  // 1. In types definition (or local interfaces), verify updateProfile is declared
  // 2. Add updateProfile callback inside AuthProvider:
  const updateProfile = useCallback(async (fullName: string, phone: string, address: string) => {
      const response = await authService.updateProfile({ fullName, phone, address });
      if (response.token) {
          tokenService.setToken(response.token);
      }
      if (response.user) {
          setUser(response.user);
      }
      return response;
  }, []);
  
  // 3. Update returned value:
  const value: AuthContextType = { user, login, logout, refreshProfile, updateProfile, isAuthenticated, loading, token: tokenService.getToken() };
  ```
  *Note: Need to check [context.d.ts](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/types/context.ts) or equivalent to ensure the type matches.*

- [x] **Step 8: Update Profile Page UI and Logic**
  Modify [Profile.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/auth/Profile.tsx):
  - Add state `isEditing`, `fullName`, `phone`, `address`.
  - Sync state fields on `user` change.
  - Switch display fields to editable inputs when `isEditing === true`.
  - Handle form submission with loading state, toaster messages, and validation.

- [x] **Step 9: Compile and Verify**
  Run `npx tsc --noEmit` under `cms.frontend/` and make sure it builds cleanly.
  Run `npm run build` as confirmation.
