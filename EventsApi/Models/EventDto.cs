using System.ComponentModel.DataAnnotations;

namespace EventsApi.Models;

/// <summary>
/// Входная модель события с правилами валидации
/// </summary>
public class EventDto : IValidatableObject
{
    /// <summary>
    /// Название события. Обязательное поле
    /// </summary>
    /// <example>Ежедневный дейлик</example>
    [Required(ErrorMessage = "Поле Title обязательно для заполнения!")]
    [StringLength(255, ErrorMessage = "Поле Title не должно превышать 255 символов!")]
    public string? Title { get; set; }

    /// <summary>
    /// Описание события (необязательное)
    /// </summary>
    /// <example>Обсуждение текущих задач</example>
    public string? Description { get; set; }

    /// <summary>
    /// Время начала события. Обязательное поле. Должно содержать часовой пояс:
    /// смещение (например, <c>2026-11-17T14:00:00+03:00</c>) или суффикс <c>Z</c>
    /// </summary>
    /// <example>2026-11-17T11:00:00Z</example>
    [Required(ErrorMessage = "Поле StartAt обязательно для заполнения!")]
    public DateTime? StartAt { get; set; }

    /// <summary>
    /// Время окончания события. Обязательное поле, должно быть позже времени начала.
    /// Должно содержать часовой пояс: смещение или суффикс <c>Z</c>
    /// </summary>
    /// <example>2026-11-17T12:00:00Z</example>
    [Required(ErrorMessage = "Поле EndAt обязательно для заполнения!")]
    public DateTime? EndAt { get; set; }

    /// <summary>
    /// Проверяет, что даты содержат часовой пояс и что время окончания позже времени начала.
    /// Сравнение выполняется в UTC
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasUnspecifiedKind = false;

        if (StartAt is { Kind: DateTimeKind.Unspecified })
        {
            hasUnspecifiedKind = true;
            yield return NoTimeZone(nameof(StartAt));
        }

        if (EndAt is { Kind: DateTimeKind.Unspecified })
        {
            hasUnspecifiedKind = true;
            yield return NoTimeZone(nameof(EndAt));
        }

        // Наивные даты (без пояса) не сравниваем: результат такого сравнения недостоверен,
        // а пользователь уже получил ошибку об отсутствии часового пояса
        if (hasUnspecifiedKind || StartAt is not { } startAt || EndAt is not { } endAt)
        {
            yield break;
        }

        if (endAt.ToUtc() <= startAt.ToUtc())
        {
            yield return new ValidationResult(
                "Поле EndAt должно быть позже StartAt!",
                new[] { nameof(EndAt) });
        }
    }

    /// <summary>
    /// Ошибка валидации: в дате не указан часовой пояс
    /// </summary>
    private static ValidationResult NoTimeZone(string memberName) => new(
        $"Поле {memberName} должно содержать часовой пояс: укажите смещение " +
        "(например, 2026-11-17T14:00:00+03:00) или суффикс Z (2026-11-17T11:00:00Z)!",
        new[] { memberName });
}
