using System.ComponentModel.DataAnnotations;

namespace HelloWorldApi.Models;

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
    /// Время начала события. Обязательное поле
    /// </summary>
    /// <example>2026-11-17T11:00:00</example>
    [Required(ErrorMessage = "Поле StartAt обязательно для заполнения!")]
    public DateTime? StartAt { get; set; }

    /// <summary>
    /// Время окончания события. Обязательное поле, должно быть позже времени начала
    /// </summary>
    /// <example>2026-11-17T12:00:00</example>
    [Required(ErrorMessage = "Поле EndAt обязательно для заполнения!")]
    public DateTime? EndAt { get; set; }

    /// <summary>
    /// Проверяет, что время окончания позже времени начала
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartAt.HasValue && EndAt.HasValue && EndAt <= StartAt)
        {
            yield return new ValidationResult(
                "Поле EndAt должно быть позже StartAt!",
                new[] { nameof(EndAt) });
        }
    }
}
