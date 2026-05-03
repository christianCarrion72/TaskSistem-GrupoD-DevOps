using System.ComponentModel.DataAnnotations;
using TaskSis.Api.Domain.Enums;

namespace TaskSis.Api.Domain.DTOs.Tarea;

public class TareaUpdateDto
{
    [Required]
    [MinLength(2)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Descripcion { get; set; }

    public EstadoTarea Estado { get; set; }
}