using TaskSis.Api.Domain.Entities;

namespace TaskSis.Api.Infrastructure.Repositories;

public interface IUsuarioRepository
{
    IReadOnlyList<Usuario> GetAll();
    Usuario? GetById(int id);
    Usuario Add(Usuario usuario);
}
