namespace HelloWorldApi.Models;

/// <summary>
/// Модель события, возвращаемая в ответах API
/// </summary>
public class EventResponse
{
    /// <summary>
    /// Уникальный идентификатор события
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// Название события
    /// </summary>
    /// <example>Ежедневный дейлик</example>
    public required string Title { get; set; }

    /// <summary>
    /// Описание события (необязательное)
    /// </summary>
    /// <example>Обсуждение текущих задач</example>
    public string? Description { get; set; }

    /// <summary>
    /// Время начала события
    /// </summary>
    /// <example>2026-11-17T11:00:00</example>
    public DateTime StartAt { get; set; }

    /// <summary>
    /// Время окончания события
    /// </summary>
    /// <example>2026-11-17T12:00:00</example>
    public DateTime EndAt { get; set; }

    /// <summary>
    /// Создаёт модель ответа из доменной модели события
    /// </summary>
    /// <param name="event">Доменная модель события</param>
    public static EventResponse FromEvent(Event @event) => new()
    {
        Id = @event.Id,
        Title = @event.Title,
        Description = @event.Description,
        StartAt = @event.StartAt,
        EndAt = @event.EndAt,
    };
}
