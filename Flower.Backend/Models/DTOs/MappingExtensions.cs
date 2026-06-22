using System.Collections.Generic;
using System.Linq;
using Flower.Data.Entities;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Models.DTOs
{
    public static class MappingExtensions
    {
        // === User Mapping ===
        public static UserDTO ToDTO(this User user)
        {
            if (user == null) return null;
            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
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

        // === Category Mapping ===
        public static CategoryDTO ToDTO(this Category category)
        {
            if (category == null) return null;
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Posts = category.Posts?.Select(p => p.ToDTO()).ToList()
            };
        }

        public static Category ToEntity(this CreateCategoryDTO dto)
        {
            if (dto == null) return null;
            return new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };
        }

        public static void UpdateEntity(this UpdateCategoryDTO dto, Category entity)
        {
            if (dto == null || entity == null) return;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
        }

        // === CategoryProduct Mapping ===
        public static CategoryProductDTO ToDTO(this CategoryProduct categoryProduct)
        {
            if (categoryProduct == null) return null;
            return new CategoryProductDTO
            {
                Id = categoryProduct.Id,
                Name = categoryProduct.Name,
                Description = categoryProduct.Description
            };
        }

        public static CategoryProduct ToEntity(this CreateCategoryProductDTO dto)
        {
            if (dto == null) return null;
            return new CategoryProduct
            {
                Name = dto.Name,
                Description = dto.Description
            };
        }

        public static void UpdateEntity(this UpdateCategoryProductDTO dto, CategoryProduct entity)
        {
            if (dto == null || entity == null) return;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
        }

        // === Product Mapping ===
        public static ProductDTO ToDTO(this Product product)
        {
            if (product == null) return null;
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryProductId = product.CategoryProductId,
                CategoryProductName = product.CategoryProduct?.Name
            };
        }

        public static Product ToEntity(this CreateProductDTO dto)
        {
            if (dto == null) return null;
            return new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageUrl = dto.ImageUrl,
                CategoryProductId = dto.CategoryProductId
            };
        }

        public static void UpdateEntity(this UpdateProductDTO dto, Product entity)
        {
            if (dto == null || entity == null) return;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.StockQuantity = dto.StockQuantity;
            entity.ImageUrl = dto.ImageUrl;
            entity.CategoryProductId = dto.CategoryProductId;
        }

        // === Customer Mapping ===
        public static CustomerDTO ToDTO(this Customer customer)
        {
            if (customer == null) return null;
            return new CustomerDTO
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
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

        // === Order Mapping ===
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
                Status = order.Status,
                Notes = order.Notes,
                OrderDetails = details
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
                ProductName = detail.Product?.Name,
                ProductImageUrl = detail.Product?.ImageUrl,
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
        }

        // === Post Mapping ===
        public static PostDTO ToDTO(this Post post)
        {
            if (post == null) return null;
            return new PostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Summary = post.Summary,
                ImageUrl = post.ImageUrl,
                CreatedDate = post.CreatedDate,
                CategoryId = post.CategoryId,
                CategoryName = post.Category?.Name
            };
        }

        public static Post ToEntity(this CreatePostDTO dto)
        {
            if (dto == null) return null;
            return new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                Summary = !string.IsNullOrWhiteSpace(dto.Summary)
                    ? dto.Summary
                    : (dto.Content?.Length > 100 ? dto.Content.Substring(0, 100) : dto.Content),
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };
        }

        public static void UpdateEntity(this UpdatePostDTO dto, Post entity)
        {
            if (dto == null || entity == null) return;
            entity.Title = dto.Title;
            entity.Content = dto.Content;
            entity.Summary = !string.IsNullOrWhiteSpace(dto.Summary)
                ? dto.Summary
                : (dto.Content?.Length > 100 ? dto.Content.Substring(0, 100) : dto.Content);
            entity.ImageUrl = dto.ImageUrl;
            entity.CategoryId = dto.CategoryId;
        }
    }
}
