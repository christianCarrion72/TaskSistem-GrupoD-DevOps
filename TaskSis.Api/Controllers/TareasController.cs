using Microsoft.AspNetCore.Mvc;
using TaskSis.Api.Domain.DTOs.Tarea;
using TaskSis.Api.Domain.Entities;
using TaskSis.Api.Infrastructure.Repositories;

namespace TaskSis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TareasController : ControllerBase
{
    private readonly ITareaRepository _repo;

    public TareasController(ITareaRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public ActionResult<List<TareaResponseDto>> GetAll()
    {
        var items = _repo.GetAll().Select(MapToDto).ToList();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public ActionResult<TareaResponseDto> GetById(int id)
    {
        var item = _repo.GetById(id);
        if (item is null) return NotFound();

        return Ok(MapToDto(item));
    }

    [HttpPost]
    public ActionResult<TareaResponseDto> Create([FromBody] TareaCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var created = _repo.Add(new Tarea
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion ?? string.Empty,
            Estado = dto.Estado
        });

        var result = MapToDto(created);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] TareaUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var ok = _repo.Update(new Tarea
        {
            Id = id,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion ?? string.Empty,
            Estado = dto.Estado
        });

        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var ok = _repo.Delete(id);
        return ok ? NoContent() : NotFound();
    }

    private static TareaResponseDto MapToDto(Tarea x) => new()
    {
        Id = x.Id,
        Nombre = x.Nombre,
        Descripcion = x.Descripcion,
        Estado = x.Estado
    };
}