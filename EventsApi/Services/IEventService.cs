using EventsApi.Models;

namespace EventsApi.Services;

/// <summary>
/// Контракт сервиса для работы с событиями
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Возвращает список всех событий
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список событий</returns>
    Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Событие либо <see langword="null"/>, если оно не найдено</returns>
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новое событие на основе входных данных. Идентификатор генерируется сервисом
    /// </summary>
    /// <param name="dto">Данные события для создания</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Созданное событие</returns>
    Task<Event> CreateAsync(EventDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Полностью обновляет существующее событие
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <param name="dto">Новые данные события</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновлённое событие либо <see langword="null"/>, если оно не найдено</returns>
    Task<Event?> UpdateAsync(Guid id, EventDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns><see langword="true"/>, если событие было удалено; иначе <see langword="false"/></returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
