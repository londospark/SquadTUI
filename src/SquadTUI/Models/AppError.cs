using LanguageExt;

namespace SquadTUI.Models;

public abstract record AppError(string Message);
public record FileNotFoundError(string Path) : AppError($"File not found: {Path}");
public record ParseError(string File, string Details) : AppError($"Parse error in {File}: {Details}");
public record ServiceError(string Service, string Details) : AppError($"{Service} failed: {Details}");
public record NoDataError(string DataType) : AppError($"No {DataType} data available");

public static class EitherExtensions
{
    public static IReadOnlyList<T> GetOrEmpty<T>(this Either<AppError, IReadOnlyList<T>> either) =>
        either.Match(Right: x => x, Left: _ => []);
}
