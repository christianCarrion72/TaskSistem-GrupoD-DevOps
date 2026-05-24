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

    public bool Update(Usuario usuario)
    {
        Usuario? existing = db.Usuarios.FirstOrDefault(u => u.Id == usuario.Id);
        if (existing is null) return false;

        existing.Nombre = usuario.Nombre;
        existing.Correo = usuario.Correo;
        db.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        Usuario? existing = db.Usuarios.FirstOrDefault(u => u.Id == id);
        if (existing is null) return false;

        db.Usuarios.Remove(existing);
        db.SaveChanges();
        return true;
    }
}
