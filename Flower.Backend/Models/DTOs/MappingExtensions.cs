using System.Collections.Generic;
using System.Linq;
using Flower.Data.Entities;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Models.DTOs
{
    public static class MappingExtensions
    {
        public static UserDTO ToDTO(this User user)
        {
            if (user == null) return null;
            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role
            };
        }

        public static User ToEntity(this CreateUserDTO dto)
        {
            if (dto == null) return null;
            return new User
            {
                Username = dto.Username,
                FullName = dto.FullName,
                Role = dto.Role
            };
        }

        public static CategoryDTO ToDTO(this Category category)
        {
            if (category == null) return null;
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name ?? "",
                Description = category.Description,
                Slug = category.Slug,
                Posts = category.Posts?.Select(p => p.ToDTO()).ToList()
            };
        }

        public static Category ToEntity(this CreateCategoryDTO dto)
        {
            if (dto == null) return null;
            return new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Slug = dto.Slug
            };
        }

        public static void UpdateEntity(this UpdateCategoryDTO dto, Category entity)
        {
            if (dto == null || entity == null) return;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Slug = dto.Slug;
        }

        public static CategoryProductDTO ToDTO(this CategoryProduct categoryProduct)
        {
            if (categoryProduct == null) return null;
            return new CategoryProductDTO
            {
                Id = categoryProduct.Id,
                Name = categoryProduct.Name ?? "",
                Description = categoryProduct.Description,
                Slug = categoryProduct.Slug
            };
        }

        public static CategoryProduct ToEntity(this CreateCategoryProductDTO dto)
        {
            if (dto == null) return null;
            return new CategoryProduct
            {
                Name = dto.Name,
                Description = dto.Description,
                Slug = dto.Slug
            };
        }

        public static void UpdateEntity(this UpdateCategoryProductDTO dto, CategoryProduct entity)
        {
            if (dto == null || entity == null) return;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Slug = dto.Slug;
        }

        public static ProductDTO ToDTO(this Product product)
        {
            if (product == null) return null;
            return new ProductDTO
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name ?? "",
                Description = product.Description,
                Slug = product.Slug,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryProductId = product.CategoryProductId,
                CategoryProductName = product.CategoryProduct?.Name,
                ViewCount = product.ViewCount,
                AddToCartCount = product.AddToCartCount,
                OriginalPrice = product.Price,
                CurrentPrice = product.Price,
                IsFlashSale = false
            };
        }

        public static Product ToEntity(this CreateProductDTO dto)
        {
            if (dto == null) return null;
            return new Product
            {
                Sku = dto.Sku,
                Name = dto.Name,
                Description = dto.Description,
                Slug = dto.Slug,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageUrl = dto.ImageUrl,
                CategoryProductId = dto.CategoryProductId
            };
        }

        public static void UpdateEntity(this UpdateProductDTO dto, Product entity)
        {
            if (dto == null || entity == null) return;
            entity.Sku = dto.Sku;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Slug = dto.Slug;
            entity.Price = dto.Price;
            entity.StockQuantity = dto.StockQuantity;
            entity.ImageUrl = dto.ImageUrl;
            entity.CategoryProductId = dto.CategoryProductId;
        }

        public static CustomerDTO ToDTO(this Customer customer)
        {
            if (customer == null) return null;
            return new CustomerDTO
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                TotalOrders = customer.TotalOrders,
                SuccessfulDeliveries = customer.SuccessfulDeliveries,
                FailedDeliveries = customer.FailedDeliveries,
                IsBlacklisted = customer.IsBlacklisted,
                FraudScore = customer.FraudScore
            };
        }

        public static Customer ToEntity(this CreateCustomerDTO dto)
        {
            if (dto == null) return null;
            return new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                PasswordHash = dto.PasswordHash
            };
        }

        public static void UpdateEntity(this UpdateCustomerDTO dto, Customer entity)
        {
            if (dto == null || entity == null) return;
            entity.FullName = dto.FullName;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.PasswordHash))
            {
                entity.PasswordHash = dto.PasswordHash;
            }
        }

        public static OrderDTO ToDTO(this Order order)
        {
            if (order == null) return null;

            var details = new List<OrderDetailDTO>();
            if (order.OrderDetails != null)
            {
                foreach (var detail in order.OrderDetails)
                {
                    details.Add(detail.ToDTO());
                }
            }

            return new OrderDTO
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.FullName,
                CustomerEmail = order.Customer?.Email,
                CustomerPhone = order.Customer?.Phone,
                Status = order.Status,
                Notes = order.Notes,
                OrderDetails = details,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                PaymentTransactionId = order.PaymentTransactionId,
                PaymentPaidAt = order.PaymentPaidAt,
                DeliveryDate = order.DeliveryDate,
                DeliveryTimeSlot = order.DeliveryTimeSlot,
                DeliveryDistrict = order.DeliveryDistrict,
                DeliveryAddress = order.DeliveryAddress,
                RecipientName = order.RecipientName,
                RecipientPhone = order.RecipientPhone,
                CancelledAt = order.CancelledAt,
                CancellationReason = order.CancellationReason,
                CancelledBy = order.CancelledBy,
                CancellationFee = order.CancellationFee,
                IsVerified = order.IsVerified,
                RefundAmount = order.RefundAmount,
                RefundRequestedAt = order.RefundRequestedAt,
                RefundCompletedAt = order.RefundCompletedAt,
                PromotionId = order.PromotionId,
                PromotionName = order.Promotion?.Name,
                CouponId = order.CouponId,
                CouponCode = order.Coupon?.Code,
                OriginalAmount = order.OriginalAmount,
                DiscountAmount = order.DiscountAmount,
                FinalAmount = order.FinalAmount
            };
        }

        public static OrderDetailDTO ToDTO(this OrderDetail detail)
        {
            if (detail == null) return null;
            return new OrderDetailDTO
            {
                Id = detail.Id,
                OrderId = detail.OrderId,
                ProductId = detail.ProductId,
                ProductName = detail.ProductName ?? detail.Product?.Name ?? $"Sản phẩm #{detail.ProductId}",
                ProductImageUrl = detail.ProductImage ?? detail.Product?.ImageUrl,
                SizeVariant = detail.SizeVariant,
                CustomerName = detail.Order?.Customer?.FullName,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice
            };
        }

        public static void UpdateEntity(this UpdateOrderDTO dto, Order entity)
        {
            if (dto == null || entity == null) return;
            entity.CustomerId = dto.CustomerId;
            entity.OrderDate = dto.OrderDate;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.DeliveryDate = dto.DeliveryDate;
            entity.DeliveryTimeSlot = dto.DeliveryTimeSlot;
            entity.DeliveryDistrict = dto.DeliveryDistrict;
            entity.DeliveryAddress = dto.DeliveryAddress;
            entity.RecipientName = dto.RecipientName;
            entity.RecipientPhone = dto.RecipientPhone;
        }

        public static PaymentDTO ToDTO(this Payment payment)
        {
            if (payment == null) return null;
            return new PaymentDTO
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                PaidAt = payment.PaidAt,
                RefundedAt = payment.RefundedAt,
                RefundTransactionId = payment.RefundTransactionId,
                RefundResponseCode = payment.RefundResponseCode,
                RefundedBy = payment.RefundedBy,
                RefundNote = payment.RefundNote,
                Notes = payment.Notes
            };
        }

        public static DeliverySlotDTO ToDTO(this DeliverySlot slot)
        {
            if (slot == null) return null;
            return new DeliverySlotDTO
            {
                Id = slot.Id,
                ProductId = slot.ProductId,
                DeliveryDate = slot.DeliveryDate,
                TimeSlot = slot.TimeSlot,
                MaxCapacity = slot.MaxCapacity,
                CurrentBooked = slot.CurrentBooked
            };
        }

        public static AdvertisementDTO ToDTO(this Advertisement ad)
        {
            if (ad == null) return null;
            return new AdvertisementDTO
            {
                Id = ad.Id,
                Title = ad.Title,
                Subtitle = ad.Subtitle,
                ImageUrl = ad.ImageUrl,
                LinkUrl = ad.LinkUrl,
                SortOrder = ad.SortOrder,
                IsActive = ad.IsActive,
                CreatedAt = ad.CreatedAt
            };
        }

        public static PromotionCampaignDTO ToDTO(this PromotionCampaign campaign)
        {
            if (campaign == null) return null;
            return new PromotionCampaignDTO
            {
                Id = campaign.Id,
                Name = campaign.Name,
                Description = campaign.Description,
                PromotionType = campaign.PromotionType,
                DiscountType = campaign.DiscountType,
                DiscountValue = campaign.DiscountValue,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                Priority = campaign.Priority,
                BannerImage = campaign.BannerImage,
                IsStackable = campaign.IsStackable,
                IsActive = campaign.IsActive,
                CreatedAt = campaign.CreatedAt,
                UpdatedAt = campaign.UpdatedAt
            };
        }

        public static PromotionCampaign ToEntity(this CreatePromotionCampaignDTO dto)
        {
            if (dto == null) return null;
            return new PromotionCampaign
            {
                Name = dto.Name,
                Description = dto.Description,
                PromotionType = dto.PromotionType,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Priority = dto.Priority,
                BannerImage = dto.BannerImage,
                IsStackable = dto.IsStackable,
                IsActive = dto.IsActive
            };
        }

        public static void UpdateEntity(this UpdatePromotionCampaignDTO dto, PromotionCampaign entity)
        {
            if (dto == null || entity == null) return;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.PromotionType = dto.PromotionType;
            entity.DiscountType = dto.DiscountType;
            entity.DiscountValue = dto.DiscountValue;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.Priority = dto.Priority;
            entity.BannerImage = dto.BannerImage;
            entity.IsStackable = dto.IsStackable;
            entity.IsActive = dto.IsActive;
        }

        public static CouponDTO ToDTO(this Coupon coupon)
        {
            if (coupon == null) return null;
            return new CouponDTO
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Description = coupon.Description,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                MinimumOrderAmount = coupon.MinimumOrderAmount,
                MaximumDiscountAmount = coupon.MaximumDiscountAmount,
                UsageLimit = coupon.UsageLimit,
                UsedCount = coupon.UsedCount,
                UsagePerCustomer = coupon.UsagePerCustomer,
                CustomerId = coupon.CustomerId,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                IsPublic = coupon.IsPublic,
                IsActive = coupon.IsActive,
                CreatedAt = coupon.CreatedAt,
                UpdatedAt = coupon.UpdatedAt
            };
        }

        public static Coupon ToEntity(this CreateCouponDTO dto)
        {
            if (dto == null) return null;
            return new Coupon
            {
                Code = dto.Code,
                Description = dto.Description,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinimumOrderAmount = dto.MinimumOrderAmount,
                MaximumDiscountAmount = dto.MaximumDiscountAmount,
                UsageLimit = dto.UsageLimit,
                UsagePerCustomer = dto.UsagePerCustomer,
                CustomerId = dto.CustomerId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsPublic = dto.IsPublic,
                IsActive = dto.IsActive
            };
        }

        public static void UpdateEntity(this UpdateCouponDTO dto, Coupon entity)
        {
            if (dto == null || entity == null) return;
            entity.Code = dto.Code;
            entity.Description = dto.Description;
            entity.DiscountType = dto.DiscountType;
            entity.DiscountValue = dto.DiscountValue;
            entity.MinimumOrderAmount = dto.MinimumOrderAmount;
            entity.MaximumDiscountAmount = dto.MaximumDiscountAmount;
            entity.UsageLimit = dto.UsageLimit;
            entity.UsagePerCustomer = dto.UsagePerCustomer;
            entity.CustomerId = dto.CustomerId;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.IsPublic = dto.IsPublic;
            entity.IsActive = dto.IsActive;
        }

        public static CouponUsageDTO ToDTO(this CouponUsage usage)
        {
            if (usage == null) return null;
            return new CouponUsageDTO
            {
                Id = usage.Id,
                CouponId = usage.CouponId,
                CustomerId = usage.CustomerId,
                OrderId = usage.OrderId,
                DiscountAmount = usage.DiscountAmount,
                UsedAt = usage.UsedAt,
                CouponCode = usage.Coupon?.Code,
                CustomerName = usage.Customer?.FullName
            };
        }

        public static PostDTO ToDTO(this Post post)
        {
            if (post == null) return null;
            return new PostDTO
            {
                Id = post.Id,
                Title = post.Title ?? "",
                Content = post.Content ?? "",
                Summary = TruncateSummary(post.Summary, post.Content),
                Slug = post.Slug,
                ImageUrl = post.ImageUrl,
                CreatedDate = post.CreatedDate,
                CategoryId = post.CategoryId,
                CategoryName = post.Category?.Name
            };
        }

        private static string StripHtml(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            var clean = System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
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

        public static Post ToEntity(this CreatePostDTO dto)
        {
            if (dto == null) return null;
            return new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                Summary = TruncateSummary(dto.Summary, dto.Content),
                Slug = dto.Slug,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };
        }

        public static void UpdateEntity(this UpdatePostDTO dto, Post entity)
        {
            if (dto == null || entity == null) return;
            entity.Title = dto.Title;
            entity.Content = dto.Content;
            entity.Summary = TruncateSummary(dto.Summary, dto.Content);
            entity.Slug = dto.Slug;
            entity.ImageUrl = dto.ImageUrl;
            entity.CategoryId = dto.CategoryId;
        }
    }
}
