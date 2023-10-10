# Minimal API Boilerplate

使用 .NET Minimal API 實作的 API Boilerplate

## Requirements

- .NET 7
- PostgreSQL

## Description

- 專案適度分層
- 多個 `Startup.cs` 放置啟動程式碼，讓 `Program.cs` 不要太混亂
- 不使用控制器
- 參照 REST 風格設計 API

## Feature
- [x] Cookie 驗證身分
- [x] Validation attributes 檢查 Request
- [x] ExceptionMiddleware 集中例外處理
- [x] 透過 Npgsql Entity Framework Core 操作資料庫
- [x] 透過 Serilog 收集 Log
- [ ] 透過 Dapper 操作資料庫
- [ ] PostgreSQL BulkUpsert 範例
- [ ] BackgroundService 背景工作範例
- [ ] MongoDB
- [ ] Unit Test

## Memo

### Database Migrations
```
cd ./MiniApi/
dotnet ef migrations add Init --context ApplicationDbContext -o ./Persistence/EntityFrameworkCore/Migrations/
```

### Output SQL script
```
cd ./MiniApi/
dotnet ef migrations script --output "script.sql"
```

