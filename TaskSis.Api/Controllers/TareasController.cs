using Microsoft.AspNetCore.Mvc;
using TaskSis.Api.Domain.DTOs.Common;
using TaskSis.Api.Domain.DTOs.Tarea;
using TaskSis.Api.Features.Tareas;

namespace TaskSis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TareasController : ControllerBase
{
    private readonly ITareaService _service;

    public TareasController(ITareaService service)
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
    public IActionResult Create([FromBody] TareaCreateDto dto)
    {
        if (!ModelState.IsValid) return UnprocessableFromModelState<TareaResponseDto>();

        ServiceResponse<TareaResponseDto> response = _service.Create(dto);

        if (response.StatusCode == 201 && response.Body.Datos is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = response.Body.Datos.Id }, response.Body);
        }

        return Respond(response);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] TareaUpdateDto dto)
    {
        if (!ModelState.IsValid) return UnprocessableFromModelState<TareaResponseDto>();

        return Respond(_service.Update(id, dto));
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return Respond(_service.Delete(id));
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
                Mensaje: "Fallo en la validación de tarea.",
                Datos: default,
                Errores: errors));
    }
}
