using System.ComponentModel.DataAnnotations;

namespace TaskSis.Api.Domain.DTOs.Usuario;

public record UsuarioCreateDto(
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    string Nombre
);
