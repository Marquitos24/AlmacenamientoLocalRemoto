using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using ServiceRemote.Repositories;
using ServiceRemote.Entity; 
using FluentAssertions;
using Npgsql;

namespace ServiceRemote.Test.Repositories;

[TestFixture]
public class CrudRepositoryIntegrationTest
{
    private PostgreSqlContainer _container = null!;
    private UserRepository _repository = null!;
    private AppDbContext _context = null!;
    
[OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        // Lanzamoss contenedor PostgreSQL
        _container = new PostgreSqlBuilder("postgres:17-alpine")
            .WithDatabase("Users")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _container.StartAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        // Configuramos EF para apuntar a la BD de Docker
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        _context = new AppDbContext(options);

        //Aseguramos que la base de datos y las tablas existen
        await _context.Database.EnsureCreatedAsync();

        // Limpiar la tabla ANTES de cada test, ya que los tests deben ser independientes entre sí 
        _context.Users.RemoveRange(_context.Users);
        await _context.SaveChangesAsync();

        // inicializamos el repositorio pasándole el contexto
        _repository = new UserRepository(_context);
        using var connection = new NpgsqlConnection(_container.GetConnectionString());
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _container.DisposeAsync();
    }

    [Test]
    public async Task Create_UserNuevo_RetornaId()
    {
        // Arrange
        var user = new UserEntity() { Name = "Ana", UserName = "ania", Email = "ana@email.com"};

        // Act
        var us = await _repository.CreateAsync(user);

        // Assert
        us.Should().NotBeNull();
        us.Name.Should().Be("Ana");
        us.UserName.Should().Be("ania");
        us.Email.Should().Be("ana@email.com");
    }

    [Test]
    public async Task GetAll_ConDatos_RetornaTodas()
    {
        // Arrange
        await _repository.CreateAsync(new UserEntity {Name = "Carlos", UserName = "carlitos24", Email = "carlos@email.com"});
        await _repository.CreateAsync(new UserEntity {Name = "Sebastian", UserName = "sebas", Email = "sebas@email.com" });

        // Act
        var usuarios = (await _repository.GetAllAsync()).ToList();

        // Assert
        usuarios.Should().HaveCount(2);
        usuarios.Should().Contain(u => u.Name == "Carlos");
        usuarios.Should().Contain(u => u.Name == "Sebastian");
        usuarios.Should().Contain(u => u.Email == "sebas@email.com");
    }

    [Test]
    public async Task GetById_Existe_RetornaUsuario()
    {
        // Arrange
        var user = new UserEntity {Name = "Carlos", UserName = "carlitos24", Email = "carlos@email.com"};
        var createdUser =  await _repository.CreateAsync(user);
       
        // Act
        var resultado = await _repository.GetByIdAsync(createdUser.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Name.Should().Be("Carlos");
    }
    
    [Test]
    public async Task Update_CambiarDatos_RetornaUsuario()
    {
        // Arrange
        var user = new UserEntity {Name = "Carlos", UserName = "carlitos24", Email = "carlos@email.com"};
        var createdUser = await _repository.CreateAsync(user);
    
        // Act
        createdUser.Name = "Puyol";
        createdUser.UserName = "capitan";
        var resultado = await _repository.UpdateAsync(createdUser.Id, createdUser);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Name.Should().Be("Puyol");
        resultado.UserName.Should().Be("capitan");
    }
    
    [Test]
    public async Task Delete_User_SinRetorno()
    {
        // Arrange
        var user = new UserEntity {Name = "Carlos", UserName = "carlitos24", Email = "carlos@email.com"};
        var createdUser =  await _repository.CreateAsync(user);
       
        // Act
        await _repository.DeleteAsync(createdUser.Id);

        // Assert
        var deletedUser = await _repository.GetByIdAsync(createdUser.Id);
        deletedUser.Should().BeNull(); 
    }
}