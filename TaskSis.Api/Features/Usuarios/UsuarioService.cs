using TaskSis.Api.Domain.DTOs.Common;
using TaskSis.Api.Domain.DTOs.Usuario;
using TaskSis.Api.Domain.Entities;
using TaskSis.Api.Domain.Validation;
using TaskSis.Api.Infrastructure.Repositories;

namespace TaskSis.Api.Features.Usuarios;

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;
    private readonly UsuarioMapper _mapper;

    public UsuarioService(IUsuarioRepository repo, UsuarioMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public ServiceResponse<List<UsuarioResponseDto>> GetAll()
    {
        List<Usuario> items = _repo.GetAll().ToList();
        List<UsuarioResponseDto> dtos = _mapper.ToResponseDtoList(items);

        string message = dtos.Count == 0
            ? "No hay usuarios registrados"
            : "Usuarios obtenidos exitosamente";

        return ServiceResponse<List<UsuarioResponseDto>>.Ok(dtos, message);
    }

    public ServiceResponse<UsuarioResponseDto> GetById(int id)
    {
        Usuario? item = _repo.GetById(id);
        if (item is null) return ServiceResponse<UsuarioResponseDto>.NotFound($"Usuario no encontrado con id: {id}");

        return ServiceResponse<UsuarioResponseDto>.Ok(_mapper.ToResponseDto(item), "Usuario obtenido exitosamente");
    }

    public ServiceResponse<UsuarioResponseDto> Create(UsuarioCreateDto dto)
    {
        IReadOnlyList<ValidationErrorDto> errors = DtoValidation.Validate(dto);
        if (errors.Count > 0)
        {
            return ServiceResponse<UsuarioResponseDto>.Unprocessable(errors, "Fallo en la validación de usuario.");
        }

        Usuario entity = _mapper.ToEntity(dto);
        Usuario created = _repo.Add(entity);

        return ServiceResponse<UsuarioResponseDto>.Created(_mapper.ToResponseDto(created), "Usuario creado exitosamente");
    }
}
