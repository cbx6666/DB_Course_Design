# 登录/注册页前端说明文档

## 📋 页面描述

本页面是系统的统一入口，采用 Vue 3 (Composition API) 和 Tailwind CSS 构建。它为不同身份的用户（消费者、商家、骑手、管理员）提供了动态的、响应式的注册和登录功能。

- **登录页**: 用户通过选择身份、输入账号（手机号/邮箱）和密码进行验证。支持“记住密码”和第三方账号快速登录。
- **注册页**: 用户首先选择身份，页面会动态渲染出对应身份所需的全部注册字段。表单包含前端输入验证，并为商家、骑手、管理员等角色提供了专属的信息填写区域。

📍 **页面路径**  
- 登录/注册统一路径: `/` 或 `/auth` (具体由路由配置决定)  
- 页面组件文件: `LoginPage.vue` (示例命名)

---

## 🛠️ 页面功能点详解

### 🔄 Tab切换
- 通过 `activeTab` 变量 (`'login'` 或 `'register'`) 控制显示登录或注册表单。
- UI 上表现为“登录”和“注册”两个可点击的按钮，有明确的激活状态。

### 👤 身份选择
- 通过 `selectedRole` 变量控制用户选择的身份 (`consumer`, `merchant`, `rider`, `admin`)。
- UI 上表现为四个带图标的单选框，选中时有高亮效果。
- 注册表单的内容会根据 `selectedRole` 的值动态显示或隐藏特定字段。

### 🔑 登录功能 (`activeTab === 'login'`)
- **表单数据**：绑定到 `loginForm` 对象，包含 `account` 和 `password` 字段。
- **密码可见性**：通过 `showPassword` 变量和点击事件切换密码输入框的 `type` 属性。
- **记住密码**：通过 `rememberPassword` 复选框记录用户偏好。
- **提交逻辑**：调用 `handleLogin` 方法，在请求期间 `isLoading` 状态为 `true`，按钮显示加载中并被禁用。

### 📝 注册功能 (`activeTab === 'register'`)
- **通用信息表单**：绑定到 `registerForm` 对象，包含所有用户共有的字段，如 `nickname`, `phone`, `email`, `password` 等。
- **角色特定信息**：
  - **骑手**：额外表单绑定到 `riderInfo` 对象 (`vehicleType`, `monthlySalary` 等)。
  - **管理员**：额外表单绑定到 `adminInfo` 对象 (`managementObject`, `handledItems`)。
  - **商家**：额外表单绑定到 `merchantInfo` 和 `storeInfo` 对象，包含店铺名称、地址、营业时间、营业执照上传等复杂字段。
- **验证码**：`sendVerificationCode` 方法负责发送验证码，并使用 `codeCountdown` 实现 60 秒倒计时。
- **用户协议**：必须勾选 `agreeTerms` 复选框，否则注册按钮禁用。
- **提交逻辑**：调用 `handleRegister` 方法，该方法会先调用 `validateRegisterForm` 进行全面的前端校验，通过后才发送请求。

### 🗣️ 第三方登录
- 页面底部提供了 Google、Facebook、WeChat 的登录按钮，预留了第三方登录接口的调用位置。

---

## 🔌 调用接口

### 1. 用户登录接口

- **接口说明**: 验证用户身份并返回 Token。在前端 `handleLogin` 函数中调用。
- **请求路径**: `/api/auth/login`
- **请求方法**: `POST`

#### 请求参数说明 (`loginForm` & `selectedRole`)

| 字段名     | 类型   | 是否必填 | 说明                               |
| ---------- | ------ | -------- | ---------------------------------- |
| `identity` | string | 是       | 用户选择的身份。值为：`consumer`, `merchant`, `rider`, `admin`。对应 `selectedRole`。 |
| `account`  | string | 是       | 用户输入的手机号或邮箱。对应 `loginForm.account`。 |
| `password` | string | 是       | 用户登录密码。对应 `loginForm.password`。 |

#### 返回参数说明

| 字段名     | 类型   | 说明                               |
| ---------- | ------ | ---------------------------------- |
| `code`     | int    | 状态码，200 表示成功。              |
| `message`  | string | 返回的提示信息。                    |
| `data.token` | string | 登录成功后返回的认证 Token。       |
| `data.user`  | object | 登录成功的用户信息，可用于直接渲染首页，减少一次请求。 |

---

### 2. 发送注册验证码接口

- **接口说明**: 向用户手机号发送短信验证码。在前端 `sendVerificationCode` 函数中调用。
- **请求路径**: `/api/auth/send-code`
- **请求方法**: `POST`

#### 请求参数说明

| 字段名  | 类型   | 是否必填 | 说明                    |
| ------- | ------ | -------- | ----------------------- |
| `phone` | string | 是       | 用户输入的 11 位手机号码。对应 `registerForm.phone`。 |

---

### 3. 用户注册接口

- **接口说明**: 接收包含所有信息的表单，创建新用户及关联信息。在前端 `handleRegister` 函数中调用。
- **请求路径**: `/api/auth/register`
- **请求方法**: `POST`

#### 请求参数说明 (`一个复杂的 JSON 对象`)

请求体根据 `identity` 字段的值，结构会有所不同。

##### 通用结构：

```json
{
  "identity": "consumer", // 或者 "merchant", "rider", "admin"
  "user": {
    "username": "用户昵称",      // 对应 registerForm.nickname (varchar(15))
    "password": "用户密码",      // 对应 registerForm.password (varchar(10))
    "phoneNumber": "13800138000", // 对应 registerForm.phone (number(11,0))
    "email": "user@example.com",  // 对应 registerForm.email (varchar(30))
    "gender": "男",             // 对应 registerForm.gender (varchar(2))
    "birthday": "2000-01-01",     // 对应 registerForm.birthday (date)
    "verificationCode": "123456" // 对应 registerForm.verificationCode
  },
  // 以下为特定角色信息，根据 identity 字段三选一，或全无(消费者)
  "riderInfo": { ... },
  "adminInfo": { ... },
  "storeInfo": { ... }
}

```

### 角色特定信息详情

#### 当 `identity = rider` 时，包含 `riderInfo` 对象：

| 字段名             | 类型   | 说明                                   |
| ------------------ | ------ | -------------------------------------- |
| `vehicleType`      | string | 配送车型。对应 `riderInfo.vehicleType` (varchar(20))。 |
| `avgDeliveryTime`  | string | 平均配送时间。对应 `riderInfo.avgDeliveryTime`。 |
| `monthlySalary`    | number | 预期月薪。对应 `riderInfo.monthlySalary` (integer)。 |

#### 当 `identity = admin` 时，包含 `adminInfo` 对象：

| 字段名             | 类型   | 说明                                   |
| ------------------ | ------ | -------------------------------------- |
| `managedEntities`  | string | 管理对象。对应 `adminInfo.managementObject` (varchar(50))。 |
| `issueDescription` | string | 处理事项说明。对应 `adminInfo.handledItems`。 |

#### 当 `identity = merchant` 时，包含 `storeInfo` 对象：

| 字段名             | 类型   | 说明                                   |
| ------------------ | ------ | -------------------------------------- |
| `storeName`        | string | 店铺名称。对应 `storeInfo.name` (varchar(50))。 |
| `storeAddress`     | string | 店铺地址。对应 `storeInfo.address` (varchar(100))。 |
| `businessHours`    | object | 营业时间。对应 `storeInfo.businessHours` (json 或 text 存储)。 |
| `storeCreationTime`| date   | 店铺建立时间。对应 `storeInfo.establishmentDate` (date)。 |
| `storeFeatures`    | string | 经营类别。对应 `storeInfo.category` (varchar(500))。 |
| `contactPhone`     | string | 店铺联系电话。对应 `storeInfo.contactPhone`。 |
| `businessLicense`  | file   | 此字段应通过独立的上传接口处理，注册接口只传递 URL。 |

---

### 返回参数说明

| 字段名   | 类型   | 说明                                    |
| -------- | ------ | --------------------------------------- |
| `code`   | int    | 状态码，200 表示成功。                   |
| `message`| string | 返回的提示信息，如 “注册成功”。         |

---

### 4. 文件上传接口 (建议)

**接口说明**: 用于上传商家营业执照等文件。

- **请求路径**: `/api/upload/license`
- **请求方法**: `POST` (使用 `multipart/form-data`)

#### 返回参数

| 字段名 | 类型   | 说明                                           |
| ------ | ------ | ---------------------------------------------- |
| `url`  | string | 返回上传成功后的文件 URL，该 URL 将被用于注册接口的 `businessLicense` 字段。 |

---

### UI设计稿

**UI实现**:[原型图跳转](https://readdy.link/share/65c24cf2b09e237ddc952325997ca7f8)。
