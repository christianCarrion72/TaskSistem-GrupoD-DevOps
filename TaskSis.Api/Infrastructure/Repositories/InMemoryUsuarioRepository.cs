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
                FechaCreacion = DateTime.UtcNow
            };

            _items.Add(entity);
            return Clone(entity);
        }
    }

    private static Usuario Clone(Usuario x) => new()
    {
        Id = x.Id,
        Nombre = x.Nombre,
        FechaCreacion = x.FechaCreacion
    };
}
