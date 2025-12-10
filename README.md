# TJFeast

> Copyright © 2025 TJFeast Development Team - Licensed under [MIT License](LICENSE)
>
> 版权所有 © 2025 TJFeast | TJFeast 项目开发组 - 采用 [MIT 许可证](LICENSE)授权

![License](https://img.shields.io/badge/license-MIT-green)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
![Version](https://img.shields.io/badge/version-1.0.0-blue)

<!-- 技术栈徽章 -->
![Vue](https://img.shields.io/badge/Vue-3.3.4-brightgreen)
![TypeScript](https://img.shields.io/badge/TypeScript-5.1-blue)
![Tailwind CSS](https://img.shields.io/badge/TailwindCSS-3.3.3-blue)
![Element Plus](https://img.shields.io/badge/ElementPlus-2.3.10-purple)
![C#](https://img.shields.io/badge/C%23-8.0-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![EF Core](https://img.shields.io/badge/EF_Core-8.0-orange)


## 后端技术栈（C# / ASP.NET Core）
- **Runtime / 框架**：.NET 8 + ASP.NET Core Web API（C# 8）
  - Minimal Hosting：`Program.cs` 中注册服务与中间件，统一入口
  - 中间件管线：`UseAuthentication()` + `UseAuthorization()` + 路由映射
  - 属性路由：`[ApiController]` + `[Route("api/...")]` 规范接口前缀
  - 统一响应：`ApiResponseDto<T>` 封装 `code/message/data`，前端直接消费
- **数据访问 / ORM**：Entity Framework Core 8（Oracle 驱动），代码优先 + 仓储模式
  - 分层：Controller → Service → Repository → DbContext，DTO 做输入输出隔离
  - Oracle 兼容：布尔判断用 `CountAsync > 0`，避免生成 `TRUE/FALSE`；注意标识符大小写
  - 导航加载：必要时 `Include/ThenInclude` 预加载，避免 N+1
- **认证与鉴权**：JWT Bearer
  - 验证：`UseAuthentication()` 验证 Token
  - 授权：`UseAuthorization()` + `[Authorize]` 控制访问
- **依赖注入（DI）**：内置容器，仓储/服务按 Scoped 注册，如 `AddScoped<IRepo, Repo>()`
- **静态文件 / 上传**：统一图片上传服务，保存于 `wwwroot`（支持头像、举报/投诉图片等子目录）
- **接口示例**：RESTful，统一前缀 `api/`，如收藏夹相关 `/api/customer/info/favorites`

## 项目运行(本地)
### 数据库迁移

``` bash
$ cd back-end
$ dotnet ef migrations add InitDatabase
$ dotnet ef database update
```
### 启动前端

```bash
$ cd front-end 
$ npm install
$ npm run serve
```

### 启动后端
确认电脑上有安装.NET SDK 8.0.412，若未安装，请到网站https-//dotnet.microsoft.com/zh-cn/download/dotnet/8.0 下载
```bash
$ cd back-end
$ dotnet run
```

## 成品展示
### 登录与注册
选择用户种类登录和注册
![image](./image/登录.png)  
![image](./image/注册.png)

### 消费者页面
#### 首页
可以查看首页推荐店铺，新增实现跳转到指定种类的店铺和优惠券中心。  
![image](./image/首页.png)
#### 个人中心
![image](./image/个人中心.png)
#### 店铺页面
![image](./image/店铺.png)
#### 点单页面
新增菜品种类筛选。  
![image](./image/点单.png)
#### 结账页面
![image](./image/结账.png)
#### 订单页面
![image](./image/历史订单.png)
#### 发起售后
![image](./image/发起售后.png)
#### 优惠页面
![image](./image/优惠中心.png)
#### 售后页面
![image](./image/售后页面.png)

### 商家页面
#### 店铺概况
![image](./image/店铺概况.png)
#### 订单中心
![image](./image/订单中心.png)
#### 菜品管理
![image](./image/菜品管理.png)
#### 商家配券
![image](./image/商家配券.png)
#### 菜品管理
![image](./image/菜品管理.png)
#### 售后管理
![image](./image/售后管理.png)
#### 商家信息
![image](./image/商家信息.png)

### 骑手页面
#### 骑手首页
![image](./image/骑手首页.png)
#### 接单页面
![image](./image/接单页面.png)
#### 配送页面
![image](./image/配送页面.png)
#### 投诉页面
![image](./image/投诉页面.png)
#### 骑手信息
![image](./image/骑手信息.png)

### 管理员页面
#### 信息页面
![image](./image/管理员信息.png)
#### 处理页面
![image](./image/管理员处理.png)