using Moq;
using TaskSis.Api.Domain.DTOs.Tarea;
using TaskSis.Api.Domain.Entities;
using TaskSis.Api.Features.Tareas;
using TaskSis.Api.Infrastructure.Repositories;
using Xunit;

namespace TaskSis.Api.Tests.Features.Tareas;

public sealed class TareaServiceTests
{
    private readonly Mock<ITareaRepository> _mockTareaRepo;
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepo;
    private readonly TareaMapper _mapper;
    private readonly TareaService _service;

    public TareaServiceTests()
    {
        _mockTareaRepo = new Mock<ITareaRepository>();
        _mockUsuarioRepo = new Mock<IUsuarioRepository>();
        _mapper = new TareaMapper(_mockUsuarioRepo.Object);
        _service = new TareaService(_mockTareaRepo.Object, _mockUsuarioRepo.Object, _mapper);
    }

    [Fact]
    public void Create_ReturnsNotFound_WhenUsuarioDoesNotExist()
    {
        // Arrange
        var dto = new TareaCreateDto
        {
            Nombre = "Diseñar base de datos",
            Descripcion = "Crear el script SQL inicial",
            UsuarioId = 999 // Usuario inexistente
        };

        _mockUsuarioRepo.Setup(r => r.GetById(dto.UsuarioId)).Returns((Usuario?)null);

        // Act
        var response = _service.Create(dto);

        // Assert
        Assert.Equal(404, response.StatusCode);
        Assert.False(response.Body.Exito);
        Assert.Null(response.Body.Datos);
        Assert.Contains("Usuario propietario no encontrado con id", response.Body.Mensaje);
    }

    [Fact]
    public void Create_ReturnsCreatedTarea_WhenDtoIsValidAndUsuarioExists()
    {
        // Arrange
        var dto = new TareaCreateDto
        {
            Nombre = "Prueba de tarea exitosa",
            Descripcion = "Esta es una descripción de prueba",
            UsuarioId = 1
        };

        var propietario = new Usuario { Id = 1, Nombre = "Christian Carrion" };

        _mockUsuarioRepo.Setup(r => r.GetById(1)).Returns(propietario);
        _mockTareaRepo.Setup(r => r.Add(It.IsAny<Tarea>()))
            .Returns((Tarea t) =>
            {
                t.Id = 100;
                return t;
            });

        // Act
        var response = _service.Create(dto);

        // Assert
        Assert.Equal(201, response.StatusCode);
        Assert.True(response.Body.Exito);
        Assert.NotNull(response.Body.Datos);
        Assert.Equal(100, response.Body.Datos.Id);
        Assert.Equal("Prueba de tarea exitosa", response.Body.Datos.Nombre);
        Assert.Equal("Esta es una descripción de prueba", response.Body.Datos.Descripcion);
        Assert.Equal("Christian Carrion", response.Body.Datos.UsuarioPropietarioNombre);

        _mockTareaRepo.Verify(r => r.Add(It.Is<Tarea>(t => t.Nombre == dto.Nombre && t.UsuarioId == dto.UsuarioId)), Times.Once);
    }
}
