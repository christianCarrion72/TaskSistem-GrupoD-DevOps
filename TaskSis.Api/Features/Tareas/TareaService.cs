using TaskSis.Api.Domain.DTOs.Common;
using TaskSis.Api.Domain.DTOs.Tarea;
using TaskSis.Api.Domain.Entities;
using TaskSis.Api.Domain.Validation;
using TaskSis.Api.Infrastructure.Repositories;

namespace TaskSis.Api.Features.Tareas;

public sealed class TareaService : ITareaService
{
    private readonly ITareaRepository _repo;
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly TareaMapper _mapper;

    public TareaService(ITareaRepository repo, IUsuarioRepository usuarioRepo, TareaMapper mapper)
    {
        _repo = repo;
        _usuarioRepo = usuarioRepo;
        _mapper = mapper;
    }

    public ServiceResponse<List<TareaResponseDto>> GetAll()
    {
        List<Tarea> items = _repo.GetAll().ToList();
        List<TareaResponseDto> dtos = _mapper.ToResponseDtoList(items);

        string message = dtos.Count == 0
            ? "No hay tareas registradas"
            : "Tareas obtenidas exitosamente";

        return ServiceResponse<List<TareaResponseDto>>.Ok(dtos, message);
    }

    public ServiceResponse<TareaResponseDto> GetById(int id)
    {
        Tarea? item = _repo.GetById(id);
        if (item is null) return ServiceResponse<TareaResponseDto>.NotFound($"Tarea no encontrada con id: {id}");

        return ServiceResponse<TareaResponseDto>.Ok(_mapper.ToResponseDto(item), "Tarea obtenida exitosamente");
    }

    public ServiceResponse<List<TareaResponseDto>> GetByUsuarioId(int usuarioId)
    {
        // Validar que el usuario existe
        if (_usuarioRepo.GetById(usuarioId) is null)
        {
            return ServiceResponse<List<TareaResponseDto>>.NotFound($"Usuario no encontrado con id: {usuarioId}");
        }

        List<Tarea> items = _repo.GetAll()
            .Where(t => t.UsuarioId == usuarioId)
            .ToList();

        List<TareaResponseDto> dtos = _mapper.ToResponseDtoList(items);

        string message = dtos.Count == 0
            ? $"No hay tareas para el usuario con id: {usuarioId}"
            : "Tareas obtenidas exitosamente";

        return ServiceResponse<List<TareaResponseDto>>.Ok(dtos, message);
    }

    public ServiceResponse<TareaResponseDto> Create(TareaCreateDto dto)
    {
        IReadOnlyList<ValidationErrorDto> errors = DtoValidation.Validate(dto);
        if (errors.Count > 0)
        {
            return ServiceResponse<TareaResponseDto>.Unprocessable(errors, "Fallo en la validación de tarea.");
        }

        // Validar usuario (se omite si el id es 0, considerado usuario sistema)
        if (dto.UsuarioId != 0 && _usuarioRepo.GetById(dto.UsuarioId) is null)
        {
            return ServiceResponse<TareaResponseDto>.NotFound($"Usuario propietario no encontrado con id: {dto.UsuarioId}");
        }

        // Validar que el usuario asignado existe
        if (dto.UsuarioAsignadoId.HasValue && _usuarioRepo.GetById(dto.UsuarioAsignadoId.Value) is null)
        {
            return ServiceResponse<TareaResponseDto>.NotFound($"Usuario asignado no encontrado con id: {dto.UsuarioAsignadoId}");
        }

        TareaCreateDto normalizedDto = dto with { Descripcion = dto.Descripcion ?? string.Empty };
        Tarea entity = _mapper.ToEntity(normalizedDto);

        Tarea created = _repo.Add(entity);
        return ServiceResponse<TareaResponseDto>.Created(_mapper.ToResponseDto(created), "Tarea creada exitosamente");
    }

    public ServiceResponse<TareaResponseDto> Update(int id, TareaUpdateDto dto)
    {
        IReadOnlyList<ValidationErrorDto> errors = DtoValidation.Validate(dto);
        if (errors.Count > 0)
        {
            return ServiceResponse<TareaResponseDto>.Unprocessable(errors, "Fallo en la validación de tarea.");
        }

        Tarea? existing = _repo.GetById(id);
        if (existing is null) return ServiceResponse<TareaResponseDto>.NotFound("Tarea no encontrada");

        // Validar que el usuario asignado existe
        if (dto.UsuarioAsignadoId.HasValue && _usuarioRepo.GetById(dto.UsuarioAsignadoId.Value) is null)
        {
            return ServiceResponse<TareaResponseDto>.NotFound($"Usuario asignado no encontrado con id: {dto.UsuarioAsignadoId}");
        }

        TareaUpdateDto normalizedDto = dto with { Descripcion = dto.Descripcion ?? string.Empty };
        _mapper.MapToEntity(normalizedDto, existing);

        bool ok = _repo.Update(existing);
        if (!ok) return ServiceResponse<TareaResponseDto>.NotFound("Tarea no encontrada");

        Tarea updated = _repo.GetById(id)!;
        return ServiceResponse<TareaResponseDto>.Ok(_mapper.ToResponseDto(updated), "Tarea actualizada exitosamente");
    }

    public ServiceResponse<string> Delete(int id)
    {
        Tarea? existing = _repo.GetById(id);
        if (existing is null) return ServiceResponse<string>.NotFound($"Tarea no encontrada con id: {id}");

        _repo.Delete(id);
        return ServiceResponse<string>.Ok("Tarea eliminada exitosamente", "Tarea eliminada exitosamente");
    }
}
