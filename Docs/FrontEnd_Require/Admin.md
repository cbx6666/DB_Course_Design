# 管理员后台系统

## 📄 页面描述

系统管理员使用的核心后台管理界面,用于处理:
- 用户售后申请
- 用户投诉
- 店铺违规举报
- 用户评论审核
- 管理员个人信息管理

**页面路径**: `/admin`

## 🎯 核心功能点

1. 🔄 **面板切换**: 通过侧边栏在不同管理面板间切换
   - 管理员信息
   - 售后处理
   - 投诉处理
   - 违规举报处理
   - 评论审核

2. 📊 **列表管理**:
   - 状态筛选 (待处理/已完成)
   - 关键词搜索
   - 详情查看
   - 处理操作
   - 实时状态更新

## 🔌 API 接口文档

### 1. 售后管理接口

#### 1.1 获取售后列表

| 配置项 | 说明 |
|--------|------|
| 请求路径 | `GET /api/v1/after-sales` |
| 调用时机 | 页面加载时 |

**返回数据结构**:

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | number | 唯一ID |
| applicationId | string | 售后申请编号 |
| orderId | string | 关联订单编号 |
| applicationTime | string | 申请时间 (YYYY-MM-DD HH:mm) |
| description | string | 情况说明 |
| status | string | 状态 ("待处理"/"已完成") |
| punishment | string | 处理措施，默认为 "-" |

#### 1.2 更新售后信息

| 配置项 | 说明 |
|--------|------|
| 请求路径 | `PUT /api/v1/after-sales/{id}` |
| 调用时机 | 管理员在售后详情弹窗中执行处罚时 |

**输入表单结构**:

| 字段名 | 类型 | 是否必填 | 说明 |
|--------|------|----------|------|
| id | number | 是 | 售后记录ID |
| applicationId | string | 是 | 售后申请编号 |
| orderId | string | 是 | 关联订单编号 |
| applicationTime | string | 是 | 申请时间 |
| description | string | 是 | 情况说明 |
| status | string | 是 | 状态，固定为 "已完成" |
| punishment | string | 是 | 处罚措施文本 |
| punishmentReason | string | 是 | 处罚原因 |
| processingNote | string | 是 | 处理备注 |

**返回数据结构**:
返回与输入表单结构相同的更新后的售后对象。

### 2. 投诉管理接口

#### 2.1 获取投诉列表

| 配置项 | 说明 |
|--------|------|
| 请求路径 | `GET /api/v1/complaints` |
| 调用时机 | 页面加载时 |

**返回数据结构**:

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | number | 唯一ID |
| complaintId | string | 投诉编号 |
| target | string | 投诉对象 |
| content | string | 投诉内容 |
| applicationTime | string | 申请时间 (YYYY-MM-DD HH:mm) |
| status | string | 状态 ("待处理"/"已完成") |
| punishment | string | 处理措施，默认为 "-" |

#### 2.2 更新投诉信息

| 配置项 | 说明 |
|--------|------|
| 请求路径 | `PUT /api/v1/complaints/{id}` |
| 调用时机 | 管理员在投诉详情弹窗中执行处罚时 |

**输入表单结构**:

| 字段名 | 类型 | 是否必填 | 说明 |
|--------|------|----------|------|
| id | number | 是 | 投诉记录ID |
| complaintId | string | 是 | 投诉编号 |
| target | string | 是 | 投诉对象 |
| content | string | 是 | 投诉内容 |
| applicationTime | string | 是 | 申请时间 |
| status | string | 是 | 状态，固定为 "已完成" |
| punishment | string | 是 | 处罚措施文本 |
| punishmentReason | string | 是 | 处罚原因 |
| processingNote | string | 是 | 处理备注 |

**返回数据结构**:
返回与输入表单结构相同的更新后的投诉对象。

### 3. 违规举报管理接口

#### 3.1 获取违规举报列表

| 配置项 | 说明 |
|--------|------|
| 请求路径 | `GET /api/v1/violations` |
| 调用时机 | 页面加载时 |

**返回数据结构**:

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | number | 唯一ID |
| punishmentId | string | 处罚编号 |
| storeName | string | 违规店铺名称 |
| reason | string | 违规原因 |
| merchantPunishment | string | 对商家的处罚措施，默认为 "-" |
| storePunishment | string | 对店铺的处罚措施，默认为 "-" |
| punishmentTime | string | 处罚时间 (YYYY-MM-DD HH:mm) |
| status | string | 状态 ("待处理"/"执行中"/"已完成") |

#### 3.2 更新违规举报信息

| 配置项 | 说明 |
|--------|------|
| 请求路径 | `PUT /api/v1/violations/{id}` |
| 调用时机 | 管理员在违规详情弹窗中“开始执行”或“完成执行”时 |

**输入表单结构**:

| 字段名 | 类型 | 是否必填 | 说明 |
|--------|------|----------|------|
| id | number | 是 | 违规记录ID |
| punishmentId | string | 是 | 处罚编号 |
| storeName | string | 是 | 违规店铺名称 |
| reason | string | 是 | 违规原因 |
| punishmentTime | string | 是 | 处罚时间 |
| status | string | 是 | 状态 ("执行中"/"已完成") |
| merchantPunishment| string | “开始执行”时必填 | 对商家的处罚措施 |
| storePunishment| string | “开始执行”时必填 | 对店铺的处罚措施 |
| processingNote | string | 是 | 处理备注 |

**返回数据结构**:
返回与输入表单结构相同的更新后的违规对象。

### 4. 评论审核管理接口

#### 4.1 获取评论审核列表

| 配置项 | 说明 |
|--------|------|
| 请求路径 | `GET /api/v1/reviews` |
| 调用时机 | 页面加载时 |

**返回数据结构**:

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | number | 唯一ID |
| reviewId | string | 评论ID |
| username | string | 用户名 |
| storeName | string | 店铺名称 |
| content | string | 评论内容 |
| rating | number | 评分 (1-5) |
| submitTime | string | 提交时间 (YYYY-MM-DD HH:mm) |
| status | string | 状态 ("待处理"/"已完成") |
| punishment | string | 处理措施，默认为 "-" |

#### 4.2 更新评论审核信息

| 配置项 | 说明 |
|--------|------|
| 请求路径 | `PUT /api/v1/reviews/{id}` |
| 调用时机 | 管理员在评论详情弹窗中执行处理时 |

**输入表单结构**:

| 字段名 | 类型 | 是否必填 | 说明 |
|--------|------|----------|------|
| id | number | 是 | 评论记录ID |
| reviewId | string | 是 | 评论ID |
| username | string | 是 | 用户名 |
| storeName | string | 是 | 店铺名称 |
| content | string | 是 | 评论内容 |
| rating | number | 是 | 评分 |
| submitTime | string | 是 | 提交时间 |
| status | string | 是 | 状态，固定为 "已完成" |
| punishment | string | 是 | 处理方式文本 (如 "通过审核", "删除评论") |
| punishmentReason | string | 是 | 处理原因 |
| processingNote | string | 是 | 审核备注 |

**返回数据结构**:
返回与输入表单结构相同的更新后的评论对象。