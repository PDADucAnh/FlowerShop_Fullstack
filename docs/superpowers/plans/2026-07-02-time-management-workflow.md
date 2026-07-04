# Time Management Workflow Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement a complete Time Management Workflow for ordering, ensuring correct Vietnam timezone usage, lead-time rules (2-hour buffer), slot capacity controls, and production completion targets.

**Architecture:**
- Extend the `Order` database model to store `TargetFinishedTime`.
- Create a configuration object `TimeSettings` loaded from `appsettings.json`.
- Create a cross-platform helper `DateTimeUtils` to retrieve the current date and time in the `"Asia/Ho_Chi_Minh"` timezone.
- Update `OrderService` creation logic to enforce past-date checks and lead-time validations on today's orders, and compute production completion targets.
- Update the Frontend DatePicker to use Vietnam-synchronized minDate, and filter out unavailable slots for today's orders using client-side timezone calculation.

**Tech Stack:** ASP.NET Core 8, EF Core, C#, React, TypeScript

## Global Constraints
- Do not break existing order verification or payment webhook logic.
- Ensure cross-platform compatibility of timezone lookup between Linux/Unix (`Asia/Ho_Chi_Minh`) and Windows (`SE Asia Standard Time`).
- Save changes incrementally and commit after each task.

---

### Task 1: Extend Database Schema and Settings

**Files:**
- Modify: `CMS.Data/Entities/Order.cs`
- Create: `CMS.Backend/Models/TimeSettings.cs`
- Modify: `CMS.Backend/Program.cs`
- Modify: `CMS.Backend/appsettings.json`
- Create: `CMS.Data/Migrations/[Timestamp]_AddOrderTargetFinishedTime.cs` (via CLI)

- [ ] **Step 1: Add TargetFinishedTime to Order entity**
  Open `CMS.Data/Entities/Order.cs` and add the `TargetFinishedTime` property:
  ```csharp
  public DateTime? TargetFinishedTime { get; set; }
  ```

- [ ] **Step 2: Create TimeSettings class**
  Create the file `CMS.Backend/Models/TimeSettings.cs`:
  ```csharp
  namespace CMS.Backend.Models
  {
      public class TimeSettings
      {
          public string TimeZone { get; set; } = "Asia/Ho_Chi_Minh";
          public int LeadTimeHours { get; set; } = 2;
          public int MaxOrdersPerSlot { get; set; } = 10;
          public int PreShippingMinutes { get; set; } = 30;
      }
  }
  ```

- [ ] **Step 3: Register settings in Program.cs**
  Open `CMS.Backend/Program.cs` and register the `TimeSettings` configuration singleton (around line 120):
  ```csharp
  var timeSettings = builder.Configuration.GetSection("TimeSettings").Get<TimeSettings>() ?? new TimeSettings();
  builder.Services.AddSingleton(timeSettings);
  ```

- [ ] **Step 4: Add configuration to appsettings.json**
  Open `CMS.Backend/appsettings.json` and add the `TimeSettings` section:
  ```json
    "TimeSettings": {
      "TimeZone": "Asia/Ho_Chi_Minh",
      "LeadTimeHours": 2,
      "MaxOrdersPerSlot": 10,
      "PreShippingMinutes": 30
    },
  ```

- [ ] **Step 5: Generate and apply EF migration**
  Run:
  ```powershell
  dotnet build CMS.Data/CMS.Data.csproj
  dotnet build CMS.Backend/CMS.Backend.csproj
  & "$env:USERPROFILE\.dotnet\tools\dotnet-ef.exe" migrations add AddOrderTargetFinishedTime --project CMS.Data/CMS.Data.csproj --startup-project CMS.Backend/CMS.Backend.csproj --no-build
  dotnet build CMS.Data/CMS.Data.csproj; dotnet build CMS.Backend/CMS.Backend.csproj
  & "$env:USERPROFILE\.dotnet\tools\dotnet-ef.exe" database update --project CMS.Data/CMS.Data.csproj --startup-project CMS.Backend/CMS.Backend.csproj --no-build
  ```
  Expected: Database schema successfully updated.

- [ ] **Step 6: Commit changes**
  Run:
  ```bash
  git add -A
  git commit -m "feat: add TargetFinishedTime schema field and TimeSettings configuration"
  ```

---

### Task 2: Implement DateTimeUtils Helper

**Files:**
- Create: `CMS.Backend/Utils/DateTimeUtils.cs`

- [ ] **Step 1: Create DateTimeUtils helper**
  Create the file `CMS.Backend/Utils/DateTimeUtils.cs` with the following content:
  ```csharp
  using System;

  namespace CMS.Backend.Utils
  {
      public static class DateTimeUtils
      {
          public static DateTime GetVietnamTime()
          {
              var utcNow = DateTime.UtcNow;
              try
              {
                  var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                  return TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);
              }
              catch
              {
                  try
                  {
                      var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                      return TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);
                  }
                  catch
                  {
                      return utcNow.AddHours(7);
                  }
              }
          }
      }
  }
  ```

- [ ] **Step 2: Compile to verify syntax**
  Run: `dotnet build CMS.Backend/CMS.Backend.csproj`
  Expected: Build succeeded with 0 errors.

- [ ] **Step 3: Commit utility helper**
  Run:
  ```bash
  git add CMS.Backend/Utils/DateTimeUtils.cs
  git commit -m "feat: add DateTimeUtils for cross-platform Vietnam time retrieval"
  ```

---

### Task 3: Integrate settings and validation in Backend Services

**Files:**
- Modify: `CMS.Backend/Services/DeliverySlotService.cs`
- Modify: `CMS.Backend/Services/OrderService.cs`

- [ ] **Step 1: Update DeliverySlotService.cs to use TimeSettings**
  Open `CMS.Backend/Services/DeliverySlotService.cs`. Inject `TimeSettings` and replace hardcoded capacity limits:
  ```csharp
          private readonly IApplicationDbContext _context;
          private readonly TimeSettings _timeSettings;

          private static readonly string[] DefaultTimeSlots =
          {
              "08:00-10:00",
              "10:00-12:00",
              "13:00-15:00",
              "15:00-17:00",
              "17:00-19:00",
              "19:00-21:00"
          };

          public DeliverySlotService(IApplicationDbContext context, TimeSettings timeSettings)
          {
              _context = context;
              _timeSettings = timeSettings;
          }
  ```
  Replace `Available = 5` (approx. line 68) with `Available = _timeSettings.MaxOrdersPerSlot`.
  Replace `MaxCapacity = 5` (approx. line 100) with `MaxCapacity = _timeSettings.MaxOrdersPerSlot`.

- [ ] **Step 2: Update OrderService.cs CreateOrder validation**
  Open `CMS.Backend/Services/OrderService.cs`. Inject `TimeSettings` in the constructor.
  At the beginning of `CreateOrder`, add the past-date, lead-time, and target finished time calculations:
  ```csharp
                  var vnNow = DateTimeUtils.GetVietnamTime();

                  if (deliveryDate.HasValue)
                  {
                      // 1. Anti-hack check: check past date
                      if (deliveryDate.Value.Date < vnNow.Date)
                      {
                          return (false, "Ngày giao hàng không hợp lệ", 0);
                      }

                      // 2. Lead-Time buffer validation (2 hours)
                      if (deliveryDate.Value.Date == vnNow.Date && !string.IsNullOrEmpty(deliveryTimeSlot))
                      {
                          var parts = deliveryTimeSlot.Split('-');
                          if (parts.Length > 0)
                          {
                              var startStr = parts[0].Trim();
                              if (TimeSpan.TryParse(startStr, out var startTime))
                              {
                                  var limitTime = vnNow.TimeOfDay.Add(TimeSpan.FromHours(_timeSettings.LeadTimeHours));
                                  if (startTime < limitTime)
                                  {
                                      return (false, "Khung giờ chọn không khả dụng, vui lòng chọn khung giờ khác.", 0);
                                  }
                              }
                          }
                      }
                  }
  ```
  And inside the `new Order` instantiation block, calculate and map `TargetFinishedTime`:
  ```csharp
                  DateTime? targetFinishedTime = null;
                  if (deliveryDate.HasValue && !string.IsNullOrEmpty(deliveryTimeSlot))
                  {
                      var parts = deliveryTimeSlot.Split('-');
                      if (parts.Length > 0)
                      {
                          var startStr = parts[0].Trim();
                          if (TimeSpan.TryParse(startStr, out var startTime))
                          {
                              targetFinishedTime = deliveryDate.Value.Date.Add(startTime).AddMinutes(-_timeSettings.PreShippingMinutes);
                          }
                      }
                  }
  ```
  Map it to the entity:
  ```csharp
                  var newOrder = new Order
                  {
                      OrderDate = orderDate ?? DateTime.Now,
                      CustomerId = customerId,
                      Status = initialStatus,
                      Notes = notes,
                      PaymentMethod = method,
                      PaymentStatus = PaymentStatus.Pending,
                      DeliveryDate = deliveryDate,
                      DeliveryTimeSlot = deliveryTimeSlot,
                      DeliveryDistrict = deliveryDistrict,
                      DeliveryAddress = deliveryAddress,
                      TargetFinishedTime = targetFinishedTime
                  };
  ```

- [ ] **Step 3: Compile to verify build**
  Run: `dotnet build CMS.Backend/CMS.Backend.csproj`
  Expected: Build succeeded with 0 errors.

- [ ] **Step 4: Commit changes**
  Run:
  ```bash
  git add CMS.Backend/Services/DeliverySlotService.cs CMS.Backend/Services/OrderService.cs
  git commit -m "feat: implement backend time validations and TargetFinishedTime auto-calculation"
  ```

---

### Task 4: Update Frontend Validation and Slots Filter

**Files:**
- Modify: `cms.frontend/src/pages/checkout/index.tsx`

- [ ] **Step 1: Implement getVietnamTodayString helper**
  Open `cms.frontend/src/pages/checkout/index.tsx`. Define the helpers near the top of the file:
  ```typescript
  const getVietnamTodayString = () => {
    const options = { timeZone: 'Asia/Ho_Chi_Minh', year: 'numeric' as const, month: '2-digit' as const, day: '2-digit' as const };
    const formatter = new Intl.DateTimeFormat('en-CA', options);
    return formatter.format(new Date());
  };

  const DEFAULT_SLOTS = [
    { value: '08:00-10:00', label: '08:00 - 10:00 (Sáng)', startHour: 8 },
    { value: '10:00-12:00', label: '10:00 - 12:00 (Sáng)', startHour: 10 },
    { value: '13:00-15:00', label: '13:00 - 15:00 (Chiều)', startHour: 13 },
    { value: '15:00-17:00', label: '15:00 - 17:00 (Chiều)', startHour: 15 },
    { value: '17:00-19:00', label: '17:00 - 19:00 (Tối)', startHour: 17 },
    { value: '19:00-21:00', label: '19:00 - 21:00 (Tối)', startHour: 19 },
  ];
  ```

- [ ] **Step 2: Update min date logic and add slot filter function**
  Inside the `CheckoutPage` component:
  Add the slot filter logic:
  ```typescript
    const selectedDate = watch('deliveryDate');

    const getFilteredSlots = () => {
      const todayStr = getVietnamTodayString();
      if (selectedDate === todayStr) {
        const now = new Date();
        const formatter = new Intl.DateTimeFormat('en-US', {
          timeZone: 'Asia/Ho_Chi_Minh',
          hour: 'numeric',
          minute: 'numeric',
          hour12: false
        });
        const parts = formatter.formatToParts(now);
        const hourPart = parts.find(p => p.type === 'hour')?.value;
        const minutePart = parts.find(p => p.type === 'minute')?.value;
        
        const currentVnHour = hourPart ? parseInt(hourPart, 10) : now.getHours();
        const currentVnMinute = minutePart ? parseInt(minutePart, 10) : now.getMinutes();
        
        const currentVnTime = currentVnHour + (currentVnMinute / 60.0);
        const limitTime = currentVnTime + 2.0; // Lead-Time Rule = 2 hours

        return DEFAULT_SLOTS.filter(slot => slot.startHour >= limitTime);
      }
      return DEFAULT_SLOTS;
    };
  ```

- [ ] **Step 3: Update input elements rendering**
  In the JSX rendering of the date input (approx. line 271):
  ```tsx
                              <input
                                  type="date"
                                  {...register('deliveryDate')}
                                  min={getVietnamTodayString()}
                                  className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm font-semibold tracking-widest text-on-surface"
                              />
  ```
  In the rendering of the select element (approx. lines 278-290):
  ```tsx
                              <select
                                  {...register('deliveryTimeSlot')}
                                  className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm font-semibold tracking-widest text-on-surface"
                              >
                                  <option value="">-- CHỌN KHUNG GIỜ --</option>
                                  {getFilteredSlots().map(slot => (
                                      <option key={slot.value} value={slot.value}>
                                          {slot.label}
                                      </option>
                                  ))}
                              </select>
  ```

- [ ] **Step 4: Verify type safety and compilation**
  Run:
  ```powershell
  npx tsc --noEmit
  ```
  Expected: Command completed successfully with 0 errors.

- [ ] **Step 5: Commit frontend changes**
  Run:
  ```bash
  git add cms.frontend/src/pages/checkout/index.tsx
  git commit -m "feat: implement frontend Vietnam timezone minDate and Lead-Time slot filtering"
  ```
