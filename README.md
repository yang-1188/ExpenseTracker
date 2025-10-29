# ExpenseTracker 記帳應用程式

>  **開發中專案** - 此專案目前正在積極開發階段，功能尚未完整，API 可能會有變動。

一個現代化的個人記帳應用程式，採用前後端分離架構開發。

## 專案動機

這是一個為了在畢業後保持 C# .NET Core 開發技能而獨立開發的練習專案。透過實作完整的前後端應用程式，持續精進現代 Web 開發技術棧。

##  技術棧

### 前端

- **Vue 3** - 漸進式 JavaScript 框架
- **Vite** - 快速建置工具
- **Pinia** - 狀態管理
- **Vue Router** - 路由管理
- **ESLint + Prettier** - 程式碼品質控制

### 後端

- **.NET Core Web API** - RESTful API 服務
- **Entity Framework Core** - ORM 資料存取
- **SQL Server** - 資料庫
- **JWT Authentication** - 身份驗證
- **BCrypt** - 密碼加密

##  功能特色

### 已實現功能

- 使用者註冊與登入 (JWT 驗證)
- 基礎 API 架構

### 開發中功能

- 收支記錄管理
- 支出分析與統計
- 分類管理
- 響應式前端介面

### 計劃功能

- 圖表視覺化
- 資料匯出
- 預算提醒

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

## 專案結構

```
ExpenseTracker/
├── frontend/          # Vue.js 前端應用
│   ├── src/
│   │   ├── components/
│   │   ├── views/
│   │   ├── router/
│   │   └── stores/
│   └── package.json
├── backend/           # .NET Core 後端 API
│   └── ExpenseTracker.Api/
│       ├── Controllers/
│       ├── Models/
│       ├── Services/
│       └── Data/
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

## 開發狀態

此專案目前處於早期開發階段
