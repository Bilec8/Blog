﻿# Personal Blog App (ASP.NET Core MVC)

A simple blog application built with ASP.NET Core MVC and Entity Framework Core.

## ✨ Features

- 🔐 Authenticated users can **create, edit, and delete their own posts**
- 👀 Unauthenticated users can only **view posts**
- 🔒 Users can **only see their own posts** after logging in
- 📦 ASP.NET Identity integration
- 🎨 Responsive UI using Bootstrap
- 🗄️ SQL Server database with EF Core

## 🖼️ Screenshots
![Post list](Blog/docs/screenshots/post-list.png)
![Create post](Blog/docs/screenshots/create-post.png)

## 🚀 Getting Started

### ✅ Requirements

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- SQL Server (or use LocalDB)
- Git

### 📥 Clone & Run

```bash
git clone https://github.com/Bilec8/Blog.git
cd blog
dotnet ef database update
dotnet run
