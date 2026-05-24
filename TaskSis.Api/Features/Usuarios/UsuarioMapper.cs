using Riok.Mapperly.Abstractions;
using TaskSis.Api.Domain.DTOs.Usuario;
using TaskSis.Api.Domain.Entities;

namespace TaskSis.Api.Features.Usuarios;

[Mapper]
public partial class UsuarioMapper
{
    public partial UsuarioResponseDto ToResponseDto(Usuario entity);
    public partial List<UsuarioResponseDto> ToResponseDtoList(List<Usuario> entities);

    [MapperIgnoreTarget(nameof(Usuario.Id))]
    [MapperIgnoreTarget(nameof(Usuario.FechaCreacion))]
    public partial Usuario ToEntity(UsuarioCreateDto dto);
}
