using System.ComponentModel.DataAnnotations;

namespace TaskSis.Api.Domain.DTOs.Usuario;

public record UsuarioUpdateDto(
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    string Nombre,

    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres")]
    string Correo
);
