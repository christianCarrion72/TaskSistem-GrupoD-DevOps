using TaskSis.Api.Domain.Entities;

namespace TaskSis.Api.Infrastructure.Repositories;

public class InMemoryUsuarioRepository : IUsuarioRepository
{
    private readonly List<Usuario> _items = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public IReadOnlyList<Usuario> GetAll()
    {
        lock (_lock)
        {
            return _items.Select(Clone).ToList();
        }
    }

    public Usuario? GetById(int id)
    {
        lock (_lock)
        {
            Usuario? item = _items.FirstOrDefault(x => x.Id == id);
            return item is null ? null : Clone(item);
        }
    }

    public Usuario Add(Usuario usuario)
    {
        lock (_lock)
        {
            Usuario entity = new()
            {
                Id = _nextId++,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                FechaCreacion = DateTime.UtcNow
            };

            _items.Add(entity);
            return Clone(entity);
        }
    }

    public bool Update(Usuario usuario)
    {
        lock (_lock)
        {
            int idx = _items.FindIndex(x => x.Id == usuario.Id);
            if (idx < 0) return false;

            _items[idx].Nombre = usuario.Nombre;
            _items[idx].Correo = usuario.Correo;
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            Usuario? item = _items.FirstOrDefault(x => x.Id == id);
            if (item is null) return false;

            _items.Remove(item);
            return true;
        }
    }

    private static Usuario Clone(Usuario x) => new()
    {
        Id = x.Id,
        Nombre = x.Nombre,
        Correo = x.Correo,
        FechaCreacion = x.FechaCreacion
    };
}

