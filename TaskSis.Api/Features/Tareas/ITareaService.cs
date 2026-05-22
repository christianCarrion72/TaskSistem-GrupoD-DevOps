using TaskSis.Api.Domain.DTOs.Common;
using TaskSis.Api.Domain.DTOs.Tarea;

namespace TaskSis.Api.Features.Tareas;

public interface ITareaService
{
    ServiceResponse<List<TareaResponseDto>> GetAll();
    ServiceResponse<TareaResponseDto> GetById(int id);
    ServiceResponse<List<TareaResponseDto>> GetByUsuarioId(int usuarioId);
    ServiceResponse<TareaResponseDto> Create(TareaCreateDto dto);
    ServiceResponse<TareaResponseDto> Update(int id, TareaUpdateDto dto);
    ServiceResponse<string> Delete(int id);
}
