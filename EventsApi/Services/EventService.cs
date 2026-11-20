using System.Collections.Concurrent;
using EventsApi.Models;

namespace EventsApi.Services;

/// <summary>
/// Реализация сервиса событий с хранением в памяти
/// </summary>
public class EventService : IEventService
{
    private readonly ConcurrentDictionary<Guid, Event> _events = new();

    /// <summary>
    /// Возвращает список всех событий, отсортированный по времени начала
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список событий</returns>
    public Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<Event> events = _events.Values
            .OrderBy(e => e.StartAt.ToUtc())
            .ToList();

        return Task.FromResult(events);
    }

    /// <summary>
    /// Возвращает событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Событие либо <see langword="null"/>, если оно не найдено</returns>
    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _events.TryGetValue(id, out var @event);
        return Task.FromResult(@event);
    }

    /// <summary>
    /// Создаёт новое событие на основе входных данных. Идентификатор генерируется сервисом
    /// </summary>
    /// <param name="dto">Данные события для создания</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Созданное событие</returns>
    public Task<Event> CreateAsync(EventDto dto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var @event = new Event
        {
            Id = Guid.NewGuid(),
            Title = dto.Title!,
            Description = dto.Description,
            StartAt = dto.StartAt!.Value.ToUtc(),
            EndAt = dto.EndAt!.Value.ToUtc(),
        };

        _events[@event.Id] = @event;

        return Task.FromResult(@event);
    }

    /// <summary>
    /// Полностью обновляет существующее событие
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <param name="dto">Новые данные события</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновлённое событие либо <see langword="null"/>, если оно не найдено</returns>
    public Task<Event?> UpdateAsync(Guid id, EventDto dto, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_events.TryGetValue(id, out var existing))
            {
                return Task.FromResult<Event?>(null);
            }

            var @event = new Event
            {
                Id = id,
                Title = dto.Title!,
                Description = dto.Description,
                StartAt = dto.StartAt!.Value.ToUtc(),
                EndAt = dto.EndAt!.Value.ToUtc(),
            };

            // TryUpdate сравнивает текущее значение со ссылкой existing: если событие
            // удалили или заменили между TryGetValue и TryUpdate, запись не произойдёт
            // и мы повторим попытку с актуальным состоянием словаря
            if (_events.TryUpdate(id, @event, existing))
            {
                return Task.FromResult<Event?>(@event);
            }
        }
    }

    /// <summary>
    /// Удаляет событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns><see langword="true"/>, если событие было удалено, иначе <see langword="false"/></returns>
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_events.TryRemove(id, out _));
    }
}
