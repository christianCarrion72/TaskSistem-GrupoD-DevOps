using Riok.Mapperly.Abstractions;
using TaskSis.Api.Domain.DTOs.Tarea;
using TaskSis.Api.Domain.Entities;

namespace TaskSis.Api.Features.Tareas;

[Mapper]
public partial class TareaMapper
{
    public partial TareaResponseDto ToResponseDto(Tarea entity);
    public partial List<TareaResponseDto> ToResponseDtoList(List<Tarea> entities);

    [MapperIgnoreTarget(nameof(Tarea.Id))]
    public partial Tarea ToEntity(TareaCreateDto dto);

    [MapperIgnoreTarget(nameof(Tarea.Id))]
    public partial void MapToEntity(TareaUpdateDto dto, Tarea entity);
}
