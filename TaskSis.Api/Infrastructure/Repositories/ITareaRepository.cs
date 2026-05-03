using TaskSis.Api.Domain.Entities;

namespace TaskSis.Api.Infrastructure.Repositories;

public interface ITareaRepository
{
    IReadOnlyList<Tarea> GetAll();
    Tarea? GetById(int id);
    Tarea Add(Tarea tarea);
    bool Update(Tarea tarea);
    bool Delete(int id);
}