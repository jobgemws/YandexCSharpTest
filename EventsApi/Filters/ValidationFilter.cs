using System.ComponentModel.DataAnnotations;

namespace EventsApi.Filters;

/// <summary>
/// Фильтр, выполняющий валидацию входной модели перед выполнением эндпоинта
/// </summary>
/// <typeparam name="T">Тип входной модели, подлежащей валидации</typeparam>
public class ValidationFilter<T> : IEndpointFilter
{
    /// <summary>
    /// Проверяет входную модель через <see cref="Validator"/> и при ошибках возвращает 400, иначе передаёт выполнение следующему делегату
    /// </summary>
    /// <param name="context">Контекст вызова эндпоинта</param>
    /// <param name="next">Следующий делегат в цепочке</param>
    /// <returns>Результат выполнения эндпоинта, либо 400 при ошибках валидации</returns>
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var dto = context.Arguments.OfType<T>().SingleOrDefault();

        if (dto is null)
        {
            return Results.BadRequest(new { error = $"Требуется тело запроса типа {typeof(T).Name}!" });
        }

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto);

        if (!Validator.TryValidateObject(dto, validationContext, validationResults, validateAllProperties: true))
        {
            var errors = validationResults
                .GroupBy(r => r.MemberNames.FirstOrDefault() ?? typeof(T).Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => r.ErrorMessage ?? "Некорректное значение!").ToArray());

            return Results.ValidationProblem(errors, title: "Один или несколько параметров не прошли валидацию!");
        }

        return await next(context);
    }
}
