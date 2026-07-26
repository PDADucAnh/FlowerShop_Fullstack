using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flower.Data.Migrations
{
    /// <inheritdoc />
    public partial class SwitchToSnakeCaseNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CouponUsages_Coupons_CouponId",
                table: "CouponUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_CouponUsages_Customers_CustomerId",
                table: "CouponUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_CouponUsages_Orders_OrderId",
                table: "CouponUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddresses_Customers_CustomerId",
                table: "CustomerAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPaymentPreferences_Customers_CustomerId",
                table: "CustomerPaymentPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPaymentPreferences_PaymentMethods_PaymentMethodId",
                table: "CustomerPaymentPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliverySlots_Products_ProductId",
                table: "DeliverySlots");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailHistories_Customers_CustomerId",
                table: "EmailHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashSaleProducts_FlashSales_FlashSaleId",
                table: "FlashSaleProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashSaleProducts_Products_ProductId",
                table: "FlashSaleProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Customers_CustomerId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Coupons_CouponId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PromotionCampaigns_PromotionId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentAttempts_Payments_PaymentId",
                table: "PaymentAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentMethods_PaymentMethodId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_CategoriesProducts_CategoryProductId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Products_ProductId",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Products_ProductId1",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionProducts_Products_ProductId",
                table: "PromotionProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionProducts_PromotionCampaigns_PromotionId",
                table: "PromotionProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Orders_OrderId",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Refunds",
                table: "Refunds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Sku",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pages",
                table: "Pages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ResetToken",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Coupons",
                table: "Coupons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Advertisements",
                table: "Advertisements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemSettings",
                table: "SystemSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PromotionProducts",
                table: "PromotionProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PromotionCampaigns",
                table: "PromotionCampaigns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductVariants",
                table: "ProductVariants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhoneBlacklists",
                table: "PhoneBlacklists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentMethods",
                table: "PaymentMethods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentAttempts",
                table: "PaymentAttempts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDetails",
                table: "OrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlashSales",
                table: "FlashSales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlashSaleProducts",
                table: "FlashSaleProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailHistories",
                table: "EmailHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliverySlots",
                table: "DeliverySlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerPaymentPreferences",
                table: "CustomerPaymentPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerAddresses",
                table: "CustomerAddresses");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddresses_CustomerId_IsDefault",
                table: "CustomerAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CouponUsages",
                table: "CouponUsages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoriesProducts",
                table: "CategoriesProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CancellationPolicies",
                table: "CancellationPolicies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdminNotifications",
                table: "AdminNotifications");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Refunds",
                newName: "refunds");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "products");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "posts");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "payments");

            migrationBuilder.RenameTable(
                name: "Pages",
                newName: "pages");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "orders");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "notifications");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "customers");

            migrationBuilder.RenameTable(
                name: "Coupons",
                newName: "coupons");

            migrationBuilder.RenameTable(
                name: "Contacts",
                newName: "contacts");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "categories");

            migrationBuilder.RenameTable(
                name: "Advertisements",
                newName: "advertisements");

            migrationBuilder.RenameTable(
                name: "SystemSettings",
                newName: "system_settings");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "refresh_tokens");

            migrationBuilder.RenameTable(
                name: "PromotionProducts",
                newName: "promotion_products");

            migrationBuilder.RenameTable(
                name: "PromotionCampaigns",
                newName: "promotion_campaigns");

            migrationBuilder.RenameTable(
                name: "ProductVariants",
                newName: "product_variants");

            migrationBuilder.RenameTable(
                name: "PhoneBlacklists",
                newName: "phone_blacklists");

            migrationBuilder.RenameTable(
                name: "PaymentMethods",
                newName: "payment_methods");

            migrationBuilder.RenameTable(
                name: "PaymentAttempts",
                newName: "payment_attempts");

            migrationBuilder.RenameTable(
                name: "OrderDetails",
                newName: "order_details");

            migrationBuilder.RenameTable(
                name: "FlashSales",
                newName: "flash_sales");

            migrationBuilder.RenameTable(
                name: "FlashSaleProducts",
                newName: "flash_sale_products");

            migrationBuilder.RenameTable(
                name: "EmailHistories",
                newName: "email_histories");

            migrationBuilder.RenameTable(
                name: "DeliverySlots",
                newName: "delivery_slots");

            migrationBuilder.RenameTable(
                name: "CustomerPaymentPreferences",
                newName: "customer_payment_preferences");

            migrationBuilder.RenameTable(
                name: "CustomerAddresses",
                newName: "customer_addresses");

            migrationBuilder.RenameTable(
                name: "CouponUsages",
                newName: "coupon_usages");

            migrationBuilder.RenameTable(
                name: "CategoriesProducts",
                newName: "categories_products");

            migrationBuilder.RenameTable(
                name: "CancellationPolicies",
                newName: "cancellation_policies");

            migrationBuilder.RenameTable(
                name: "AdminNotifications",
                newName: "admin_notifications");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "users",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "users",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "users",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "users",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "LastLogin",
                table: "users",
                newName: "last_login");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "users",
                newName: "full_name");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "refunds",
                newName: "reason");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "refunds",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "refunds",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "RequestedBy",
                table: "refunds",
                newName: "requested_by");

            migrationBuilder.RenameColumn(
                name: "RefundType",
                table: "refunds",
                newName: "refund_type");

            migrationBuilder.RenameColumn(
                name: "RefundStatus",
                table: "refunds",
                newName: "refund_status");

            migrationBuilder.RenameColumn(
                name: "RefundPercent",
                table: "refunds",
                newName: "refund_percent");

            migrationBuilder.RenameColumn(
                name: "RefundAmount",
                table: "refunds",
                newName: "refund_amount");

            migrationBuilder.RenameColumn(
                name: "ProcessedAt",
                table: "refunds",
                newName: "processed_at");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "refunds",
                newName: "payment_id");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "refunds",
                newName: "order_id");

            migrationBuilder.RenameColumn(
                name: "GatewayRefundId",
                table: "refunds",
                newName: "gateway_refund_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "refunds",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ApprovedBy",
                table: "refunds",
                newName: "approved_by");

            migrationBuilder.RenameIndex(
                name: "IX_Refunds_PaymentId",
                table: "refunds",
                newName: "ix_refunds_payment_id");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "products",
                newName: "slug");

            migrationBuilder.RenameColumn(
                name: "Sku",
                table: "products",
                newName: "sku");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "products",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Origin",
                table: "products",
                newName: "origin");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "products",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "products",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "products",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ViewCount",
                table: "products",
                newName: "view_count");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "products",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "StockQuantity",
                table: "products",
                newName: "stock_quantity");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "products",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "products",
                newName: "image_url");

            migrationBuilder.RenameColumn(
                name: "FlowerMeaning",
                table: "products",
                newName: "flower_meaning");

            migrationBuilder.RenameColumn(
                name: "DiscountPrice",
                table: "products",
                newName: "discount_price");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "products",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CategoryProductId",
                table: "products",
                newName: "category_product_id");

            migrationBuilder.RenameColumn(
                name: "CareInstruction",
                table: "products",
                newName: "care_instruction");

            migrationBuilder.RenameColumn(
                name: "AddToCartCount",
                table: "products",
                newName: "add_to_cart_count");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryProductId",
                table: "products",
                newName: "ix_products_category_product_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "posts",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "posts",
                newName: "summary");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "posts",
                newName: "slug");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "posts",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "posts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "posts",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "posts",
                newName: "image_url");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "posts",
                newName: "created_date");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "posts",
                newName: "category_id");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_CategoryId",
                table: "posts",
                newName: "ix_posts_category_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "payments",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "payments",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Method",
                table: "payments",
                newName: "method");

            migrationBuilder.RenameColumn(
                name: "Gateway",
                table: "payments",
                newName: "gateway");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "payments",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "payments",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "payments",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "payments",
                newName: "transaction_id");

            migrationBuilder.RenameColumn(
                name: "RefundedBy",
                table: "payments",
                newName: "refunded_by");

            migrationBuilder.RenameColumn(
                name: "RefundedAt",
                table: "payments",
                newName: "refunded_at");

            migrationBuilder.RenameColumn(
                name: "RefundTransactionId",
                table: "payments",
                newName: "refund_transaction_id");

            migrationBuilder.RenameColumn(
                name: "RefundResponseCode",
                table: "payments",
                newName: "refund_response_code");

            migrationBuilder.RenameColumn(
                name: "RefundNote",
                table: "payments",
                newName: "refund_note");

            migrationBuilder.RenameColumn(
                name: "PaymentUrl",
                table: "payments",
                newName: "payment_url");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodId",
                table: "payments",
                newName: "payment_method_id");

            migrationBuilder.RenameColumn(
                name: "PaidAt",
                table: "payments",
                newName: "paid_at");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "payments",
                newName: "order_id");

            migrationBuilder.RenameColumn(
                name: "GatewayResponseCode",
                table: "payments",
                newName: "gateway_response_code");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "payments",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "BankCode",
                table: "payments",
                newName: "bank_code");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PaymentMethodId",
                table: "payments",
                newName: "ix_payments_payment_method_id");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_OrderId",
                table: "payments",
                newName: "ix_payments_order_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "pages",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "pages",
                newName: "slug");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "pages",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "pages",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "pages",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "pages",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "pages",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "orders",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "orders",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "orders",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "VerifiedAt",
                table: "orders",
                newName: "verified_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "orders",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TargetFinishedTime",
                table: "orders",
                newName: "target_finished_time");

            migrationBuilder.RenameColumn(
                name: "ShippingFee",
                table: "orders",
                newName: "shipping_fee");

            migrationBuilder.RenameColumn(
                name: "RefundRequestedAt",
                table: "orders",
                newName: "refund_requested_at");

            migrationBuilder.RenameColumn(
                name: "RefundCompletedAt",
                table: "orders",
                newName: "refund_completed_at");

            migrationBuilder.RenameColumn(
                name: "RefundAmount",
                table: "orders",
                newName: "refund_amount");

            migrationBuilder.RenameColumn(
                name: "RecipientPhone",
                table: "orders",
                newName: "recipient_phone");

            migrationBuilder.RenameColumn(
                name: "RecipientName",
                table: "orders",
                newName: "recipient_name");

            migrationBuilder.RenameColumn(
                name: "PromotionId",
                table: "orders",
                newName: "promotion_id");

            migrationBuilder.RenameColumn(
                name: "PaymentTransactionId",
                table: "orders",
                newName: "payment_transaction_id");

            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "orders",
                newName: "payment_status");

            migrationBuilder.RenameColumn(
                name: "PaymentPaidAt",
                table: "orders",
                newName: "payment_paid_at");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "orders",
                newName: "payment_method");

            migrationBuilder.RenameColumn(
                name: "OriginalAmount",
                table: "orders",
                newName: "original_amount");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "orders",
                newName: "order_date");

            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "orders",
                newName: "is_verified");

            migrationBuilder.RenameColumn(
                name: "FinalAmount",
                table: "orders",
                newName: "final_amount");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "orders",
                newName: "discount_amount");

            migrationBuilder.RenameColumn(
                name: "DeliveryWard",
                table: "orders",
                newName: "delivery_ward");

            migrationBuilder.RenameColumn(
                name: "DeliveryTimeSlot",
                table: "orders",
                newName: "delivery_time_slot");

            migrationBuilder.RenameColumn(
                name: "DeliverySlotId",
                table: "orders",
                newName: "delivery_slot_id");

            migrationBuilder.RenameColumn(
                name: "DeliveryReceiverPhone",
                table: "orders",
                newName: "delivery_receiver_phone");

            migrationBuilder.RenameColumn(
                name: "DeliveryReceiverName",
                table: "orders",
                newName: "delivery_receiver_name");

            migrationBuilder.RenameColumn(
                name: "DeliveryProvince",
                table: "orders",
                newName: "delivery_province");

            migrationBuilder.RenameColumn(
                name: "DeliveryPostalCode",
                table: "orders",
                newName: "delivery_postal_code");

            migrationBuilder.RenameColumn(
                name: "DeliveryDistrict",
                table: "orders",
                newName: "delivery_district");

            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                table: "orders",
                newName: "delivery_date");

            migrationBuilder.RenameColumn(
                name: "DeliveryAddressLine",
                table: "orders",
                newName: "delivery_address_line");

            migrationBuilder.RenameColumn(
                name: "DeliveryAddress",
                table: "orders",
                newName: "delivery_address");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "orders",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "orders",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CouponId",
                table: "orders",
                newName: "coupon_id");

            migrationBuilder.RenameColumn(
                name: "CancelledBy",
                table: "orders",
                newName: "cancelled_by");

            migrationBuilder.RenameColumn(
                name: "CancelledAt",
                table: "orders",
                newName: "cancelled_at");

            migrationBuilder.RenameColumn(
                name: "CancellationReason",
                table: "orders",
                newName: "cancellation_reason");

            migrationBuilder.RenameColumn(
                name: "CancellationFee",
                table: "orders",
                newName: "cancellation_fee");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_PromotionId",
                table: "orders",
                newName: "ix_orders_promotion_id");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerId",
                table: "orders",
                newName: "ix_orders_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CouponId",
                table: "orders",
                newName: "ix_orders_coupon_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "notifications",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "notifications",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "notifications",
                newName: "priority");

            migrationBuilder.RenameColumn(
                name: "Metadata",
                table: "notifications",
                newName: "metadata");

            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "notifications",
                newName: "icon");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "notifications",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "notifications",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ReferenceType",
                table: "notifications",
                newName: "reference_type");

            migrationBuilder.RenameColumn(
                name: "ReadAt",
                table: "notifications",
                newName: "read_at");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "notifications",
                newName: "order_id");

            migrationBuilder.RenameColumn(
                name: "NavigationUrl",
                table: "notifications",
                newName: "navigation_url");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "notifications",
                newName: "is_read");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "notifications",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "notifications",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "customers",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "customers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "customers",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "customers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "customers",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TotalOrders",
                table: "customers",
                newName: "total_orders");

            migrationBuilder.RenameColumn(
                name: "SuccessfulDeliveries",
                table: "customers",
                newName: "successful_deliveries");

            migrationBuilder.RenameColumn(
                name: "ResetTokenExpiry",
                table: "customers",
                newName: "reset_token_expiry");

            migrationBuilder.RenameColumn(
                name: "ResetToken",
                table: "customers",
                newName: "reset_token");

            migrationBuilder.RenameColumn(
                name: "PhoneVerified",
                table: "customers",
                newName: "phone_verified");

            migrationBuilder.RenameColumn(
                name: "LastLogin",
                table: "customers",
                newName: "last_login");

            migrationBuilder.RenameColumn(
                name: "IsBlacklisted",
                table: "customers",
                newName: "is_blacklisted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "customers",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "customers",
                newName: "full_name");

            migrationBuilder.RenameColumn(
                name: "FraudScore",
                table: "customers",
                newName: "fraud_score");

            migrationBuilder.RenameColumn(
                name: "FailedDeliveries",
                table: "customers",
                newName: "failed_deliveries");

            migrationBuilder.RenameColumn(
                name: "EmailVerified",
                table: "customers",
                newName: "email_verified");

            migrationBuilder.RenameColumn(
                name: "DefaultAddressId",
                table: "customers",
                newName: "default_address_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "customers",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_Email",
                table: "customers",
                newName: "ix_customers_email");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "coupons",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "coupons",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "coupons",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UsedCount",
                table: "coupons",
                newName: "used_count");

            migrationBuilder.RenameColumn(
                name: "UsagePerCustomer",
                table: "coupons",
                newName: "usage_per_customer");

            migrationBuilder.RenameColumn(
                name: "UsageLimit",
                table: "coupons",
                newName: "usage_limit");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "coupons",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "coupons",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "MinimumOrderAmount",
                table: "coupons",
                newName: "minimum_order_amount");

            migrationBuilder.RenameColumn(
                name: "MaximumDiscountAmount",
                table: "coupons",
                newName: "maximum_discount_amount");

            migrationBuilder.RenameColumn(
                name: "IsPublic",
                table: "coupons",
                newName: "is_public");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "coupons",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "coupons",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                table: "coupons",
                newName: "discount_value");

            migrationBuilder.RenameColumn(
                name: "DiscountType",
                table: "coupons",
                newName: "discount_type");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "coupons",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "coupons",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "contacts",
                newName: "subject");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "contacts",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "contacts",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "contacts",
                newName: "message");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "contacts",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "contacts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ReadAt",
                table: "contacts",
                newName: "read_at");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "contacts",
                newName: "is_read");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "contacts",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "categories",
                newName: "slug");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "categories",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "categories",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "categories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "categories",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "categories",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "advertisements",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Subtitle",
                table: "advertisements",
                newName: "subtitle");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "advertisements",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "advertisements",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SortOrder",
                table: "advertisements",
                newName: "sort_order");

            migrationBuilder.RenameColumn(
                name: "LinkUrl",
                table: "advertisements",
                newName: "link_url");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "advertisements",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "advertisements",
                newName: "image_url");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "advertisements",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "system_settings",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "system_settings",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "system_settings",
                newName: "key");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "system_settings",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "system_settings",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "refresh_tokens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "refresh_tokens",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "refresh_tokens",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "refresh_tokens",
                newName: "token_hash");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "refresh_tokens",
                newName: "revoked_at");

            migrationBuilder.RenameColumn(
                name: "IsRevoked",
                table: "refresh_tokens",
                newName: "is_revoked");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "refresh_tokens",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "DeviceInfo",
                table: "refresh_tokens",
                newName: "device_info");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "refresh_tokens",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "promotion_products",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PromotionId",
                table: "promotion_products",
                newName: "promotion_id");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "promotion_products",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "promotion_products",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_PromotionProducts_ProductId",
                table: "promotion_products",
                newName: "ix_promotion_products_product_id");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "promotion_campaigns",
                newName: "priority");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "promotion_campaigns",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "promotion_campaigns",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "promotion_campaigns",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "promotion_campaigns",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "promotion_campaigns",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "PromotionType",
                table: "promotion_campaigns",
                newName: "promotion_type");

            migrationBuilder.RenameColumn(
                name: "IsStackable",
                table: "promotion_campaigns",
                newName: "is_stackable");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "promotion_campaigns",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "promotion_campaigns",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                table: "promotion_campaigns",
                newName: "discount_value");

            migrationBuilder.RenameColumn(
                name: "DiscountType",
                table: "promotion_campaigns",
                newName: "discount_type");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "promotion_campaigns",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "BannerImage",
                table: "promotion_campaigns",
                newName: "banner_image");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "product_variants",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "product_variants",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ProductId1",
                table: "product_variants",
                newName: "product_id1");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "product_variants",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "PriceAdjustment",
                table: "product_variants",
                newName: "price_adjustment");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "product_variants",
                newName: "is_default");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariants_ProductId1",
                table: "product_variants",
                newName: "ix_product_variants_product_id1");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariants_ProductId",
                table: "product_variants",
                newName: "ix_product_variants_product_id");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "phone_blacklists",
                newName: "reason");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "phone_blacklists",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "phone_blacklists",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "phone_blacklists",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "phone_blacklists",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "payment_methods",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "payment_methods",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "payment_methods",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "payment_methods",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "payment_methods",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsOnline",
                table: "payment_methods",
                newName: "is_online");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "payment_methods",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "DisplayOrder",
                table: "payment_methods",
                newName: "display_order");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "payment_methods",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentMethods_Code",
                table: "payment_methods",
                newName: "ix_payment_methods_code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "payment_attempts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserAgent",
                table: "payment_attempts",
                newName: "user_agent");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "payment_attempts",
                newName: "payment_id");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "payment_attempts",
                newName: "ip_address");

            migrationBuilder.RenameColumn(
                name: "GatewayResponse",
                table: "payment_attempts",
                newName: "gateway_response");

            migrationBuilder.RenameColumn(
                name: "GatewayRequest",
                table: "payment_attempts",
                newName: "gateway_request");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "payment_attempts",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AttemptNumber",
                table: "payment_attempts",
                newName: "attempt_number");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentAttempts_PaymentId_AttemptNumber",
                table: "payment_attempts",
                newName: "ix_payment_attempts_payment_id_attempt_number");

            migrationBuilder.RenameColumn(
                name: "Subtotal",
                table: "order_details",
                newName: "subtotal");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "order_details",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Discount",
                table: "order_details",
                newName: "discount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "order_details",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "order_details",
                newName: "unit_price");

            migrationBuilder.RenameColumn(
                name: "SizeVariant",
                table: "order_details",
                newName: "size_variant");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "order_details",
                newName: "product_name");

            migrationBuilder.RenameColumn(
                name: "ProductImage",
                table: "order_details",
                newName: "product_image");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "order_details",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "order_details",
                newName: "order_id");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ProductId",
                table: "order_details",
                newName: "ix_order_details_product_id");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrderId",
                table: "order_details",
                newName: "ix_order_details_order_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "flash_sales",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "flash_sales",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "flash_sales",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "flash_sales",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "flash_sales",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "flash_sales",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "flash_sales",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "flash_sales",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "flash_sale_products",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SalePrice",
                table: "flash_sale_products",
                newName: "sale_price");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "flash_sale_products",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "FlashSaleId",
                table: "flash_sale_products",
                newName: "flash_sale_id");

            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "flash_sale_products",
                newName: "discount_percent");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "flash_sale_products",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_FlashSaleProducts_ProductId",
                table: "flash_sale_products",
                newName: "ix_flash_sale_products_product_id");

            migrationBuilder.RenameIndex(
                name: "IX_FlashSaleProducts_FlashSaleId",
                table: "flash_sale_products",
                newName: "ix_flash_sale_products_flash_sale_id");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "email_histories",
                newName: "subject");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "email_histories",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Recipient",
                table: "email_histories",
                newName: "recipient");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "email_histories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "email_histories",
                newName: "sent_at");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "email_histories",
                newName: "order_id");

            migrationBuilder.RenameColumn(
                name: "EmailType",
                table: "email_histories",
                newName: "email_type");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "email_histories",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "email_histories",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_EmailHistories_CustomerId",
                table: "email_histories",
                newName: "ix_email_histories_customer_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "delivery_slots",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "delivery_slots",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TimeSlot",
                table: "delivery_slots",
                newName: "time_slot");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "delivery_slots",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "MaxCapacity",
                table: "delivery_slots",
                newName: "max_capacity");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "delivery_slots",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                table: "delivery_slots",
                newName: "delivery_date");

            migrationBuilder.RenameColumn(
                name: "CurrentBooked",
                table: "delivery_slots",
                newName: "current_booked");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "delivery_slots",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "customer_payment_preferences",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "customer_payment_preferences",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodId",
                table: "customer_payment_preferences",
                newName: "payment_method_id");

            migrationBuilder.RenameColumn(
                name: "LastUsedAt",
                table: "customer_payment_preferences",
                newName: "last_used_at");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "customer_payment_preferences",
                newName: "is_default");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "customer_payment_preferences",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "customer_payment_preferences",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerPaymentPreferences_PaymentMethodId",
                table: "customer_payment_preferences",
                newName: "ix_customer_payment_preferences_payment_method_id");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerPaymentPreferences_CustomerId_PaymentMethodId",
                table: "customer_payment_preferences",
                newName: "ix_customer_payment_preferences_customer_id_payment_method_id");

            migrationBuilder.RenameColumn(
                name: "Ward",
                table: "customer_addresses",
                newName: "ward");

            migrationBuilder.RenameColumn(
                name: "Province",
                table: "customer_addresses",
                newName: "province");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "customer_addresses",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "customer_addresses",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "customer_addresses",
                newName: "latitude");

            migrationBuilder.RenameColumn(
                name: "District",
                table: "customer_addresses",
                newName: "district");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "customer_addresses",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "customer_addresses",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ReceiverPhone",
                table: "customer_addresses",
                newName: "receiver_phone");

            migrationBuilder.RenameColumn(
                name: "ReceiverName",
                table: "customer_addresses",
                newName: "receiver_name");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "customer_addresses",
                newName: "postal_code");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "customer_addresses",
                newName: "is_default");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "customer_addresses",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "customer_addresses",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "customer_addresses",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AddressLine",
                table: "customer_addresses",
                newName: "address_line");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "coupon_usages",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UsedAt",
                table: "coupon_usages",
                newName: "used_at");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "coupon_usages",
                newName: "order_id");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "coupon_usages",
                newName: "discount_amount");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "coupon_usages",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CouponId",
                table: "coupon_usages",
                newName: "coupon_id");

            migrationBuilder.RenameIndex(
                name: "IX_CouponUsages_CustomerId",
                table: "coupon_usages",
                newName: "ix_coupon_usages_customer_id");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "categories_products",
                newName: "slug");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "categories_products",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "categories_products",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "categories_products",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "categories_products",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "categories_products",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "cancellation_policies",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "cancellation_policies",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "cancellation_policies",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "RefundPercent",
                table: "cancellation_policies",
                newName: "refund_percent");

            migrationBuilder.RenameColumn(
                name: "OrderStatus",
                table: "cancellation_policies",
                newName: "order_status");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "cancellation_policies",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "cancellation_policies",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CancellationFeePercent",
                table: "cancellation_policies",
                newName: "cancellation_fee_percent");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "admin_notifications",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "admin_notifications",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "admin_notifications",
                newName: "priority");

            migrationBuilder.RenameColumn(
                name: "Metadata",
                table: "admin_notifications",
                newName: "metadata");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "admin_notifications",
                newName: "message");

            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "admin_notifications",
                newName: "icon");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "admin_notifications",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "admin_notifications",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ReferenceType",
                table: "admin_notifications",
                newName: "reference_type");

            migrationBuilder.RenameColumn(
                name: "ReferenceId",
                table: "admin_notifications",
                newName: "reference_id");

            migrationBuilder.RenameColumn(
                name: "ReadAt",
                table: "admin_notifications",
                newName: "read_at");

            migrationBuilder.RenameColumn(
                name: "NavigationUrl",
                table: "admin_notifications",
                newName: "navigation_url");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "admin_notifications",
                newName: "is_read");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "admin_notifications",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "admin_notifications",
                newName: "created_at");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "users",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "last_login",
                table: "users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "users",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "reason",
                table: "refunds",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "refunds",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "refunds",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "requested_by",
                table: "refunds",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "refund_type",
                table: "refunds",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "refund_status",
                table: "refunds",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "refund_percent",
                table: "refunds",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "processed_at",
                table: "refunds",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "payment_id",
                table: "refunds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                table: "refunds",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "gateway_refund_id",
                table: "refunds",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refunds",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "approved_by",
                table: "refunds",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "products",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "sku",
                table: "products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "origin",
                table: "products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "view_count",
                table: "products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "products",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "stock_quantity",
                table: "products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "products",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                table: "products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "flower_meaning",
                table: "products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "discount_price",
                table: "products",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "products",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "category_product_id",
                table: "products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "care_instruction",
                table: "products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "add_to_cart_count",
                table: "products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "posts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "summary",
                table: "posts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "posts",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "posts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "posts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "posts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                table: "posts",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_date",
                table: "posts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "posts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "notes",
                table: "payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "method",
                table: "payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "gateway",
                table: "payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "payments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "transaction_id",
                table: "payments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "refunded_by",
                table: "payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "refunded_at",
                table: "payments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "refund_transaction_id",
                table: "payments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "refund_response_code",
                table: "payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "refund_note",
                table: "payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "payment_url",
                table: "payments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "payment_method_id",
                table: "payments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "paid_at",
                table: "payments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                table: "payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "gateway_response_code",
                table: "payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "payments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "bank_code",
                table: "payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "pages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "pages",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "pages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "pages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "pages",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "pages",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "pages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "notes",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "verified_at",
                table: "orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "target_finished_time",
                table: "orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "refund_requested_at",
                table: "orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "refund_completed_at",
                table: "orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "refund_amount",
                table: "orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,0)");

            migrationBuilder.AlterColumn<string>(
                name: "recipient_phone",
                table: "orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "recipient_name",
                table: "orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "promotion_id",
                table: "orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "payment_transaction_id",
                table: "orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "payment_status",
                table: "orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "payment_paid_at",
                table: "orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "payment_method",
                table: "orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "order_date",
                table: "orders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "is_verified",
                table: "orders",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "delivery_ward",
                table: "orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_time_slot",
                table: "orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "delivery_slot_id",
                table: "orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_receiver_phone",
                table: "orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_receiver_name",
                table: "orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_province",
                table: "orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_postal_code",
                table: "orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_district",
                table: "orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "delivery_date",
                table: "orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_address_line",
                table: "orders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "delivery_address",
                table: "orders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "customer_id",
                table: "orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "orders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "coupon_id",
                table: "orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cancelled_by",
                table: "orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "cancelled_at",
                table: "orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cancellation_reason",
                table: "orders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "priority",
                table: "notifications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "metadata",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "icon",
                table: "notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "notifications",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "reference_type",
                table: "notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "read_at",
                table: "notifications",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                table: "notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "navigation_url",
                table: "notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_read",
                table: "notifications",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "customer_id",
                table: "notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "notifications",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "customers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "customers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "customers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "total_orders",
                table: "customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "successful_deliveries",
                table: "customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "reset_token_expiry",
                table: "customers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reset_token",
                table: "customers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "phone_verified",
                table: "customers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "last_login",
                table: "customers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_blacklisted",
                table: "customers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "customers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "fraud_score",
                table: "customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "failed_deliveries",
                table: "customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "email_verified",
                table: "customers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "default_address_id",
                table: "customers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "customers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "coupons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "coupons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "coupons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "used_count",
                table: "coupons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "usage_per_customer",
                table: "coupons",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "usage_limit",
                table: "coupons",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "coupons",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "coupons",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_public",
                table: "coupons",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "coupons",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                table: "coupons",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "discount_type",
                table: "coupons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "customer_id",
                table: "coupons",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "coupons",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "subject",
                table: "contacts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "contacts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "contacts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "contacts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "contacts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "contacts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "read_at",
                table: "contacts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_read",
                table: "contacts",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "contacts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "categories",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "categories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "categories",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "categories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "categories",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "categories",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "advertisements",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "subtitle",
                table: "advertisements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "advertisements",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "advertisements",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "sort_order",
                table: "advertisements",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "link_url",
                table: "advertisements",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "advertisements",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                table: "advertisements",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "advertisements",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "system_settings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "system_settings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "key",
                table: "system_settings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "system_settings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "system_settings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "refresh_tokens",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "refresh_tokens",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "refresh_tokens",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "token_hash",
                table: "refresh_tokens",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<DateTime>(
                name: "revoked_at",
                table: "refresh_tokens",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_revoked",
                table: "refresh_tokens",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "expires_at",
                table: "refresh_tokens",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "device_info",
                table: "refresh_tokens",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refresh_tokens",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "promotion_products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "promotion_id",
                table: "promotion_products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "promotion_products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "promotion_products",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "priority",
                table: "promotion_campaigns",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "promotion_campaigns",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "promotion_campaigns",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "promotion_campaigns",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "promotion_campaigns",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "promotion_campaigns",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "promotion_type",
                table: "promotion_campaigns",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "is_stackable",
                table: "promotion_campaigns",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "promotion_campaigns",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                table: "promotion_campaigns",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "discount_type",
                table: "promotion_campaigns",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "promotion_campaigns",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "banner_image",
                table: "promotion_campaigns",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "product_variants",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "product_variants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "product_id1",
                table: "product_variants",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "product_variants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "is_default",
                table: "product_variants",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "reason",
                table: "phone_blacklists",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "phone_blacklists",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "phone_blacklists",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "phone_blacklists",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "phone_blacklists",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "payment_methods",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "payment_methods",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "payment_methods",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "payment_methods",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "payment_methods",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_online",
                table: "payment_methods",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "payment_methods",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "display_order",
                table: "payment_methods",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "payment_methods",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "payment_attempts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "user_agent",
                table: "payment_attempts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "payment_id",
                table: "payment_attempts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ip_address",
                table: "payment_attempts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "gateway_response",
                table: "payment_attempts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "gateway_request",
                table: "payment_attempts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "payment_attempts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "attempt_number",
                table: "payment_attempts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "quantity",
                table: "order_details",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "order_details",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "size_variant",
                table: "order_details",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "product_name",
                table: "order_details",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "product_image",
                table: "order_details",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "order_details",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                table: "order_details",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "flash_sales",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "flash_sales",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "flash_sales",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "flash_sales",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "flash_sales",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "flash_sales",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                table: "flash_sales",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "flash_sales",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "flash_sale_products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "flash_sale_products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "flash_sale_id",
                table: "flash_sale_products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "flash_sale_products",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "subject",
                table: "email_histories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "email_histories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "recipient",
                table: "email_histories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "email_histories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "sent_at",
                table: "email_histories",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                table: "email_histories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email_type",
                table: "email_histories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "customer_id",
                table: "email_histories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "email_histories",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "delivery_slots",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "delivery_slots",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "time_slot",
                table: "delivery_slots",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "delivery_slots",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "max_capacity",
                table: "delivery_slots",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "delivery_slots",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "delivery_date",
                table: "delivery_slots",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "current_booked",
                table: "delivery_slots",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "delivery_slots",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "customer_payment_preferences",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "customer_payment_preferences",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "payment_method_id",
                table: "customer_payment_preferences",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "last_used_at",
                table: "customer_payment_preferences",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_default",
                table: "customer_payment_preferences",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "customer_id",
                table: "customer_payment_preferences",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "customer_payment_preferences",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "ward",
                table: "customer_addresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "province",
                table: "customer_addresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "note",
                table: "customer_addresses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "longitude",
                table: "customer_addresses",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "latitude",
                table: "customer_addresses",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "district",
                table: "customer_addresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "customer_addresses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "customer_addresses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "receiver_phone",
                table: "customer_addresses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "receiver_name",
                table: "customer_addresses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "postal_code",
                table: "customer_addresses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_default",
                table: "customer_addresses",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "customer_addresses",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "customer_id",
                table: "customer_addresses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "customer_addresses",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "address_line",
                table: "customer_addresses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "coupon_usages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "used_at",
                table: "coupon_usages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                table: "coupon_usages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "customer_id",
                table: "coupon_usages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "coupon_id",
                table: "coupon_usages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "categories_products",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "categories_products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "categories_products",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "categories_products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "categories_products",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "categories_products",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "cancellation_policies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "cancellation_policies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "cancellation_policies",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "refund_percent",
                table: "cancellation_policies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "order_status",
                table: "cancellation_policies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "cancellation_policies",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "cancellation_policies",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "cancellation_fee_percent",
                table: "cancellation_policies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "admin_notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "admin_notifications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "priority",
                table: "admin_notifications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "metadata",
                table: "admin_notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "admin_notifications",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "icon",
                table: "admin_notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "admin_notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "admin_notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reference_type",
                table: "admin_notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reference_id",
                table: "admin_notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "read_at",
                table: "admin_notifications",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "navigation_url",
                table: "admin_notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_read",
                table: "admin_notifications",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "admin_notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "admin_notifications",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_refunds",
                table: "refunds",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_products",
                table: "products",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_posts",
                table: "posts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_payments",
                table: "payments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_pages",
                table: "pages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_orders",
                table: "orders",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_notifications",
                table: "notifications",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_customers",
                table: "customers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_coupons",
                table: "coupons",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_contacts",
                table: "contacts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_categories",
                table: "categories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_advertisements",
                table: "advertisements",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_system_settings",
                table: "system_settings",
                column: "key");

            migrationBuilder.AddPrimaryKey(
                name: "pk_refresh_tokens",
                table: "refresh_tokens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_promotion_products",
                table: "promotion_products",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_promotion_campaigns",
                table: "promotion_campaigns",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_product_variants",
                table: "product_variants",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_phone_blacklists",
                table: "phone_blacklists",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_payment_methods",
                table: "payment_methods",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_payment_attempts",
                table: "payment_attempts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_details",
                table: "order_details",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_flash_sales",
                table: "flash_sales",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_flash_sale_products",
                table: "flash_sale_products",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_email_histories",
                table: "email_histories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_delivery_slots",
                table: "delivery_slots",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_customer_payment_preferences",
                table: "customer_payment_preferences",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_customer_addresses",
                table: "customer_addresses",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_coupon_usages",
                table: "coupon_usages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_categories_products",
                table: "categories_products",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_cancellation_policies",
                table: "cancellation_policies",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_admin_notifications",
                table: "admin_notifications",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_products_sku",
                table: "products",
                column: "sku",
                unique: true,
                filter: "[Sku] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ResetToken",
                table: "customers",
                column: "reset_token",
                filter: "[ResetToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_CustomerId_IsDefault",
                table: "customer_addresses",
                columns: new[] { "customer_id", "is_default" },
                filter: "[IsDefault] = 1");

            migrationBuilder.AddForeignKey(
                name: "fk_coupon_usages_coupons_coupon_id",
                table: "coupon_usages",
                column: "coupon_id",
                principalTable: "coupons",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_coupon_usages_customers_customer_id",
                table: "coupon_usages",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_coupon_usages_orders_order_id",
                table: "coupon_usages",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_customer_addresses_customers_customer_id",
                table: "customer_addresses",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_customer_payment_preferences_customers_customer_id",
                table: "customer_payment_preferences",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_customer_payment_preferences_payment_methods_payment_method_id",
                table: "customer_payment_preferences",
                column: "payment_method_id",
                principalTable: "payment_methods",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_delivery_slots_products_product_id",
                table: "delivery_slots",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_email_histories_customers_customer_id",
                table: "email_histories",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_flash_sale_products_flash_sales_flash_sale_id",
                table: "flash_sale_products",
                column: "flash_sale_id",
                principalTable: "flash_sales",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_flash_sale_products_products_product_id",
                table: "flash_sale_products",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_notifications_customers_customer_id",
                table: "notifications",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_order_details_orders_order_id",
                table: "order_details",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_order_details_products_product_id",
                table: "order_details",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_coupons_coupon_id",
                table: "orders",
                column: "coupon_id",
                principalTable: "coupons",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_customers_customer_id",
                table: "orders",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_promotion_campaigns_promotion_id",
                table: "orders",
                column: "promotion_id",
                principalTable: "promotion_campaigns",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_payment_attempts_payments_payment_id",
                table: "payment_attempts",
                column: "payment_id",
                principalTable: "payments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_payments_orders_order_id",
                table: "payments",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_payments_payment_methods_payment_method_id",
                table: "payments",
                column: "payment_method_id",
                principalTable: "payment_methods",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_posts_categories_category_id",
                table: "posts",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_product_variants_products_product_id",
                table: "product_variants",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_product_variants_products_product_id1",
                table: "product_variants",
                column: "product_id1",
                principalTable: "products",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_products_categories_products_category_product_id",
                table: "products",
                column: "category_product_id",
                principalTable: "categories_products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_promotion_products_products_product_id",
                table: "promotion_products",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_promotion_products_promotion_campaigns_promotion_id",
                table: "promotion_products",
                column: "promotion_id",
                principalTable: "promotion_campaigns",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                table: "refresh_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_refunds_orders_order_id",
                table: "refunds",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_refunds_payments_payment_id",
                table: "refunds",
                column: "payment_id",
                principalTable: "payments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_coupon_usages_coupons_coupon_id",
                table: "coupon_usages");

            migrationBuilder.DropForeignKey(
                name: "fk_coupon_usages_customers_customer_id",
                table: "coupon_usages");

            migrationBuilder.DropForeignKey(
                name: "fk_coupon_usages_orders_order_id",
                table: "coupon_usages");

            migrationBuilder.DropForeignKey(
                name: "fk_customer_addresses_customers_customer_id",
                table: "customer_addresses");

            migrationBuilder.DropForeignKey(
                name: "fk_customer_payment_preferences_customers_customer_id",
                table: "customer_payment_preferences");

            migrationBuilder.DropForeignKey(
                name: "fk_customer_payment_preferences_payment_methods_payment_method_id",
                table: "customer_payment_preferences");

            migrationBuilder.DropForeignKey(
                name: "fk_delivery_slots_products_product_id",
                table: "delivery_slots");

            migrationBuilder.DropForeignKey(
                name: "fk_email_histories_customers_customer_id",
                table: "email_histories");

            migrationBuilder.DropForeignKey(
                name: "fk_flash_sale_products_flash_sales_flash_sale_id",
                table: "flash_sale_products");

            migrationBuilder.DropForeignKey(
                name: "fk_flash_sale_products_products_product_id",
                table: "flash_sale_products");

            migrationBuilder.DropForeignKey(
                name: "fk_notifications_customers_customer_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "fk_order_details_orders_order_id",
                table: "order_details");

            migrationBuilder.DropForeignKey(
                name: "fk_order_details_products_product_id",
                table: "order_details");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_coupons_coupon_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_customers_customer_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_promotion_campaigns_promotion_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_payment_attempts_payments_payment_id",
                table: "payment_attempts");

            migrationBuilder.DropForeignKey(
                name: "fk_payments_orders_order_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "fk_payments_payment_methods_payment_method_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "fk_posts_categories_category_id",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "fk_product_variants_products_product_id",
                table: "product_variants");

            migrationBuilder.DropForeignKey(
                name: "fk_product_variants_products_product_id1",
                table: "product_variants");

            migrationBuilder.DropForeignKey(
                name: "fk_products_categories_products_category_product_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "fk_promotion_products_products_product_id",
                table: "promotion_products");

            migrationBuilder.DropForeignKey(
                name: "fk_promotion_products_promotion_campaigns_promotion_id",
                table: "promotion_products");

            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                table: "refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "fk_refunds_orders_order_id",
                table: "refunds");

            migrationBuilder.DropForeignKey(
                name: "fk_refunds_payments_payment_id",
                table: "refunds");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_refunds",
                table: "refunds");

            migrationBuilder.DropPrimaryKey(
                name: "pk_products",
                table: "products");

            migrationBuilder.DropIndex(
                name: "ix_products_sku",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "pk_posts",
                table: "posts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_payments",
                table: "payments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pages",
                table: "pages");

            migrationBuilder.DropPrimaryKey(
                name: "pk_orders",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status",
                table: "orders");

            migrationBuilder.DropPrimaryKey(
                name: "pk_notifications",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "pk_customers",
                table: "customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ResetToken",
                table: "customers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_coupons",
                table: "coupons");

            migrationBuilder.DropPrimaryKey(
                name: "pk_contacts",
                table: "contacts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_categories",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_advertisements",
                table: "advertisements");

            migrationBuilder.DropPrimaryKey(
                name: "pk_system_settings",
                table: "system_settings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_promotion_products",
                table: "promotion_products");

            migrationBuilder.DropPrimaryKey(
                name: "pk_promotion_campaigns",
                table: "promotion_campaigns");

            migrationBuilder.DropPrimaryKey(
                name: "pk_product_variants",
                table: "product_variants");

            migrationBuilder.DropPrimaryKey(
                name: "pk_phone_blacklists",
                table: "phone_blacklists");

            migrationBuilder.DropPrimaryKey(
                name: "pk_payment_methods",
                table: "payment_methods");

            migrationBuilder.DropPrimaryKey(
                name: "pk_payment_attempts",
                table: "payment_attempts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_order_details",
                table: "order_details");

            migrationBuilder.DropPrimaryKey(
                name: "pk_flash_sales",
                table: "flash_sales");

            migrationBuilder.DropPrimaryKey(
                name: "pk_flash_sale_products",
                table: "flash_sale_products");

            migrationBuilder.DropPrimaryKey(
                name: "pk_email_histories",
                table: "email_histories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_delivery_slots",
                table: "delivery_slots");

            migrationBuilder.DropPrimaryKey(
                name: "pk_customer_payment_preferences",
                table: "customer_payment_preferences");

            migrationBuilder.DropPrimaryKey(
                name: "pk_customer_addresses",
                table: "customer_addresses");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddresses_CustomerId_IsDefault",
                table: "customer_addresses");

            migrationBuilder.DropPrimaryKey(
                name: "pk_coupon_usages",
                table: "coupon_usages");

            migrationBuilder.DropPrimaryKey(
                name: "pk_categories_products",
                table: "categories_products");

            migrationBuilder.DropPrimaryKey(
                name: "pk_cancellation_policies",
                table: "cancellation_policies");

            migrationBuilder.DropPrimaryKey(
                name: "pk_admin_notifications",
                table: "admin_notifications");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "refunds",
                newName: "Refunds");

            migrationBuilder.RenameTable(
                name: "products",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "posts",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "payments",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "pages",
                newName: "Pages");

            migrationBuilder.RenameTable(
                name: "orders",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "notifications",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "customers",
                newName: "Customers");

            migrationBuilder.RenameTable(
                name: "coupons",
                newName: "Coupons");

            migrationBuilder.RenameTable(
                name: "contacts",
                newName: "Contacts");

            migrationBuilder.RenameTable(
                name: "categories",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "advertisements",
                newName: "Advertisements");

            migrationBuilder.RenameTable(
                name: "system_settings",
                newName: "SystemSettings");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "promotion_products",
                newName: "PromotionProducts");

            migrationBuilder.RenameTable(
                name: "promotion_campaigns",
                newName: "PromotionCampaigns");

            migrationBuilder.RenameTable(
                name: "product_variants",
                newName: "ProductVariants");

            migrationBuilder.RenameTable(
                name: "phone_blacklists",
                newName: "PhoneBlacklists");

            migrationBuilder.RenameTable(
                name: "payment_methods",
                newName: "PaymentMethods");

            migrationBuilder.RenameTable(
                name: "payment_attempts",
                newName: "PaymentAttempts");

            migrationBuilder.RenameTable(
                name: "order_details",
                newName: "OrderDetails");

            migrationBuilder.RenameTable(
                name: "flash_sales",
                newName: "FlashSales");

            migrationBuilder.RenameTable(
                name: "flash_sale_products",
                newName: "FlashSaleProducts");

            migrationBuilder.RenameTable(
                name: "email_histories",
                newName: "EmailHistories");

            migrationBuilder.RenameTable(
                name: "delivery_slots",
                newName: "DeliverySlots");

            migrationBuilder.RenameTable(
                name: "customer_payment_preferences",
                newName: "CustomerPaymentPreferences");

            migrationBuilder.RenameTable(
                name: "customer_addresses",
                newName: "CustomerAddresses");

            migrationBuilder.RenameTable(
                name: "coupon_usages",
                newName: "CouponUsages");

            migrationBuilder.RenameTable(
                name: "categories_products",
                newName: "CategoriesProducts");

            migrationBuilder.RenameTable(
                name: "cancellation_policies",
                newName: "CancellationPolicies");

            migrationBuilder.RenameTable(
                name: "admin_notifications",
                newName: "AdminNotifications");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Users",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "Users",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "last_login",
                table: "Users",
                newName: "LastLogin");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "Users",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "reason",
                table: "Refunds",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Refunds",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Refunds",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "requested_by",
                table: "Refunds",
                newName: "RequestedBy");

            migrationBuilder.RenameColumn(
                name: "refund_type",
                table: "Refunds",
                newName: "RefundType");

            migrationBuilder.RenameColumn(
                name: "refund_status",
                table: "Refunds",
                newName: "RefundStatus");

            migrationBuilder.RenameColumn(
                name: "refund_percent",
                table: "Refunds",
                newName: "RefundPercent");

            migrationBuilder.RenameColumn(
                name: "refund_amount",
                table: "Refunds",
                newName: "RefundAmount");

            migrationBuilder.RenameColumn(
                name: "processed_at",
                table: "Refunds",
                newName: "ProcessedAt");

            migrationBuilder.RenameColumn(
                name: "payment_id",
                table: "Refunds",
                newName: "PaymentId");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "Refunds",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "gateway_refund_id",
                table: "Refunds",
                newName: "GatewayRefundId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Refunds",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "approved_by",
                table: "Refunds",
                newName: "ApprovedBy");

            migrationBuilder.RenameIndex(
                name: "ix_refunds_payment_id",
                table: "Refunds",
                newName: "IX_Refunds_PaymentId");

            migrationBuilder.RenameColumn(
                name: "slug",
                table: "Products",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "sku",
                table: "Products",
                newName: "Sku");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Products",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "origin",
                table: "Products",
                newName: "Origin");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Products",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "view_count",
                table: "Products",
                newName: "ViewCount");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Products",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "stock_quantity",
                table: "Products",
                newName: "StockQuantity");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Products",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "image_url",
                table: "Products",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "flower_meaning",
                table: "Products",
                newName: "FlowerMeaning");

            migrationBuilder.RenameColumn(
                name: "discount_price",
                table: "Products",
                newName: "DiscountPrice");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Products",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "category_product_id",
                table: "Products",
                newName: "CategoryProductId");

            migrationBuilder.RenameColumn(
                name: "care_instruction",
                table: "Products",
                newName: "CareInstruction");

            migrationBuilder.RenameColumn(
                name: "add_to_cart_count",
                table: "Products",
                newName: "AddToCartCount");

            migrationBuilder.RenameIndex(
                name: "ix_products_category_product_id",
                table: "Products",
                newName: "IX_Products_CategoryProductId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Posts",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "summary",
                table: "Posts",
                newName: "Summary");

            migrationBuilder.RenameColumn(
                name: "slug",
                table: "Posts",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Posts",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Posts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Posts",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "image_url",
                table: "Posts",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "Posts",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "Posts",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "ix_posts_category_id",
                table: "Posts",
                newName: "IX_Posts_CategoryId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Payments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "Payments",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "method",
                table: "Payments",
                newName: "Method");

            migrationBuilder.RenameColumn(
                name: "gateway",
                table: "Payments",
                newName: "Gateway");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "Payments",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Payments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Payments",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "transaction_id",
                table: "Payments",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "refunded_by",
                table: "Payments",
                newName: "RefundedBy");

            migrationBuilder.RenameColumn(
                name: "refunded_at",
                table: "Payments",
                newName: "RefundedAt");

            migrationBuilder.RenameColumn(
                name: "refund_transaction_id",
                table: "Payments",
                newName: "RefundTransactionId");

            migrationBuilder.RenameColumn(
                name: "refund_response_code",
                table: "Payments",
                newName: "RefundResponseCode");

            migrationBuilder.RenameColumn(
                name: "refund_note",
                table: "Payments",
                newName: "RefundNote");

            migrationBuilder.RenameColumn(
                name: "payment_url",
                table: "Payments",
                newName: "PaymentUrl");

            migrationBuilder.RenameColumn(
                name: "payment_method_id",
                table: "Payments",
                newName: "PaymentMethodId");

            migrationBuilder.RenameColumn(
                name: "paid_at",
                table: "Payments",
                newName: "PaidAt");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "Payments",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "gateway_response_code",
                table: "Payments",
                newName: "GatewayResponseCode");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Payments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "bank_code",
                table: "Payments",
                newName: "BankCode");

            migrationBuilder.RenameIndex(
                name: "ix_payments_payment_method_id",
                table: "Payments",
                newName: "IX_Payments_PaymentMethodId");

            migrationBuilder.RenameIndex(
                name: "ix_payments_order_id",
                table: "Payments",
                newName: "IX_Payments_OrderId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Pages",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "slug",
                table: "Pages",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Pages",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Pages",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Pages",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Pages",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Pages",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Orders",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "Orders",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Orders",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "verified_at",
                table: "Orders",
                newName: "VerifiedAt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Orders",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "target_finished_time",
                table: "Orders",
                newName: "TargetFinishedTime");

            migrationBuilder.RenameColumn(
                name: "shipping_fee",
                table: "Orders",
                newName: "ShippingFee");

            migrationBuilder.RenameColumn(
                name: "refund_requested_at",
                table: "Orders",
                newName: "RefundRequestedAt");

            migrationBuilder.RenameColumn(
                name: "refund_completed_at",
                table: "Orders",
                newName: "RefundCompletedAt");

            migrationBuilder.RenameColumn(
                name: "refund_amount",
                table: "Orders",
                newName: "RefundAmount");

            migrationBuilder.RenameColumn(
                name: "recipient_phone",
                table: "Orders",
                newName: "RecipientPhone");

            migrationBuilder.RenameColumn(
                name: "recipient_name",
                table: "Orders",
                newName: "RecipientName");

            migrationBuilder.RenameColumn(
                name: "promotion_id",
                table: "Orders",
                newName: "PromotionId");

            migrationBuilder.RenameColumn(
                name: "payment_transaction_id",
                table: "Orders",
                newName: "PaymentTransactionId");

            migrationBuilder.RenameColumn(
                name: "payment_status",
                table: "Orders",
                newName: "PaymentStatus");

            migrationBuilder.RenameColumn(
                name: "payment_paid_at",
                table: "Orders",
                newName: "PaymentPaidAt");

            migrationBuilder.RenameColumn(
                name: "payment_method",
                table: "Orders",
                newName: "PaymentMethod");

            migrationBuilder.RenameColumn(
                name: "original_amount",
                table: "Orders",
                newName: "OriginalAmount");

            migrationBuilder.RenameColumn(
                name: "order_date",
                table: "Orders",
                newName: "OrderDate");

            migrationBuilder.RenameColumn(
                name: "is_verified",
                table: "Orders",
                newName: "IsVerified");

            migrationBuilder.RenameColumn(
                name: "final_amount",
                table: "Orders",
                newName: "FinalAmount");

            migrationBuilder.RenameColumn(
                name: "discount_amount",
                table: "Orders",
                newName: "DiscountAmount");

            migrationBuilder.RenameColumn(
                name: "delivery_ward",
                table: "Orders",
                newName: "DeliveryWard");

            migrationBuilder.RenameColumn(
                name: "delivery_time_slot",
                table: "Orders",
                newName: "DeliveryTimeSlot");

            migrationBuilder.RenameColumn(
                name: "delivery_slot_id",
                table: "Orders",
                newName: "DeliverySlotId");

            migrationBuilder.RenameColumn(
                name: "delivery_receiver_phone",
                table: "Orders",
                newName: "DeliveryReceiverPhone");

            migrationBuilder.RenameColumn(
                name: "delivery_receiver_name",
                table: "Orders",
                newName: "DeliveryReceiverName");

            migrationBuilder.RenameColumn(
                name: "delivery_province",
                table: "Orders",
                newName: "DeliveryProvince");

            migrationBuilder.RenameColumn(
                name: "delivery_postal_code",
                table: "Orders",
                newName: "DeliveryPostalCode");

            migrationBuilder.RenameColumn(
                name: "delivery_district",
                table: "Orders",
                newName: "DeliveryDistrict");

            migrationBuilder.RenameColumn(
                name: "delivery_date",
                table: "Orders",
                newName: "DeliveryDate");

            migrationBuilder.RenameColumn(
                name: "delivery_address_line",
                table: "Orders",
                newName: "DeliveryAddressLine");

            migrationBuilder.RenameColumn(
                name: "delivery_address",
                table: "Orders",
                newName: "DeliveryAddress");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "Orders",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Orders",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "coupon_id",
                table: "Orders",
                newName: "CouponId");

            migrationBuilder.RenameColumn(
                name: "cancelled_by",
                table: "Orders",
                newName: "CancelledBy");

            migrationBuilder.RenameColumn(
                name: "cancelled_at",
                table: "Orders",
                newName: "CancelledAt");

            migrationBuilder.RenameColumn(
                name: "cancellation_reason",
                table: "Orders",
                newName: "CancellationReason");

            migrationBuilder.RenameColumn(
                name: "cancellation_fee",
                table: "Orders",
                newName: "CancellationFee");

            migrationBuilder.RenameIndex(
                name: "ix_orders_promotion_id",
                table: "Orders",
                newName: "IX_Orders_PromotionId");

            migrationBuilder.RenameIndex(
                name: "ix_orders_customer_id",
                table: "Orders",
                newName: "IX_Orders_CustomerId");

            migrationBuilder.RenameIndex(
                name: "ix_orders_coupon_id",
                table: "Orders",
                newName: "IX_Orders_CouponId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Notifications",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Notifications",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "Notifications",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "metadata",
                table: "Notifications",
                newName: "Metadata");

            migrationBuilder.RenameColumn(
                name: "icon",
                table: "Notifications",
                newName: "Icon");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Notifications",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Notifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "reference_type",
                table: "Notifications",
                newName: "ReferenceType");

            migrationBuilder.RenameColumn(
                name: "read_at",
                table: "Notifications",
                newName: "ReadAt");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "Notifications",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "navigation_url",
                table: "Notifications",
                newName: "NavigationUrl");

            migrationBuilder.RenameColumn(
                name: "is_read",
                table: "Notifications",
                newName: "IsRead");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "Notifications",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Notifications",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Customers",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Customers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "Customers",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Customers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Customers",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "total_orders",
                table: "Customers",
                newName: "TotalOrders");

            migrationBuilder.RenameColumn(
                name: "successful_deliveries",
                table: "Customers",
                newName: "SuccessfulDeliveries");

            migrationBuilder.RenameColumn(
                name: "reset_token_expiry",
                table: "Customers",
                newName: "ResetTokenExpiry");

            migrationBuilder.RenameColumn(
                name: "reset_token",
                table: "Customers",
                newName: "ResetToken");

            migrationBuilder.RenameColumn(
                name: "phone_verified",
                table: "Customers",
                newName: "PhoneVerified");

            migrationBuilder.RenameColumn(
                name: "last_login",
                table: "Customers",
                newName: "LastLogin");

            migrationBuilder.RenameColumn(
                name: "is_blacklisted",
                table: "Customers",
                newName: "IsBlacklisted");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Customers",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "Customers",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "fraud_score",
                table: "Customers",
                newName: "FraudScore");

            migrationBuilder.RenameColumn(
                name: "failed_deliveries",
                table: "Customers",
                newName: "FailedDeliveries");

            migrationBuilder.RenameColumn(
                name: "email_verified",
                table: "Customers",
                newName: "EmailVerified");

            migrationBuilder.RenameColumn(
                name: "default_address_id",
                table: "Customers",
                newName: "DefaultAddressId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Customers",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_customers_email",
                table: "Customers",
                newName: "IX_Customers_Email");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Coupons",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "Coupons",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Coupons",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "used_count",
                table: "Coupons",
                newName: "UsedCount");

            migrationBuilder.RenameColumn(
                name: "usage_per_customer",
                table: "Coupons",
                newName: "UsagePerCustomer");

            migrationBuilder.RenameColumn(
                name: "usage_limit",
                table: "Coupons",
                newName: "UsageLimit");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Coupons",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "Coupons",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "minimum_order_amount",
                table: "Coupons",
                newName: "MinimumOrderAmount");

            migrationBuilder.RenameColumn(
                name: "maximum_discount_amount",
                table: "Coupons",
                newName: "MaximumDiscountAmount");

            migrationBuilder.RenameColumn(
                name: "is_public",
                table: "Coupons",
                newName: "IsPublic");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Coupons",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "Coupons",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "discount_value",
                table: "Coupons",
                newName: "DiscountValue");

            migrationBuilder.RenameColumn(
                name: "discount_type",
                table: "Coupons",
                newName: "DiscountType");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "Coupons",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Coupons",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "subject",
                table: "Contacts",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Contacts",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Contacts",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "Contacts",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Contacts",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Contacts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "read_at",
                table: "Contacts",
                newName: "ReadAt");

            migrationBuilder.RenameColumn(
                name: "is_read",
                table: "Contacts",
                newName: "IsRead");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Contacts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "slug",
                table: "Categories",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Categories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Categories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Categories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Categories",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Categories",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Advertisements",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "subtitle",
                table: "Advertisements",
                newName: "Subtitle");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Advertisements",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Advertisements",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "sort_order",
                table: "Advertisements",
                newName: "SortOrder");

            migrationBuilder.RenameColumn(
                name: "link_url",
                table: "Advertisements",
                newName: "LinkUrl");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Advertisements",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "image_url",
                table: "Advertisements",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Advertisements",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "SystemSettings",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "SystemSettings",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "key",
                table: "SystemSettings",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "SystemSettings",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "SystemSettings",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RefreshTokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "RefreshTokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "RefreshTokens",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "token_hash",
                table: "RefreshTokens",
                newName: "TokenHash");

            migrationBuilder.RenameColumn(
                name: "revoked_at",
                table: "RefreshTokens",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "is_revoked",
                table: "RefreshTokens",
                newName: "IsRevoked");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "RefreshTokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "device_info",
                table: "RefreshTokens",
                newName: "DeviceInfo");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "RefreshTokens",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_user_id",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PromotionProducts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "promotion_id",
                table: "PromotionProducts",
                newName: "PromotionId");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "PromotionProducts",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "PromotionProducts",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_promotion_products_product_id",
                table: "PromotionProducts",
                newName: "IX_PromotionProducts_ProductId");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "PromotionCampaigns",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "PromotionCampaigns",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "PromotionCampaigns",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PromotionCampaigns",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "PromotionCampaigns",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "PromotionCampaigns",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "promotion_type",
                table: "PromotionCampaigns",
                newName: "PromotionType");

            migrationBuilder.RenameColumn(
                name: "is_stackable",
                table: "PromotionCampaigns",
                newName: "IsStackable");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "PromotionCampaigns",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "PromotionCampaigns",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "discount_value",
                table: "PromotionCampaigns",
                newName: "DiscountValue");

            migrationBuilder.RenameColumn(
                name: "discount_type",
                table: "PromotionCampaigns",
                newName: "DiscountType");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "PromotionCampaigns",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "banner_image",
                table: "PromotionCampaigns",
                newName: "BannerImage");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "ProductVariants",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ProductVariants",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "product_id1",
                table: "ProductVariants",
                newName: "ProductId1");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "ProductVariants",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "price_adjustment",
                table: "ProductVariants",
                newName: "PriceAdjustment");

            migrationBuilder.RenameColumn(
                name: "is_default",
                table: "ProductVariants",
                newName: "IsDefault");

            migrationBuilder.RenameIndex(
                name: "ix_product_variants_product_id1",
                table: "ProductVariants",
                newName: "IX_ProductVariants_ProductId1");

            migrationBuilder.RenameIndex(
                name: "ix_product_variants_product_id",
                table: "ProductVariants",
                newName: "IX_ProductVariants_ProductId");

            migrationBuilder.RenameColumn(
                name: "reason",
                table: "PhoneBlacklists",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PhoneBlacklists",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "PhoneBlacklists",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "PhoneBlacklists",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "PhoneBlacklists",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "PaymentMethods",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "PaymentMethods",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "PaymentMethods",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PaymentMethods",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "PaymentMethods",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_online",
                table: "PaymentMethods",
                newName: "IsOnline");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "PaymentMethods",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "display_order",
                table: "PaymentMethods",
                newName: "DisplayOrder");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "PaymentMethods",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_payment_methods_code",
                table: "PaymentMethods",
                newName: "IX_PaymentMethods_Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PaymentAttempts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_agent",
                table: "PaymentAttempts",
                newName: "UserAgent");

            migrationBuilder.RenameColumn(
                name: "payment_id",
                table: "PaymentAttempts",
                newName: "PaymentId");

            migrationBuilder.RenameColumn(
                name: "ip_address",
                table: "PaymentAttempts",
                newName: "IpAddress");

            migrationBuilder.RenameColumn(
                name: "gateway_response",
                table: "PaymentAttempts",
                newName: "GatewayResponse");

            migrationBuilder.RenameColumn(
                name: "gateway_request",
                table: "PaymentAttempts",
                newName: "GatewayRequest");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "PaymentAttempts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "attempt_number",
                table: "PaymentAttempts",
                newName: "AttemptNumber");

            migrationBuilder.RenameIndex(
                name: "ix_payment_attempts_payment_id_attempt_number",
                table: "PaymentAttempts",
                newName: "IX_PaymentAttempts_PaymentId_AttemptNumber");

            migrationBuilder.RenameColumn(
                name: "subtotal",
                table: "OrderDetails",
                newName: "Subtotal");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "OrderDetails",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "discount",
                table: "OrderDetails",
                newName: "Discount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "OrderDetails",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "unit_price",
                table: "OrderDetails",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "size_variant",
                table: "OrderDetails",
                newName: "SizeVariant");

            migrationBuilder.RenameColumn(
                name: "product_name",
                table: "OrderDetails",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "product_image",
                table: "OrderDetails",
                newName: "ProductImage");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "OrderDetails",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "OrderDetails",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "ix_order_details_product_id",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ProductId");

            migrationBuilder.RenameIndex(
                name: "ix_order_details_order_id",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrderId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "FlashSales",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "FlashSales",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FlashSales",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "FlashSales",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "FlashSales",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "FlashSales",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "FlashSales",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "FlashSales",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FlashSaleProducts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "sale_price",
                table: "FlashSaleProducts",
                newName: "SalePrice");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "FlashSaleProducts",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "flash_sale_id",
                table: "FlashSaleProducts",
                newName: "FlashSaleId");

            migrationBuilder.RenameColumn(
                name: "discount_percent",
                table: "FlashSaleProducts",
                newName: "DiscountPercent");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "FlashSaleProducts",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_flash_sale_products_product_id",
                table: "FlashSaleProducts",
                newName: "IX_FlashSaleProducts_ProductId");

            migrationBuilder.RenameIndex(
                name: "ix_flash_sale_products_flash_sale_id",
                table: "FlashSaleProducts",
                newName: "IX_FlashSaleProducts_FlashSaleId");

            migrationBuilder.RenameColumn(
                name: "subject",
                table: "EmailHistories",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "EmailHistories",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "recipient",
                table: "EmailHistories",
                newName: "Recipient");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "EmailHistories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "sent_at",
                table: "EmailHistories",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "EmailHistories",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "email_type",
                table: "EmailHistories",
                newName: "EmailType");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "EmailHistories",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "EmailHistories",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_email_histories_customer_id",
                table: "EmailHistories",
                newName: "IX_EmailHistories_CustomerId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "DeliverySlots",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "DeliverySlots",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "time_slot",
                table: "DeliverySlots",
                newName: "TimeSlot");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "DeliverySlots",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "max_capacity",
                table: "DeliverySlots",
                newName: "MaxCapacity");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "DeliverySlots",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "delivery_date",
                table: "DeliverySlots",
                newName: "DeliveryDate");

            migrationBuilder.RenameColumn(
                name: "current_booked",
                table: "DeliverySlots",
                newName: "CurrentBooked");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "DeliverySlots",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CustomerPaymentPreferences",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "CustomerPaymentPreferences",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "payment_method_id",
                table: "CustomerPaymentPreferences",
                newName: "PaymentMethodId");

            migrationBuilder.RenameColumn(
                name: "last_used_at",
                table: "CustomerPaymentPreferences",
                newName: "LastUsedAt");

            migrationBuilder.RenameColumn(
                name: "is_default",
                table: "CustomerPaymentPreferences",
                newName: "IsDefault");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "CustomerPaymentPreferences",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "CustomerPaymentPreferences",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_customer_payment_preferences_payment_method_id",
                table: "CustomerPaymentPreferences",
                newName: "IX_CustomerPaymentPreferences_PaymentMethodId");

            migrationBuilder.RenameIndex(
                name: "ix_customer_payment_preferences_customer_id_payment_method_id",
                table: "CustomerPaymentPreferences",
                newName: "IX_CustomerPaymentPreferences_CustomerId_PaymentMethodId");

            migrationBuilder.RenameColumn(
                name: "ward",
                table: "CustomerAddresses",
                newName: "Ward");

            migrationBuilder.RenameColumn(
                name: "province",
                table: "CustomerAddresses",
                newName: "Province");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "CustomerAddresses",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "CustomerAddresses",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "CustomerAddresses",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "district",
                table: "CustomerAddresses",
                newName: "District");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CustomerAddresses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "CustomerAddresses",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "receiver_phone",
                table: "CustomerAddresses",
                newName: "ReceiverPhone");

            migrationBuilder.RenameColumn(
                name: "receiver_name",
                table: "CustomerAddresses",
                newName: "ReceiverName");

            migrationBuilder.RenameColumn(
                name: "postal_code",
                table: "CustomerAddresses",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "is_default",
                table: "CustomerAddresses",
                newName: "IsDefault");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "CustomerAddresses",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "CustomerAddresses",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "CustomerAddresses",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "address_line",
                table: "CustomerAddresses",
                newName: "AddressLine");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CouponUsages",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "used_at",
                table: "CouponUsages",
                newName: "UsedAt");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "CouponUsages",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "discount_amount",
                table: "CouponUsages",
                newName: "DiscountAmount");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "CouponUsages",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "coupon_id",
                table: "CouponUsages",
                newName: "CouponId");

            migrationBuilder.RenameIndex(
                name: "ix_coupon_usages_customer_id",
                table: "CouponUsages",
                newName: "IX_CouponUsages_CustomerId");

            migrationBuilder.RenameColumn(
                name: "slug",
                table: "CategoriesProducts",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "CategoriesProducts",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "CategoriesProducts",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CategoriesProducts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "CategoriesProducts",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "CategoriesProducts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "CancellationPolicies",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CancellationPolicies",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "CancellationPolicies",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "refund_percent",
                table: "CancellationPolicies",
                newName: "RefundPercent");

            migrationBuilder.RenameColumn(
                name: "order_status",
                table: "CancellationPolicies",
                newName: "OrderStatus");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "CancellationPolicies",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "CancellationPolicies",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "cancellation_fee_percent",
                table: "CancellationPolicies",
                newName: "CancellationFeePercent");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "AdminNotifications",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "AdminNotifications",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "AdminNotifications",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "metadata",
                table: "AdminNotifications",
                newName: "Metadata");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "AdminNotifications",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "icon",
                table: "AdminNotifications",
                newName: "Icon");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AdminNotifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AdminNotifications",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "reference_type",
                table: "AdminNotifications",
                newName: "ReferenceType");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "AdminNotifications",
                newName: "ReferenceId");

            migrationBuilder.RenameColumn(
                name: "read_at",
                table: "AdminNotifications",
                newName: "ReadAt");

            migrationBuilder.RenameColumn(
                name: "navigation_url",
                table: "AdminNotifications",
                newName: "NavigationUrl");

            migrationBuilder.RenameColumn(
                name: "is_read",
                table: "AdminNotifications",
                newName: "IsRead");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "AdminNotifications",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "AdminNotifications",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Refunds",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Refunds",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Refunds",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequestedBy",
                table: "Refunds",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RefundType",
                table: "Refunds",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RefundStatus",
                table: "Refunds",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RefundPercent",
                table: "Refunds",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedAt",
                table: "Refunds",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "Refunds",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Refunds",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "GatewayRefundId",
                table: "Refunds",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Refunds",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "Refunds",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Products",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "Products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Origin",
                table: "Products",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ViewCount",
                table: "Products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Products",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StockQuantity",
                table: "Products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FlowerMeaning",
                table: "Products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPrice",
                table: "Products",
                type: "numeric(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryProductId",
                table: "Products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CareInstruction",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AddToCartCount",
                table: "Products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Posts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "Posts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Posts",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Posts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Posts",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Posts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Posts",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Posts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Payments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Payments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Method",
                table: "Payments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Gateway",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Payments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Payments",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RefundedBy",
                table: "Payments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefundedAt",
                table: "Payments",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RefundTransactionId",
                table: "Payments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RefundResponseCode",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RefundNote",
                table: "Payments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentUrl",
                table: "Payments",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethodId",
                table: "Payments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidAt",
                table: "Payments",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Payments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "GatewayResponseCode",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Payments",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "BankCode",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Pages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Pages",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Pages",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Pages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Pages",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Pages",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Pages",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Orders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VerifiedAt",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TargetFinishedTime",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefundRequestedAt",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefundCompletedAt",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RefundAmount",
                table: "Orders",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "RecipientPhone",
                table: "Orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientName",
                table: "Orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PromotionId",
                table: "Orders",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentTransactionId",
                table: "Orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentStatus",
                table: "Orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentPaidAt",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethod",
                table: "Orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerified",
                table: "Orders",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryWard",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryTimeSlot",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeliverySlotId",
                table: "Orders",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryReceiverPhone",
                table: "Orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryReceiverName",
                table: "Orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryProvince",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryPostalCode",
                table: "Orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryDistrict",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDate",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryAddressLine",
                table: "Orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryAddress",
                table: "Orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "CouponId",
                table: "Orders",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CancelledBy",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CancelledAt",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CancellationReason",
                table: "Orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "Notifications",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Metadata",
                table: "Notifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Notifications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Notifications",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Notifications",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceType",
                table: "Notifications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReadAt",
                table: "Notifications",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Notifications",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NavigationUrl",
                table: "Notifications",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Notifications",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Notifications",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Customers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Customers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Customers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Customers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Customers",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalOrders",
                table: "Customers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SuccessfulDeliveries",
                table: "Customers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResetTokenExpiry",
                table: "Customers",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResetToken",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "PhoneVerified",
                table: "Customers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLogin",
                table: "Customers",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsBlacklisted",
                table: "Customers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Customers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Customers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "FraudScore",
                table: "Customers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FailedDeliveries",
                table: "Customers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailVerified",
                table: "Customers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "DefaultAddressId",
                table: "Customers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Customers",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Coupons",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Coupons",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Coupons",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "UsedCount",
                table: "Coupons",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UsagePerCustomer",
                table: "Coupons",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UsageLimit",
                table: "Coupons",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Coupons",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Coupons",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublic",
                table: "Coupons",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Coupons",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Coupons",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiscountType",
                table: "Coupons",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Coupons",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Coupons",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "Contacts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Contacts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Contacts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Contacts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contacts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Contacts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReadAt",
                table: "Contacts",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Contacts",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Contacts",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Categories",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Categories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Categories",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Advertisements",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Subtitle",
                table: "Advertisements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Advertisements",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Advertisements",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                table: "Advertisements",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LinkUrl",
                table: "Advertisements",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Advertisements",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Advertisements",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Advertisements",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "SystemSettings",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SystemSettings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "SystemSettings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "SystemSettings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "SystemSettings",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "RefreshTokens",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "RefreshTokens",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "RefreshTokens",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TokenHash",
                table: "RefreshTokens",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RevokedAt",
                table: "RefreshTokens",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRevoked",
                table: "RefreshTokens",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresAt",
                table: "RefreshTokens",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceInfo",
                table: "RefreshTokens",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RefreshTokens",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PromotionProducts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "PromotionId",
                table: "PromotionProducts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "PromotionProducts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PromotionProducts",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "PromotionCampaigns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PromotionCampaigns",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PromotionCampaigns",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PromotionCampaigns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "PromotionCampaigns",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "PromotionCampaigns",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "PromotionType",
                table: "PromotionCampaigns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsStackable",
                table: "PromotionCampaigns",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "PromotionCampaigns",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "PromotionCampaigns",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountType",
                table: "PromotionCampaigns",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PromotionCampaigns",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "BannerImage",
                table: "PromotionCampaigns",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductVariants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ProductVariants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId1",
                table: "ProductVariants",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductVariants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault",
                table: "ProductVariants",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "PhoneBlacklists",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PhoneBlacklists",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "PhoneBlacklists",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "PhoneBlacklists",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PhoneBlacklists",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PaymentMethods",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PaymentMethods",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "PaymentMethods",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PaymentMethods",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "PaymentMethods",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsOnline",
                table: "PaymentMethods",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "PaymentMethods",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                table: "PaymentMethods",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PaymentMethods",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PaymentAttempts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "UserAgent",
                table: "PaymentAttempts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "PaymentAttempts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "PaymentAttempts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GatewayResponse",
                table: "PaymentAttempts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GatewayRequest",
                table: "PaymentAttempts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PaymentAttempts",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "AttemptNumber",
                table: "PaymentAttempts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "OrderDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "OrderDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "SizeVariant",
                table: "OrderDetails",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "OrderDetails",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductImage",
                table: "OrderDetails",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "OrderDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "OrderDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FlashSales",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FlashSales",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "FlashSales",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "FlashSales",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "FlashSales",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "FlashSales",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "FlashSales",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "FlashSales",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "FlashSaleProducts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "FlashSaleProducts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FlashSaleId",
                table: "FlashSaleProducts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "FlashSaleProducts",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "EmailHistories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "EmailHistories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Recipient",
                table: "EmailHistories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "EmailHistories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentAt",
                table: "EmailHistories",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "EmailHistories",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailType",
                table: "EmailHistories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "EmailHistories",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "EmailHistories",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DeliverySlots",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "DeliverySlots",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TimeSlot",
                table: "DeliverySlots",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "DeliverySlots",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MaxCapacity",
                table: "DeliverySlots",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "DeliverySlots",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDate",
                table: "DeliverySlots",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentBooked",
                table: "DeliverySlots",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "DeliverySlots",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CustomerPaymentPreferences",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "CustomerPaymentPreferences",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethodId",
                table: "CustomerPaymentPreferences",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUsedAt",
                table: "CustomerPaymentPreferences",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault",
                table: "CustomerPaymentPreferences",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CustomerPaymentPreferences",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CustomerPaymentPreferences",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Ward",
                table: "CustomerAddresses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Province",
                table: "CustomerAddresses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "CustomerAddresses",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "CustomerAddresses",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "CustomerAddresses",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "CustomerAddresses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CustomerAddresses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "CustomerAddresses",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverPhone",
                table: "CustomerAddresses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverName",
                table: "CustomerAddresses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "CustomerAddresses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault",
                table: "CustomerAddresses",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CustomerAddresses",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CustomerAddresses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CustomerAddresses",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine",
                table: "CustomerAddresses",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CouponUsages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UsedAt",
                table: "CouponUsages",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "CouponUsages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CouponUsages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CouponId",
                table: "CouponUsages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "CategoriesProducts",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CategoriesProducts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CategoriesProducts",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CategoriesProducts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "CategoriesProducts",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CategoriesProducts",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CancellationPolicies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CancellationPolicies",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "CancellationPolicies",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RefundPercent",
                table: "CancellationPolicies",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "OrderStatus",
                table: "CancellationPolicies",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CancellationPolicies",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CancellationPolicies",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "CancellationFeePercent",
                table: "CancellationPolicies",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "AdminNotifications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "AdminNotifications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "AdminNotifications",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Metadata",
                table: "AdminNotifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "AdminNotifications",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "AdminNotifications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AdminNotifications",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "AdminNotifications",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceType",
                table: "AdminNotifications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceId",
                table: "AdminNotifications",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReadAt",
                table: "AdminNotifications",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NavigationUrl",
                table: "AdminNotifications",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "AdminNotifications",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "AdminNotifications",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AdminNotifications",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Refunds",
                table: "Refunds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pages",
                table: "Pages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Coupons",
                table: "Coupons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Advertisements",
                table: "Advertisements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemSettings",
                table: "SystemSettings",
                column: "Key");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromotionProducts",
                table: "PromotionProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromotionCampaigns",
                table: "PromotionCampaigns",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductVariants",
                table: "ProductVariants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhoneBlacklists",
                table: "PhoneBlacklists",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentMethods",
                table: "PaymentMethods",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentAttempts",
                table: "PaymentAttempts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDetails",
                table: "OrderDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlashSales",
                table: "FlashSales",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlashSaleProducts",
                table: "FlashSaleProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailHistories",
                table: "EmailHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliverySlots",
                table: "DeliverySlots",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerPaymentPreferences",
                table: "CustomerPaymentPreferences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerAddresses",
                table: "CustomerAddresses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CouponUsages",
                table: "CouponUsages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoriesProducts",
                table: "CategoriesProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CancellationPolicies",
                table: "CancellationPolicies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdminNotifications",
                table: "AdminNotifications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku",
                unique: true,
                filter: "\"Sku\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status")
                .Annotation("SqlServer:Include", new[] { "OrderDate", "PaymentMethod" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ResetToken",
                table: "Customers",
                column: "ResetToken",
                filter: "\"ResetToken\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_CustomerId_IsDefault",
                table: "CustomerAddresses",
                columns: new[] { "CustomerId", "IsDefault" },
                filter: "\"IsDefault\" = true");

            migrationBuilder.AddForeignKey(
                name: "FK_CouponUsages_Coupons_CouponId",
                table: "CouponUsages",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CouponUsages_Customers_CustomerId",
                table: "CouponUsages",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CouponUsages_Orders_OrderId",
                table: "CouponUsages",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddresses_Customers_CustomerId",
                table: "CustomerAddresses",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPaymentPreferences_Customers_CustomerId",
                table: "CustomerPaymentPreferences",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPaymentPreferences_PaymentMethods_PaymentMethodId",
                table: "CustomerPaymentPreferences",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliverySlots_Products_ProductId",
                table: "DeliverySlots",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailHistories_Customers_CustomerId",
                table: "EmailHistories",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlashSaleProducts_FlashSales_FlashSaleId",
                table: "FlashSaleProducts",
                column: "FlashSaleId",
                principalTable: "FlashSales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlashSaleProducts_Products_ProductId",
                table: "FlashSaleProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Customers_CustomerId",
                table: "Notifications",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Coupons_CouponId",
                table: "Orders",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PromotionCampaigns_PromotionId",
                table: "Orders",
                column: "PromotionId",
                principalTable: "PromotionCampaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentAttempts_Payments_PaymentId",
                table: "PaymentAttempts",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentMethods_PaymentMethodId",
                table: "Payments",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                table: "Posts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CategoriesProducts_CategoryProductId",
                table: "Products",
                column: "CategoryProductId",
                principalTable: "CategoriesProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Products_ProductId",
                table: "ProductVariants",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Products_ProductId1",
                table: "ProductVariants",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionProducts_Products_ProductId",
                table: "PromotionProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionProducts_PromotionCampaigns_PromotionId",
                table: "PromotionProducts",
                column: "PromotionId",
                principalTable: "PromotionCampaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Orders_OrderId",
                table: "Refunds",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
