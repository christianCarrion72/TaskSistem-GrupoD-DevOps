using Microsoft.AspNetCore.Mvc;
using TaskSis.Api.Domain.DTOs.Common;
using TaskSis.Api.Domain.DTOs.Usuario;
using TaskSis.Api.Features.Usuarios;

namespace TaskSis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Respond(_service.GetAll());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        return Respond(_service.GetById(id));
    }

    [HttpPost]
    public IActionResult Create([FromBody] UsuarioCreateDto dto)
    {
        if (!ModelState.IsValid) return UnprocessableFromModelState<UsuarioResponseDto>();

        ServiceResponse<UsuarioResponseDto> response = _service.Create(dto);

        if (response.StatusCode == 201 && response.Body.Datos is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = response.Body.Datos.Id }, response.Body);
        }

        return Respond(response);
    }

    private IActionResult Respond<T>(ServiceResponse<T> response) =>
        StatusCode(response.StatusCode, response.Body);

    private IActionResult UnprocessableFromModelState<T>()
    {
        List<ValidationErrorDto> errors = ModelState
            .SelectMany(kvp =>
                kvp.Value?.Errors.Select(e => new ValidationErrorDto(kvp.Key, e.ErrorMessage))
                ?? Enumerable.Empty<ValidationErrorDto>())
            .ToList();

        return StatusCode(
            422,
            new ApiResponse<T>(
                Exito: false,
                Mensaje: "Fallo en la validación de usuario.",
                Datos: default,
                Errores: errors));
    }
}
