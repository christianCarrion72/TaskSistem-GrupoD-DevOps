using System.ComponentModel.DataAnnotations;
using TaskSis.Api.Domain.Enums;

namespace TaskSis.Api.Domain.DTOs.Tarea;

public sealed record TareaUpdateDto
{
    [Required]
    [MinLength(2)]
    public string Nombre { get; init; } = string.Empty;

    [MaxLength(200)]
    public string? Descripcion { get; init; }

    public EstadoTarea Estado { get; init; }

    public int? UsuarioAsignadoId { get; init; }
}
