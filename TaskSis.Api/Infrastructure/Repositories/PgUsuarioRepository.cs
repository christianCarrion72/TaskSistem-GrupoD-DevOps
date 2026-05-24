using Microsoft.EntityFrameworkCore;
using TaskSis.Api.Domain.Entities;
using TaskSis.Api.Infrastructure.Persistence;

namespace TaskSis.Api.Infrastructure.Repositories;

public class PgUsuarioRepository(AppDbContext db) : IUsuarioRepository
{
    public IReadOnlyList<Usuario> GetAll() =>
        db.Usuarios.AsNoTracking().ToList();

    public Usuario? GetById(int id) =>
        db.Usuarios.AsNoTracking().FirstOrDefault(u => u.Id == id);

    public Usuario Add(Usuario usuario)
    {
        db.Usuarios.Add(usuario);
        db.SaveChanges();
        return usuario;
    }
}
