# 🚪 登录/注册页 前端说明文档

## 📄 一. 页面描述

### 功能
提供系统统一的登录和多角色注册入口。

### 路径
`/login` (由路由配置决定)

### 文件
`src/views/login/LoginView.vue`

---

## 🎯 二. 核心功能点

- 🔄 **表单切换**: 支持在“登录”和“注册”两个表单之间切换。
- 👥 **角色系统**: 支持“管理员”、“商家”、“骑手”、“消费者”四种角色的登录和注册。
- 📝 **动态表单**: 注册表单会根据所选角色动态增减专属信息字段。
- ⏳ **异步处理**: 登录和注册期间，提交按钮会显示加载状态并被禁用。
- 💬 **响应处理**: 操作成功或失败时，会通过 **alert** 弹出后端返回的信息。
- 🗝️ **Token 存储**: 登录成功后，会将后端返回的 **token** 存入 `localStorage`。
- 📜 **协议弹窗**: 注册时，用户协议和隐私政策通过弹窗展示。

---

## 🔌 三. 调用接口

所有接口通过 `src/services/api.ts` 模块发起，该模块配置的基础路径为 `/api`。

### 🌐 接口调用说明

所有接口请求均通过 `src/services/api.ts` 文件中封装的函数发起。

- **请求基础路径** (baseURL): `http://localhost:3000/api`
- **全局请求头** (Headers): `Content-Type: application/json`

---

### 1. 🏫 用户登录接口

#### 🔐 前端调用:
`api.login(data)`

#### 🌍 请求路径:
`POST /login`

#### 📝 接口说明:
接收用户凭证，验证成功后返回身份令牌 (Token) 和用户信息。

#### 📤 前端发送的请求体 (Request Body)

前端会发送一个符合 `UserData` 接口的对象，包含以下字段：

| 字段名      | 类型   | 来源 (前端变量)       | 示例值           |
| ----------- | ------ | --------------------- | ---------------- |
| account     | string | `loginForm.account`    | `"user@example.com"` |
| password    | string | `loginForm.password`   | `"123456"`       |
| role        | string | `selectedRole.value`   | `"consumer"`     |

#### 📥 前端期望的返回体 (Response Body)

- **登录成功 (HTTP 状态码 200)**:
  
  前端期望接收一个 JSON 对象，至少包含以下字段：

  | 字段名     | 类型   | 说明                                 |
  | ---------- | ------ | ------------------------------------ |
  | token      | string | 必需。用于后续请求身份验证的 JWT。   |
  | user       | object | 必需。包含登录用户信息的对象。       |
  | message    | string | 可选。成功的提示信息，如“登录成功！” |

- **登录失败 (HTTP 状态码 4xx 或 5xx)**:

  前端期望接收一个 JSON 对象，包含以下字段：

  | 字段名     | 类型   | 说明                               |
  | ---------- | ------ | ---------------------------------- |
  | error      | string | 必需。描述失败原因的字符串，如“账号或密码错误”。 |

---

### 2. 📝 用户注册接口

#### 🔑 前端调用:
`api.register(data)`

#### 🌍 请求路径:
`POST /register`

#### 📝 接口说明:
接收完整的用户注册信息（包含基础信息和角色特定信息），创建新用户。

#### 📤 前端发送的请求体 (Request Body)

前端会发送一个符合 `RegistrationData` 接口的对象，其结构根据用户选择的角色动态变化。

##### 通用字段 (所有角色都会发送):

| 字段名          | 类型   | 来源 (前端变量)         | 示例值              |
| --------------- | ------ | ----------------------- | ------------------- |
| nickname        | string | `registerForm.nickname`  | `"新用户123"`        |
| password        | string | `registerForm.password`  | `"mysecretpwd"`      |
| confirmPassword | string | `registerForm.confirmPassword` | `"mysecretpwd"`  |
| phone           | string | `registerForm.phone`     | `"13800138000"`      |
| email           | string | `registerForm.email`     | `"new@example.com"`  |
| gender          | string | `registerForm.gender`    | `"female"`           |
| birthday        | string | `registerForm.birthday`  | `"2000-01-01"`       |
| verificationCode| string | `registerForm.verificationCode` | `"123456"`    |
| role            | string | `selectedRole.value`     | `"merchant"`         |
| avatarUrl       | string | `registerForm.avatarUrl` | `""`                 |
| isPublic        | number | `registerForm.isPublic`  | `0`                  |

##### 角色特定字段 (根据 `role` 字段选择性包含)

- 当 `role` 为 `'rider'` 时，请求体中会额外包含 `riderInfo` 对象：

  | 字段名              | 类型   | 是否必填 | 说明     |
  | ------------------- | ------ | -------- | -------- |
  | riderInfo.vehicleType | string | 是       | 配送车型 |

- 当 `role` 为 `'admin'` 时，请求体中会额外包含 `adminInfo` 对象：

  | 字段名               | 类型   | 是否必填 | 说明           |
  | -------------------- | ------ | -------- | -------------- |
  | adminInfo.managementObject | string | 是       | 管理对象       |
  | adminInfo.handledItems   | string | 是       | 处理事项说明   |

- 当 `role` 为 `'merchant'` 时，请求体中会额外包含 `storeInfo` 对象：

  | 字段名                | 类型         | 是否必填 | 说明           |
  | --------------------- | ------------ | -------- | -------------- |
  | storeInfo.name         | string       | 是       | 店铺名称       |
  | storeInfo.address      | string       | 是       | 店铺地址       |
  | storeInfo.businessHours | string       | 是       | 营业时间       |
  | storeInfo.establishmentDate | string   | 是       | 店铺建立时间   |
  | storeInfo.businessLicense | File 或 null | 是       | 营业执照文件   |
  | storeInfo.category     | string       | 是       | 经营类别       |

---

## 返回参数说明

### ⇒ 注册成功

| 字段名    | 类型   | 说明                           |
| --------- | ------ | ------------------------------ |
| code      | int    | 状态码，201 表示成功。          |
| message   | string | 返回的提示信息，如“注册成功！” |

### ⇒ 注册失败

| 字段名    | 类型   | 说明                                         |
| --------- | ------ | -------------------------------------------- |
| code      | int    | 状态码，如 400、409 等。                     |
| error     | string | 描述失败原因的字符串，如“该手机号已被注册”。 |

