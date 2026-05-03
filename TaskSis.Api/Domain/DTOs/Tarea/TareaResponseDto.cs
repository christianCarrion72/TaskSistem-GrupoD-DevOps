using TaskSis.Api.Domain.Enums;

namespace TaskSis.Api.Domain.DTOs.Tarea;

public class TareaResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public EstadoTarea Estado { get; set; }
}
