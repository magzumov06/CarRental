# CarRental API 🚗

Современное многофункциональное веб-приложение (REST API) для системы
аренды автомобилей (Car Rental System), разработанное на платформе .NET
9 с использованием чистой многослойной архитектуры (Domain,
Infrastructure, WebApp).

## 🛠 Технологический стек

-   **Платформа:** .NET 9.0
-   **Фреймворк:** ASP.NET Core Web API
-   **База данных и ORM:** PostgreSQL, Entity Framework Core 9.0
    (Npgsql.EntityFrameworkCore.PostgreSQL)
-   **Аутентификация и безопасность:** JWT (JSON Web Tokens), ASP.NET
    Core Identity
-   **Фоновые задачи:** Hangfire (для планирования и выполнения задач в
    фоновом режиме)
-   **Уведомления:** MailKit / MimeKit (отправка email-уведомлений через
    SMTP)
-   **Логирование:** Serilog (консольное логирование и запись в файлы)
-   **Документация API:** Swagger / OpenAPI (Swashbuckle.AspNetCore)
-   **Контейнеризация:** Docker & Docker Compose

## 📁 Структура проекта

Решение `CarRental.sln` разделено на три ключевых проекта:

### 1. Domain (Доменный слой)

-   **Entities** --- основные бизнес-модели (`User`, `Car`, `Rental`,
    `BaseEntities`).
-   **DTOs** --- объекты передачи данных (`Account`, `CarDto`,
    `EmailDto`, `RentalDto`, `UserDto`).
-   **Enums** --- перечисления (`Roles`, `Status`).
-   **Filters** --- модели фильтрации и пагинации (`BaseFilter`,
    `CarFilter`, `RentalFilter`, `UserFilter`).
-   **Responces** --- стандартизированные ответы API и постраничная
    навигация (`Responce<T>`, `PaginationResponce<T>`).

### 2. Infrastructure (Инфраструктурный слой)

-   **Data** --- контекст базы данных (`DataContext`) и сидеры (`Seed`).
-   **Services** --- реализация бизнес-логики (`AccountService`,
    `CarService`, `RentalService`, `UserService`, `EmailSender`).
-   **Interfaces** --- контракты сервисов (`IAccountService`,
    `ICarService`, `IRentalService`, `IUserService`, `IEmailSender`).
-   **Helpers** --- вспомогательные утилиты (генерация JWT-токенов,
    хэширование паролей, шаблоны писем).
-   **FileStorage** --- управление загрузкой и хранением файлов
    (изображений автомобилей).
-   **Migrations** --- миграции базы данных Entity Framework Core.

### 3. WebApp (Презентационный слой)

-   **Controller** --- контроллеры REST API (`AccountController`,
    `CarController`, `RentalController`, `UserController`).
-   **Program.cs** --- точка входа, конфигурация сервисов и Middleware.
-   **appsettings.json** --- конфигурационные настройки (подключение к
    БД, JWT, SMTP, FileStorage).

## 🚀 Основной функционал (API Endpoints)

### 🔐 Авторизация и аккаунты (`/api/Account`)

-   Регистрация пользователей, вход в систему (генерация JWT-токена),
    смена пароля.

### 🚗 Автомобили (`/api/Car`)

-   Добавление, редактирование, удаление и просмотр каталога
    автомобилей.
-   Загрузка и прикрепление изображений к автомобилям.
-   Продвинутая фильтрация и пагинация (`CarFilter`).

### 📋 Аренда (`/api/Rental`)

-   Оформление заказов на аренду автомобилей, управление статусами
    аренды (`Status`), обновление и отмена.
-   Фильтрация истории аренд (`RentalFilter`).

### 👤 Пользователи (`/api/User`)

-   Управление профилями пользователей, обновление данных и ролевая
    модель (`Roles`).

## ⚙️ Инструкция по установке и запуску

### Вариант 1: Запуск через .NET CLI (Локально)

#### 1. Предварительные требования

-   Установленный .NET 9.0 SDK
-   Установленная СУБД PostgreSQL

#### 2. Клонируйте или распакуйте архив с проектом

``` bash
cd /path/to/CarRental
```

#### 3. Настройте `appsettings.json`

В файле `WebApp/appsettings.json` укажите параметры подключения к
PostgreSQL, секретный ключ JWT и настройки почтового сервера:

``` json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=car_rental_db;Username=postgres;Password=ваш_пароль;"
  },
  "JWT": {
    "Issuer": "Car-Rental",
    "Audience": "Car-Rental",
    "Key": "ВАШ_СЕКРЕТНЫЙ_КЛЮЧ_ДЛИНОЙ_НЕ_МЕНЕЕ_32_СИМВОЛОВ",
    "ExpiresDay": 3
  }
}
```

#### 4. Примените миграции базы данных

``` bash
dotnet ef database update --project Infrastructure --startup-project WebApp
```

#### 5. Запустите проект

``` bash
cd WebApp
dotnet run
```

#### 6. Документация API (Swagger)

Откройте браузер и перейдите по адресу:

`https://localhost:<port>/swagger`

для интерактивного тестирования эндпоинтов.

### Вариант 2: Запуск с помощью Docker Compose

Если в корне проекта настроены `Dockerfile` и `docker-compose.yml`, вы
можете запустить приложение вместе с базой данных одной командой:

``` bash
docker-compose up --build
```

## 📌 Автор и лицензия

Проект разработан в рамках учебной и практической разработки надежных
серверных приложений на .NET Core с использованием передовых паттернов
проектирования.
