namespace TaskSis.Api.Domain.DTOs.Common;

public sealed record ValidationErrorDto(string Campo, string Mensaje);

public sealed record ApiResponse<T>(bool Exito, string Mensaje, T? Datos = default, IReadOnlyList<ValidationErrorDto>? Errores = null);

public sealed record ServiceResponse<T>(int StatusCode, ApiResponse<T> Body)
{
    public static ServiceResponse<T> Ok(T data, string message) =>
        new(200, new ApiResponse<T>(true, message, data));

    public static ServiceResponse<T> Created(T data, string message) =>
        new(201, new ApiResponse<T>(true, message, data));

    public static ServiceResponse<T> NotFound(string message) =>
        new(404, new ApiResponse<T>(false, message));

    public static ServiceResponse<T> Unprocessable(IReadOnlyList<ValidationErrorDto> errors, string message) =>
        new(422, new ApiResponse<T>(false, message, default, errors));
}
