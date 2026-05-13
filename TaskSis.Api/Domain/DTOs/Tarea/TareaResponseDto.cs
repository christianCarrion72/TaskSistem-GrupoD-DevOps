using TaskSis.Api.Domain.Enums;

namespace TaskSis.Api.Domain.DTOs.Tarea;

public sealed record TareaResponseDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public EstadoTarea Estado { get; init; }
}
