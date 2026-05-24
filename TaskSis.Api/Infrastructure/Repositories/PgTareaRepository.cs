using Microsoft.EntityFrameworkCore;
using TaskSis.Api.Domain.Entities;
using TaskSis.Api.Infrastructure.Persistence;

namespace TaskSis.Api.Infrastructure.Repositories;

public class PgTareaRepository(AppDbContext db) : ITareaRepository
{
    public IReadOnlyList<Tarea> GetAll() =>
        db.Tareas.AsNoTracking().ToList();

    public Tarea? GetById(int id) =>
        db.Tareas.AsNoTracking().FirstOrDefault(t => t.Id == id);

    public Tarea Add(Tarea tarea)
    {
        db.Tareas.Add(tarea);
        db.SaveChanges();
        return tarea;
    }

    public bool Update(Tarea tarea)
    {
        Tarea? existing = db.Tareas.Find(tarea.Id);
        if (existing is null) return false;

        existing.Nombre = tarea.Nombre;
        existing.Descripcion = tarea.Descripcion;
        existing.Estado = tarea.Estado;
        existing.UsuarioAsignadoId = tarea.UsuarioAsignadoId;
        db.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        Tarea? existing = db.Tareas.Find(id);
        if (existing is null) return false;

        db.Tareas.Remove(existing);
        db.SaveChanges();
        return true;
    }
}
