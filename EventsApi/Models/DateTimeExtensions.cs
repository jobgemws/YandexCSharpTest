namespace EventsApi.Models;

/// <summary>
/// Приведение дат к UTC
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Приводит дату к UTC в зависимости от её <see cref="DateTime.Kind"/>.
    /// <para>
    /// <see cref="DateTime.ToUniversalTime"/> для <see cref="DateTimeKind.Unspecified"/>
    /// считает дату локальной для сервера, то есть результат зависит от часового пояса
    /// машины. Чтобы поведение было одинаковым на любом сервере, дата без указания
    /// пояса трактуется как UTC.
    /// </para>
    /// </summary>
    /// <param name="value">Дата для приведения</param>
    /// <returns>Дата с <see cref="DateTimeKind.Utc"/></returns>
    public static DateTime ToUtc(this DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
        _ => value,
    };
}
