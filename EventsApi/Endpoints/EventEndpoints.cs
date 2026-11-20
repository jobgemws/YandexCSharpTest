using EventsApi.Filters;
using EventsApi.Models;
using EventsApi.Services;

namespace EventsApi.Endpoints;

/// <summary>
/// Регистрация REST-эндпоинтов для работы с событиями
/// </summary>
public static class EventEndpoints
{
    /// <summary>
    /// Регистрирует эндпоинты событий по маршруту <c>/events</c>
    /// </summary>
    /// <param name="app">Строитель маршрутов приложения</param>
    public static void MapEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/events");

        group.MapGet("/", GetAllEvents)
            .Produces<IEnumerable<EventResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetEventById)
            .Produces<EventResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateEvent)
            .AddEndpointFilter<ValidationFilter<EventDto>>()
            .Produces<EventResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateEvent)
            .AddEndpointFilter<ValidationFilter<EventDto>>()
            .Produces<EventResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapDelete("/{id:guid}", DeleteEvent)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Возвращает список всех событий
    /// </summary>
    /// <param name="service">Сервис событий</param>
    /// <param name="ct">Токен отмены запроса</param>
    /// <returns>HTTP 200 со списком событий</returns>
    private static async Task<IResult> GetAllEvents(IEventService service, CancellationToken ct)
    {
        var events = await service.GetAllAsync(ct);
        return Results.Ok(events.Select(EventResponse.FromEvent));
    }

    /// <summary>
    /// Возвращает событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события (GUID)</param>
    /// <param name="service">Сервис событий</param>
    /// <param name="ct">Токен отмены запроса</param>
    /// <returns>HTTP 200 с событием, либо HTTP 404, если событие не найдено</returns>
    private static async Task<IResult> GetEventById(Guid id, IEventService service, CancellationToken ct)
    {
        var @event = await service.GetByIdAsync(id, ct);
        return @event is null ? Results.NotFound() : Results.Ok(EventResponse.FromEvent(@event));
    }

    /// <summary>
    /// Создаёт новое событие. Идентификатор присваивается автоматически
    /// </summary>
    /// <param name="dto">Данные события для создания</param>
    /// <param name="service">Сервис событий</param>
    /// <param name="ct">Токен отмены запроса</param>
    /// <returns>HTTP 201 с созданным событием, либо HTTP 400 при ошибке валидации</returns>
    private static async Task<IResult> CreateEvent(EventDto dto, IEventService service, CancellationToken ct)
    {
        var created = await service.CreateAsync(dto, ct);
        return Results.Created($"/events/{created.Id}", EventResponse.FromEvent(created));
    }

    /// <summary>
    /// Полностью обновляет событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события (GUID)</param>
    /// <param name="dto">Новые данные события</param>
    /// <param name="service">Сервис событий</param>
    /// <param name="ct">Токен отмены запроса</param>
    /// <returns>HTTP 200 с обновлённым событием, HTTP 404, если событие не найдено, либо HTTP 400 при ошибке валидации</returns>
    private static async Task<IResult> UpdateEvent(Guid id, EventDto dto, IEventService service, CancellationToken ct)
    {
        var updated = await service.UpdateAsync(id, dto, ct);
        return updated is null ? Results.NotFound() : Results.Ok(EventResponse.FromEvent(updated));
    }

    /// <summary>
    /// Удаляет событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события (GUID)</param>
    /// <param name="service">Сервис событий</param>
    /// <param name="ct">Токен отмены запроса</param>
    /// <returns>HTTP 204 при успешном удалении, либо HTTP 404, если событие не найдено</returns>
    private static async Task<IResult> DeleteEvent(Guid id, IEventService service, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
