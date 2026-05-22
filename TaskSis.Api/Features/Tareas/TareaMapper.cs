using Riok.Mapperly.Abstractions;
using TaskSis.Api.Domain.DTOs.Tarea;
using TaskSis.Api.Domain.Entities;
using TaskSis.Api.Infrastructure.Repositories;

namespace TaskSis.Api.Features.Tareas;

[Mapper]
public partial class TareaMapper
{
    private readonly IUsuarioRepository _usuarioRepo;

    public TareaMapper(IUsuarioRepository usuarioRepo)
    {
        _usuarioRepo = usuarioRepo;
    }

    public TareaResponseDto ToResponseDto(Tarea entity)
    {
        var dto = ToResponseDtoBase(entity);

        // Obtener nombres de usuarios
        var propietario = _usuarioRepo.GetById(entity.UsuarioId);
        var asignado = entity.UsuarioAsignadoId.HasValue
            ? _usuarioRepo.GetById(entity.UsuarioAsignadoId.Value)
            : null;

        return dto with
        {
            UsuarioPropietarioNombre = propietario?.Nombre ?? "Usuario desconocido",
            UsuarioAsignadoNombre = asignado?.Nombre
        };
    }

    public List<TareaResponseDto> ToResponseDtoList(List<Tarea> entities)
    {
        return entities.Select(ToResponseDto).ToList();
    }

    [MapperIgnoreTarget(nameof(TareaResponseDto.UsuarioPropietarioNombre))]
    [MapperIgnoreTarget(nameof(TareaResponseDto.UsuarioAsignadoNombre))]
    private partial TareaResponseDto ToResponseDtoBase(Tarea entity);

    [MapperIgnoreTarget(nameof(Tarea.Id))]
    public partial Tarea ToEntity(TareaCreateDto dto);

    [MapperIgnoreTarget(nameof(Tarea.Id))]
    [MapperIgnoreTarget(nameof(Tarea.UsuarioId))]
    public partial void MapToEntity(TareaUpdateDto dto, Tarea entity);
}
