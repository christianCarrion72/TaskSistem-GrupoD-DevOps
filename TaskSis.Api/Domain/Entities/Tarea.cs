using TaskSis.Api.Domain.Enums;

namespace TaskSis.Api.Domain.Entities;

public class Tarea
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;

    // Relación con Usuario
    public int UsuarioId { get; set; }
    public int? UsuarioAsignadoId { get; set; }
}