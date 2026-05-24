using TaskSis.Api.Domain.DTOs.Common;
using TaskSis.Api.Domain.DTOs.Usuario;

namespace TaskSis.Api.Features.Usuarios;

public interface IUsuarioService
{
    ServiceResponse<List<UsuarioResponseDto>> GetAll();
    ServiceResponse<UsuarioResponseDto> GetById(int id);
    ServiceResponse<UsuarioResponseDto> Create(UsuarioCreateDto dto);
}
