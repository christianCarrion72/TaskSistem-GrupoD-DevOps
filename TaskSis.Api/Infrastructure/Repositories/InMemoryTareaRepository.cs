using TaskSis.Api.Domain.Entities;

namespace TaskSis.Api.Infrastructure.Repositories;

public class InMemoryTareaRepository : ITareaRepository
{
    private readonly List<Tarea> _items = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public IReadOnlyList<Tarea> GetAll()
    {
        lock (_lock)
        {
            return _items.Select(Clone).ToList();
        }
    }

    public Tarea? GetById(int id)
    {
        lock (_lock)
        {
            Tarea? item = _items.FirstOrDefault(x => x.Id == id);
            return item is null ? null : Clone(item);
        }
    }

    public Tarea Add(Tarea tarea)
    {
        lock (_lock)
        {
            Tarea entity = new()
            {
                Id = _nextId++,
                Nombre = tarea.Nombre,
                Descripcion = tarea.Descripcion,
                Estado = tarea.Estado,
                UsuarioId = tarea.UsuarioId,
                UsuarioAsignadoId = tarea.UsuarioAsignadoId
            };

            _items.Add(entity);
            return Clone(entity);
        }
    }

    public bool Update(Tarea tarea)
    {
        lock (_lock)
        {
            int idx = _items.FindIndex(x => x.Id == tarea.Id);
            if (idx < 0) return false;

            _items[idx].Nombre = tarea.Nombre;
            _items[idx].Descripcion = tarea.Descripcion;
            _items[idx].Estado = tarea.Estado;
            _items[idx].UsuarioAsignadoId = tarea.UsuarioAsignadoId;
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            Tarea? item = _items.FirstOrDefault(x => x.Id == id);
            if (item is null) return false;

            _items.Remove(item);
            return true;
        }
    }

    private static Tarea Clone(Tarea x) => new()
    {
        Id = x.Id,
        Nombre = x.Nombre,
        Descripcion = x.Descripcion,
        Estado = x.Estado,
        UsuarioId = x.UsuarioId,
        UsuarioAsignadoId = x.UsuarioAsignadoId
    };
}
