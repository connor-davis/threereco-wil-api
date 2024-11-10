
# 3rEco ASP .NET Core Web API

Welcome to the 3rEco ASP .NET Core Web API! This project has been built with JWT Authentication Logic and CRUD Functionality.

3rEco is an application that allows waste collection recycling businesses to manage their collectors, products and collections easily.

### 🛠️ Technologies Used:

<div align="center">
  <img src="https://skillicons.dev/icons?i=dotnet" height="40" alt="actix logo"  />
  <img width="12" />
</div>

### Deployment

There are some steps involved to get this API up and running and ready to use.

#### 1. Clone This Repository

```bash
git clone https://github.com/connor-davis/threereco-wil-api
```

#### 2. Dependencies

1. This API uses the PostgreSQL Database to store data.
2. This API uses the .NET 8.0 Runtime with ASP.NET CORE 8.0 Runtime

#### 3. Create The Database

```sql
CREATE DATABASE "whatyouwanttocallit";
```

#### 4. Create The `appsettings.json` File

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "PostgresDB": "Host=host;Username=username;Password=password;Database=whatyouwanttocallit"
  }
}
```

#### 5. Build The Project

```bash
dotnet build three-api.sln -configuration release
```

Please make sure for this step as well as step 4 that you are in the same folder as this project.

#### 6. Create Your Admin User

Navigate to the applications url and append /swagger to the end. You will see the Swagger UI. Here you can register your admin user.

#### 7. Set Your Admin Users Role

```sql
UPDATE "Users" SET "Roles" = '{0}' WHERE "Email" = 'youradminusersemail';
```

#### 7. You Are Done ✅

You can now use the admin users login details to remain authenticated and begin playing around with the API.
