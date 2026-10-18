// GolBet.Services/Helpers/DateTimeExtensions.cs
namespace GolBet.Services.Helpers;

/// <summary>
/// Conversiones de Colombia local <-> UTC para la capa de servicios.
/// </summary>
public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo ColombiaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");

    /// <summary>UTC (base de datos) -> Colombia local (para el formulario de edición).</summary>
    public static DateTime ToColombiaTime(this DateTime utc)
        => TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utc, DateTimeKind.Utc), ColombiaZone);

    /// <summary>Colombia local (formulario) -> UTC (para guardar en la base de datos).</summary>
    public static DateTime ToUtcFromColombia(this DateTime colombiaLocal)
        => TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(colombiaLocal, DateTimeKind.Unspecified), ColombiaZone);
}