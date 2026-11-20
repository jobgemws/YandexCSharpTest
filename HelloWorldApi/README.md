# HelloWorldApi

Простой REST API для управления событиями, реализованный на ASP.NET Core (minimal API). Приложение хранит события в памяти (без базы данных) и предоставляет Swagger UI для интерактивного тестирования.

## Требования

- [.NET SDK](https://dotnet.microsoft.com/download) 10.0 или новее

## Запуск

```bash
cd HelloWorldApi
dotnet run
```

Приложение запустится на адресе из `Properties/launchSettings.json` — по умолчанию `http://localhost:5198`.

После запуска:

| Ресурс | Адрес |
|---|---|
| Swagger UI | http://localhost:5198/swagger |
| Swagger JSON (OpenAPI-спецификация) | http://localhost:5198/swagger/v1/swagger.json |

## Модель события `Event`

| Поле | Тип | Обязательное | Описание |
|---|---|---|---|
| `id` | `Guid` | да (генерируется автоматически) | Уникальный идентификатор |
| `title` | `string` | да | Название события (до 255 символов) |
| `description` | `string` | нет | Описание события |
| `startAt` | `DateTime` | да | Время начала |
| `endAt` | `DateTime` | да | Время окончания (должно быть позже `startAt`) |

> Дата и время указываются в формате ISO 8601, например `2026-11-17T10:00:00`.

## API

Базовый адрес: `/events`.

### GET /events

Получить список всех событий.

Ответ: `200 OK`, массив событий.

```bash
curl http://localhost:5198/events
```

### GET /events/{id}

Получить событие по идентификатору.

Ответы:
- `200 OK` — событие найдено;
- `404 Not Found` — событие не найдено.

```bash
curl http://localhost:5198/events/002d2439-4516-4aff-aba8-5069eb9110a7
```

### POST /events

Создать новое событие. `id` присваивается автоматически.

Ответы:
- `201 Created` — событие создано (в ответе событие с его `id`, заголовок `Location`);
- `400 Bad Request` — ошибка валидации (пропущено обязательное поле, `title` длиннее 255 символов или `endAt` не позже `startAt`).

```bash
curl -X POST http://localhost:5198/events \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Дейлик",
    "description": "Ежедневная планёрка",
    "startAt": "2026-11-17T11:00:00",
    "endAt": "2026-11-17T12:00:00"
  }'
```

### PUT /events/{id}

Полностью обновить событие (все поля обязательны, как и при создании).

Ответы:
- `200 OK` — событие обновлено;
- `404 Not Found` — событие не найдено;
- `400 Bad Request` — ошибка валидации.

```bash
curl -X PUT http://localhost:5198/events/002d2439-4516-4aff-aba8-5069eb9110a7 \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Обновлённый дейлик",
    "startAt": "2026-11-17T09:00:00",
    "endAt": "2026-11-17T10:00:00"
  }'
```

### DELETE /events/{id}

Удалить событие.

Ответы:
- `204 No Content` — событие удалено;
- `404 Not Found` — событие не найдено.

```bash
curl -X DELETE http://localhost:5198/events/002d2439-4516-4aff-aba8-5069eb9110a7
```

## Обработка ошибок

Все ошибки (400, 404, 500 и т.д.) возвращаются в едином формате `ProblemDetails` (RFC 9110, `Content-Type: application/problem+json`). За это отвечают `builder.Services.AddProblemDetails()`, `app.UseExceptionHandler()` (необработанные исключения → 500) и `app.UseStatusCodePages()` (голые статус-коды, например `404 Not Found`, → `ProblemDetails`) в `Program.cs`.

Пример `404 Not Found`:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "00-...-00"
}
```

## Валидация

Некорректные входные данные возвращают `400 Bad Request` с телом в формате `ProblemDetails` (RFC 9110), например:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Один или несколько параметров не прошли валидацию!",
  "status": 400,
  "errors": {
    "EndAt": ["Поле EndAt должно быть позже StartAt."]
  }
}
```

Правила:
- `title`, `startAt`, `endAt` — обязательные;
- `title` — строка не длиннее 255 символов;
- `endAt` должно быть позже `startAt`.

## Структура проекта

| Путь | Назначение |
|---|---|
| `Program.cs` | Точка входа, настройка DI и Swagger, подключение эндпоинтов |
| `Models/Event.cs` | Доменная модель события |
| `Models/EventDto.cs` | Входная модель (запрос) с правилами валидации |
| `Models/EventResponse.cs` | Модель ответа API и маппинг из доменной модели |
| `Services/IEventService.cs` | Интерфейс сервиса событий |
| `Services/EventService.cs` | Реализация сервиса (хранилище в памяти) |
| `Endpoints/EventEndpoints.cs` | REST-эндпоинты (`/events`) |
| `Filters/ValidationFilter.cs` | Фильтр валидации входных моделей |
| `Program.cs` → `AddProblemDetails`/`UseExceptionHandler`/`UseStatusCodePages` | Централизованная обработка ошибок в формате `ProblemDetails` |

> Хранение реализовано в памяти. После перезапуска приложения все события очищаются.
