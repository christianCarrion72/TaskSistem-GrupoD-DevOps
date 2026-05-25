using Moq;
using TaskSis.Api.Domain.DTOs.Usuario;
using TaskSis.Api.Domain.Entities;
using TaskSis.Api.Features.Usuarios;
using TaskSis.Api.Infrastructure.Repositories;
using Xunit;

namespace TaskSis.Api.Tests.Features.Usuarios;

public sealed class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _mockRepo;
    private readonly UsuarioMapper _mapper;
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _mockRepo = new Mock<IUsuarioRepository>();
        _mapper = new UsuarioMapper();
        _service = new UsuarioService(_mockRepo.Object, _mapper);
    }

    [Fact]
    public void GetById_ReturnsNotFound_WhenUsuarioDoesNotExist()
    {
        // Arrange
        int nonExistingId = 99;
        _mockRepo.Setup(r => r.GetById(nonExistingId)).Returns((Usuario?)null);

        // Act
        var response = _service.GetById(nonExistingId);

        // Assert
        Assert.Equal(404, response.StatusCode);
        Assert.False(response.Body.Exito);
        Assert.Null(response.Body.Datos);
        Assert.Contains("Usuario no encontrado con id", response.Body.Mensaje);
    }

    [Fact]
    public void Create_ReturnsCreatedUsuario_WhenDtoIsValid()
    {
        // Arrange
        var dto = new UsuarioCreateDto("Juan Perez", "juan.perez@example.com");
        
        _mockRepo.Setup(r => r.Add(It.IsAny<Usuario>()))
            .Returns((Usuario u) =>
            {
                u.Id = 1;
                return u;
            });

        // Act
        var response = _service.Create(dto);

        // Assert
        Assert.Equal(201, response.StatusCode);
        Assert.True(response.Body.Exito);
        Assert.NotNull(response.Body.Datos);
        Assert.Equal(1, response.Body.Datos.Id);
        Assert.Equal("Juan Perez", response.Body.Datos.Nombre);
        Assert.Equal("juan.perez@example.com", response.Body.Datos.Correo);
        Assert.Equal("Usuario creado exitosamente", response.Body.Mensaje);

        _mockRepo.Verify(r => r.Add(It.Is<Usuario>(u => u.Nombre == dto.Nombre && u.Correo == dto.Correo)), Times.Once);
    }
}
