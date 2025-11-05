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

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration; // 应用程序所有配置信息的集合

// Kestrel配置已移除，使用默认配置

// 数据库上下文注册
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information)); // 添加SQL日志

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 启用 MVC 控制器支持
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // 配置 JSON 序列化为驼峰命名（小驼峰）
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // 验证用于签名 Token 的密钥
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT密钥 'Jwt:Key' 未在配置中设置"))),

        // 不验证发行人 (Issuer)
        ValidateIssuer = false,

        // 不验证接收方 (Audience)
        ValidateAudience = false,

        // 验证Token的生命周期
        ValidateLifetime = true,

        // 允许的服务器时间偏移量，设置为零表示不容忍任何时间误差
        ClockSkew = TimeSpan.Zero
    };
});

// 添加授权服务
builder.Services.AddAuthorization();

// 添加 HttpContextAccessor 支持
builder.Services.AddHttpContextAccessor();

// 添加 CORS 支持
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 注册 Repository 层
// Repository 层注入，接口在前，实现类在后
builder.Services.AddScoped<IAdministratorRepository, AdministratorRepository>();
builder.Services.AddScoped<IAfterSaleApplicationRepository, AfterSaleApplicationRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICouponManagerRepository, CouponManagerRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IDeliveryComplaintRepository, DeliveryComplaintRepository>();
builder.Services.AddScoped<IDeliveryTaskRepository, DeliveryTaskRepository>();
builder.Services.AddScoped<IDishRepository, DishRepository>();
builder.Services.AddScoped<IEvaluate_AfterSaleRepository, Evaluate_AfterSaleRepository>();
builder.Services.AddScoped<IEvaluate_ComplaintRepository, Evaluate_ComplaintRepository>();
builder.Services.AddScoped<IFavoriteItemRepository, FavoriteItemRepository>();
builder.Services.AddScoped<IFavoritesFolderRepository, FavoritesFolderRepository>();
builder.Services.AddScoped<IFoodOrderRepository, FoodOrderRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IDishCategoryRepository, DishCategoryRepository>();
builder.Services.AddScoped<IReview_CommentRepository, Review_CommentRepository>();
builder.Services.AddScoped<ISellerRepository, SellerRepository>();
builder.Services.AddScoped<IShoppingCartItemRepository, ShoppingCartItemRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IStoreViolationPenaltyRepository, StoreViolationPenaltyRepository>();
builder.Services.AddScoped<ISupervise_Repository, Supervise_Repository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMerchantRepository, MerchantRepository>();

// 注册 Service 层（按DTO分类和角色重构后的服务）
// ========== 基础服务（用户信息管理） ==========
builder.Services.AddScoped<ICustomerInfoService, CustomerInfoService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICourierInfoService, CourierInfoService>();
builder.Services.AddScoped<IMerchantInfoService, MerchantInfoService>();  // 商家信息管理
builder.Services.AddScoped<IAdminInfoService, AdminInfoService>();

// ========== 售后申请服务（按角色分类） ==========
builder.Services.AddScoped<ICustomerAfterSaleService, CustomerAfterSaleService>();
builder.Services.AddScoped<IMerchantAfterSaleService, MerchantAfterSaleService>();
builder.Services.AddScoped<IAdminAfterSaleService, AdminAfterSaleService>();

// ========== 评论服务（按角色分类） ==========
builder.Services.AddScoped<ICustomerCommentService, CustomerCommentService>();
builder.Services.AddScoped<IMerchantCommentService, MerchantCommentService>();
builder.Services.AddScoped<IAdminCommentService, AdminCommentService>();

// ========== 店铺举报惩罚服务（按角色分类） ==========
builder.Services.AddScoped<ICustomerPenaltyService, CustomerPenaltyService>();
builder.Services.AddScoped<IMerchantPenaltyService, MerchantPenaltyService>();
builder.Services.AddScoped<IAdminPenaltyService, AdminPenaltyService>();

// ========== 配送投诉服务（按角色分类） ==========
builder.Services.AddScoped<ICustomerDeliveryComplaintService, CustomerDeliveryComplaintService>();
builder.Services.AddScoped<ICourierDeliveryComplaintService, CourierDeliveryComplaintService>();
builder.Services.AddScoped<IAdminDeliveryComplaintService, AdminDeliveryComplaintService>();

// ========== 订单服务（按角色分类） ==========
builder.Services.AddScoped<ICustomerOrderService, CustomerOrderService>();
builder.Services.AddScoped<IMerchantOrderService, MerchantOrderService>();

// ========== 菜品服务（按角色分类） ==========
builder.Services.AddScoped<IMerchantDishService, MerchantDishService>();
builder.Services.AddScoped<ICustomerDishService, CustomerDishService>();

// ========== 优惠券服务（按角色分类） ==========
builder.Services.AddScoped<IMerchantCouponService, MerchantCouponService>();
builder.Services.AddScoped<ICustomerCouponService, CustomerCouponService>();

// ========== 店铺服务（按角色分类） ==========
builder.Services.AddScoped<ICustomerStoreService, CustomerStoreService>();
builder.Services.AddScoped<IMerchantStoreService, MerchantStoreService>();

// ========== 配送任务服务（按角色分类） ==========
builder.Services.AddScoped<IMerchantDeliveryTaskService, MerchantDeliveryTaskService>();
builder.Services.AddScoped<ICourierDeliveryTaskService, CourierDeliveryTaskService>();

// ========== 其他服务 ==========
builder.Services.AddScoped<IGeoHelper, GeoHelper>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IDishCategoryService, DishCategoryService>();

// ========== 后台任务服务 ==========
builder.Services.AddHostedService<MonthlyCommissionResetService>();
builder.Services.AddHostedService<MonthlySalesUpdateService>();
var app = builder.Build();

// 如果是开发环境，启用 Swagger UI 来浏览 API 接口文档
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 配置静态文件服务
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = ""
});

// wwwroot/images/random.png 会被映射成一个浏览器可访问的 URL http://localhost:8080/images/random.png
// 这样就可以在浏览器中访问 wwwroot/images/random.png 这个文件了
// 配置头像文件服务
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "avatars")),
    RequestPath = "/avatars"
});

// 配置店铺图片文件服务
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "images", "stores")),
    RequestPath = "/images/stores"
});

// 配置菜品图片文件服务
var dishesImagesPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "images", "dishes");
if (!Directory.Exists(dishesImagesPath))
{
    Directory.CreateDirectory(dishesImagesPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(dishesImagesPath),
    RequestPath = "/images/dishes"
});

// 启用 CORS
app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
