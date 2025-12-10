using BackEnd.Data;
using BackEnd.Repositories;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

// ===================================================================
// 应用程序入口点配置
// ===================================================================

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ===================================================================
// 1. 基础服务配置
// ===================================================================

// 数据库上下文
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(configuration.GetConnectionString("DefaultConnection"))
           .LogTo(Console.WriteLine, LogLevel.Information));

// API 文档
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 控制器和 JSON 序列化
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// ===================================================================
// 2. 身份验证和授权配置
// ===================================================================

// JWT 身份验证
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = configuration["Jwt:Key"] 
        ?? throw new InvalidOperationException("JWT密钥 'Jwt:Key' 未在配置中设置");
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// 授权服务
builder.Services.AddAuthorization();

// ===================================================================
// 3. 跨域和上下文访问配置
// ===================================================================

// CORS 配置
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// HttpContext 访问器
builder.Services.AddHttpContextAccessor();

// ===================================================================
// 4. Repository 层依赖注入（按字母顺序）
// ===================================================================

builder.Services.AddScoped<IAdministratorRepository, AdministratorRepository>();
builder.Services.AddScoped<IAfterSaleApplicationRepository, AfterSaleApplicationRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICouponManagerRepository, CouponManagerRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IDeliveryComplaintRepository, DeliveryComplaintRepository>();
builder.Services.AddScoped<IDeliveryTaskRepository, DeliveryTaskRepository>();
builder.Services.AddScoped<IDishCategoryRepository, DishCategoryRepository>();
builder.Services.AddScoped<IDishRepository, DishRepository>();
builder.Services.AddScoped<IEvaluate_AfterSaleRepository, Evaluate_AfterSaleRepository>();
builder.Services.AddScoped<IEvaluate_ComplaintRepository, Evaluate_ComplaintRepository>();
builder.Services.AddScoped<IFavoriteItemRepository, FavoriteItemRepository>();
builder.Services.AddScoped<IFavoritesFolderRepository, FavoritesFolderRepository>();
builder.Services.AddScoped<IFoodOrderRepository, FoodOrderRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMerchantRepository, MerchantRepository>();
builder.Services.AddScoped<IReview_CommentRepository, Review_CommentRepository>();
builder.Services.AddScoped<ISellerRepository, SellerRepository>();
builder.Services.AddScoped<IShoppingCartItemRepository, ShoppingCartItemRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IStoreViolationPenaltyRepository, StoreViolationPenaltyRepository>();
builder.Services.AddScoped<ISupervise_Repository, Supervise_Repository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ===================================================================
// 5. Service 层依赖注入（按功能模块分类）
// ===================================================================

// 5.1 基础服务（用户信息管理）
builder.Services.AddScoped<IAdminInfoService, AdminInfoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICourierInfoService, CourierInfoService>();
builder.Services.AddScoped<ICustomerInfoService, CustomerInfoService>();
builder.Services.AddScoped<IMerchantInfoService, MerchantInfoService>();

// 5.2 售后申请服务（按角色分类）
builder.Services.AddScoped<IAdminAfterSaleService, AdminAfterSaleService>();
builder.Services.AddScoped<ICustomerAfterSaleService, CustomerAfterSaleService>();
builder.Services.AddScoped<IMerchantAfterSaleService, MerchantAfterSaleService>();

// 5.3 评论服务（按角色分类）
builder.Services.AddScoped<IAdminCommentService, AdminCommentService>();
builder.Services.AddScoped<ICustomerCommentService, CustomerCommentService>();
builder.Services.AddScoped<IMerchantCommentService, MerchantCommentService>();

// 5.4 店铺举报服务（按角色分类）
builder.Services.AddScoped<IAdminPenaltyService, AdminPenaltyService>();
builder.Services.AddScoped<ICustomerStoreReportService, CustomerStoreReportService>();
builder.Services.AddScoped<IMerchantPenaltyService, MerchantPenaltyService>();

// 5.5 配送投诉服务（按角色分类）
builder.Services.AddScoped<IAdminDeliveryComplaintService, AdminDeliveryComplaintService>();
builder.Services.AddScoped<ICourierDeliveryComplaintService, CourierDeliveryComplaintService>();
builder.Services.AddScoped<ICustomerDeliveryComplaintService, CustomerDeliveryComplaintService>();

// 5.6 订单服务（按角色分类）
builder.Services.AddScoped<ICustomerOrderService, CustomerOrderService>();
builder.Services.AddScoped<IMerchantOrderService, MerchantOrderService>();

// 5.7 菜品服务（按角色分类）
builder.Services.AddScoped<ICustomerDishService, CustomerDishService>();
builder.Services.AddScoped<IMerchantDishService, MerchantDishService>();

// 5.8 优惠券服务（按角色分类）
builder.Services.AddScoped<ICustomerCouponService, CustomerCouponService>();
builder.Services.AddScoped<IMerchantCouponService, MerchantCouponService>();

// 5.9 店铺服务（按角色分类）
builder.Services.AddScoped<ICustomerStoreService, CustomerStoreService>();
builder.Services.AddScoped<IMerchantStoreService, MerchantStoreService>();

// 5.10 配送任务服务（按角色分类）
builder.Services.AddScoped<ICourierDeliveryTaskService, CourierDeliveryTaskService>();
builder.Services.AddScoped<IMerchantDeliveryTaskService, MerchantDeliveryTaskService>();

// 5.11 其他业务服务
builder.Services.AddScoped<ICourierRatingService, CourierRatingService>();
builder.Services.AddScoped<IDishCategoryService, DishCategoryService>();
builder.Services.AddScoped<IGeoHelper, GeoHelper>();
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();
builder.Services.AddScoped<IMenuService, MenuService>();

// ===================================================================
// 6. 后台任务服务
// ===================================================================

builder.Services.AddHostedService<MonthlyCommissionResetService>();
builder.Services.AddHostedService<MonthlySalesUpdateService>();

// ===================================================================
// 7. 构建应用程序
// ===================================================================

var app = builder.Build();

// ===================================================================
// 8. 中间件配置（按执行顺序）
// ===================================================================

// 8.1 开发环境工具
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 8.2 静态文件服务配置
ConfigureStaticFiles(app, builder.Environment.ContentRootPath);

// 8.3 跨域
app.UseCors("AllowAll");

// 8.4 身份验证和授权（顺序重要：先认证后授权）
app.UseAuthentication();
app.UseAuthorization();

// 8.5 路由映射
app.MapControllers();

// ===================================================================
// 9. 启动应用程序
// ===================================================================

app.Run();

// ===================================================================
// 辅助方法
// ===================================================================

/// <summary>
/// 配置静态文件服务
/// </summary>
static void ConfigureStaticFiles(WebApplication app, string contentRootPath)
{
    // 根目录静态文件（wwwroot）
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
            Path.Combine(contentRootPath, "wwwroot")),
        RequestPath = ""
    });

    // 头像文件服务
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
            Path.Combine(contentRootPath, "wwwroot", "avatars")),
        RequestPath = "/avatars"
    });

    // 店铺图片文件服务
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
            Path.Combine(contentRootPath, "wwwroot", "images", "stores")),
        RequestPath = "/images/stores"
    });

    // 菜品图片文件服务
    var dishesImagesPath = Path.Combine(contentRootPath, "wwwroot", "images", "dishes");
    if (!Directory.Exists(dishesImagesPath))
    {
        Directory.CreateDirectory(dishesImagesPath);
    }
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(dishesImagesPath),
        RequestPath = "/images/dishes"
    });

    // 通用上传图片文件服务
    var uploadsImagesPath = Path.Combine(contentRootPath, "wwwroot", "images", "uploads");
    if (!Directory.Exists(uploadsImagesPath))
    {
        Directory.CreateDirectory(uploadsImagesPath);
    }
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsImagesPath),
        RequestPath = "/images/uploads"
    });
}
