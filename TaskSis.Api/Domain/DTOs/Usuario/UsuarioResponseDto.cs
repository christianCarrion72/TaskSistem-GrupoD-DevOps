namespace TaskSis.Api.Domain.DTOs.Usuario;

public record UsuarioResponseDto(
    int Id,
    string Nombre,
    string Correo,
    DateTime FechaCreacion
);
