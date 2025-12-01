# ExpenseTracker 記帳應用程式

> ⚠️ **開發中專案** - 核心功能已完成，持續優化與新增功能中。

一個現代化的個人記帳應用程式，採用前後端分離架構開發。支援多帳戶、分類管理，並提供直覺的使用者介面。

## 專案動機

這是一個為了在畢業後保持 C# .NET Core 開發技能而獨立開發的練習專案。透過實作完整的前後端應用程式，持續精進現代 Web 開發技術棧。

## 🚀 技術

### 前端
- **Vue 3** (Composition API) - 漸進式 JavaScript 框架
- **Element Plus** - UI 元件庫
- **Vite** - 快速建置工具
- **Axios** - HTTP 請求處理
- **Vue Router** - 路由管理與導航守衛
- **vue3-google-login** - Google OAuth 2.0 整合
- **Pinia** - 狀態管理
- **ESLint + Prettier** - 程式碼品質控制

### 後端
- **.NET 8.0 Web API** - RESTful API 服務
- **Entity Framework Core** - ORM 資料存取
- **SQL Server** - 關聯式資料庫
- **JWT Bearer Authentication** - 無狀態身份驗證
- **Google.Apis.Auth** - Google ID Token 驗證
- **BCrypt.Net** - 密碼雜湊加密
- **Dependency Injection** - 服務層架構設計

## 📋 功能特色

### ✅ 已實現功能

**使用者系統**
- 使用者註冊與登入（Email/密碼）
- Google 第三方登入整合
  - Google ID Token 驗證
  - 自動帳號綁定與註冊
  - 頭像同步
- JWT Token 身份驗證
- 路由守衛保護

**交易管理**
- 新增收支記錄
- 查詢交易列表（依使用者過濾）
- 刪除交易記錄
- 交易日期、金額、備註記錄

**分類與帳戶**
- 分類查詢（支出/收入分類）
- 帳戶查詢
- 下拉選單動態載入

**前端介面**
- 響應式 Dashboard 頁面
- Element Plus 表格展示
- 新增交易對話框
- 刪除確認提示
- Loading 狀態處理

### 🚧 開發中功能

- 交易編輯功能
- 分類與帳戶的 CRUD 管理
- 收支統計與圖表
- 日期範圍篩選
- 搜尋功能

### 📋 計劃功能

- 圖表視覺化（Chart.js / ECharts）
- 資料匯出（CSV/Excel）
- 預算設定與提醒
- 多幣別支援
- 深色模式
- 更多第三方登入選項（Facebook、Line）

## 開發環境設定

### 前置需求

- Node.js (v20.19.0 或更高版本)
- .NET 8.0 SDK
- SQL Server

### 前端設定

```bash
cd frontend
npm install
npm run dev
```

前端開發伺服器將在 `http://localhost:5173` 啟動

### 後端設定

1. 設定資料庫連線字串和 JWT 設定：

```bash
cd backend/ExpenseTracker.Api/ExpenseTracker.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "你的資料庫連線字串"
dotnet user-secrets set "JwtSettings:SecretKey" "你的JWT密鑰"
dotnet user-secrets set "JwtSettings:Issuer" "ExpenseTracker"
dotnet user-secrets set "JwtSettings:Audience" "ExpenseTracker-Users"
dotnet user-secrets set "Authentication:Google:ClientId" "你的Google Client ID"
```

2. 執行資料庫遷移：

```bash
dotnet ef database update
```

3. 啟動 API 服務：

```bash
dotnet run
```

後端 API 將在 `https://localhost:7000` 啟動

## 📁 專案結構

```
ExpenseTracker/
├── frontend/                    # Vue.js 前端應用
│   ├── src/
│   │   ├── api/                # API 請求封裝
│   │   ├── components/         # 可重用元件
│   │   ├── views/              # 頁面元件
│   │   │   ├── LoginView.vue
│   │   │   ├── RegisterView.vue
│   │   │   └── DashboardView.vue
│   │   ├── router/             # 路由設定
│   │   └── stores/             # Pinia 狀態管理
│   └── package.json
│
├── backend/                     # .NET Core 後端 API
│   └── ExpenseTracker.Api/
│       ├── Controllers/         # API 控制器
│       │   ├── AuthController.cs
│       │   ├── TransactionsController.cs
│       │   ├── CategoriesController.cs
│       │   └── AccountsController.cs
│       ├── Models/              # 資料模型
│       │   ├── User.cs
│       │   ├── Transaction.cs
│       │   ├── Category.cs
│       │   └── Account.cs
│       ├── Services/            # 業務邏輯層
│       │   ├── IAuthService.cs / AuthService.cs
│       │   ├── ITransactionService.cs / TransactionService.cs
│       │   ├── ICategoryService.cs / CategoryService.cs
│       │   └── IAccountService.cs / AccountService.cs
│       ├── Dtos/                # 資料傳輸物件
│       └── Program.cs           # 應用程式進入點
└── README.md
```

## 開發指令

### 前端

```bash
npm run dev      # 開發模式
npm run build    # 建置生產版本
npm run lint     # 程式碼檢查
npm run format   # 程式碼格式化
```

### 後端

```bash
dotnet run                    # 執行應用程式
dotnet build                  # 建置專案
dotnet ef migrations add      # 新增資料庫遷移
dotnet ef database update     # 更新資料庫
```

##  部署

### 前端部署

```bash
cd frontend
npm run build
# 將 dist/ 資料夾部署到靜態檔案伺服器
```

### 後端部署

```bash
cd backend/ExpenseTracker.Api/ExpenseTracker.Api
dotnet publish -c Release
# 部署 publish 資料夾到伺服器
```

## 🎯 API 端點

### 認證相關
- `POST /api/Auth/register` - 使用者註冊
- `POST /api/Auth/login` - 使用者登入
- `POST /api/Auth/google-login` - Google 第三方登入

### 交易管理
- `GET /api/Transactions` - 取得使用者的所有交易記錄
- `POST /api/Transactions` - 新增交易記錄
- `DELETE /api/Transactions/{id}` - 刪除交易記錄

### 查詢資料
- `GET /api/Categories` - 取得分類列表
- `GET /api/Accounts` - 取得帳戶列表

> 所有交易相關 API 皆需要 JWT Token 驗證

## 🚧 開發狀態

**目前進度：核心功能已完成 (~70%)**

✅ 完成項目：
- 後端 API 架構與服務層設計
- 使用者認證系統
- 交易 CRUD（新增、查詢、刪除）
- 前端基礎介面與路由
- JWT 身份驗證與路由守衛

🚧 進行中：
- 交易編輯功能
- 分類與帳戶管理介面

📋 待開發：
- 統計分析功能
- 圖表視覺化
- 進階篩選與搜尋

## 📞 聯絡方式

如有任何問題或建議，歡迎開啟 Issue 討論。
