using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using todolist.Context;
using todolist.DAO;
using todolist.Models;
using Xunit;

namespace todolist.Tests
{
    public class UserDAOTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;

        public UserDAOTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Configura il mock per restituire valori specifici
            var adminUrlSection = new Mock<IConfigurationSection>();
            adminUrlSection.Setup(s => s.Value).Returns("http://keycloak:8080");
            
            var tokenUrlSection = new Mock<IConfigurationSection>();
            tokenUrlSection.Setup(s => s.Value).Returns("http://keycloak:8080");
            
            var adminUsernameSection = new Mock<IConfigurationSection>();
            adminUsernameSection.Setup(s => s.Value).Returns("admin");
            
            var adminPasswordSection = new Mock<IConfigurationSection>();
            adminPasswordSection.Setup(s => s.Value).Returns("admin");
            
            _mockConfiguration.Setup(c => c.GetSection("Keycloak:AdminUrl")).Returns(adminUrlSection.Object);
            _mockConfiguration.Setup(c => c.GetSection("Keycloak:TokenUrl")).Returns(tokenUrlSection.Object);
            _mockConfiguration.Setup(c => c.GetSection("Keycloak:AdminUsername")).Returns(adminUsernameSection.Object);
            _mockConfiguration.Setup(c => c.GetSection("Keycloak:AdminPassword")).Returns(adminPasswordSection.Object);
            
            // Configura anche l'accesso diretto alle chiavi
            _mockConfiguration.Setup(c => c["Keycloak:AdminUrl"]).Returns("http://keycloak:8080");
            _mockConfiguration.Setup(c => c["Keycloak:TokenUrl"]).Returns("http://keycloak:8080");
            _mockConfiguration.Setup(c => c["Keycloak:AdminUsername"]).Returns("admin");
            _mockConfiguration.Setup(c => c["Keycloak:AdminPassword"]).Returns("admin");
        }

        private DbContextOptions<ToDoContext> GetDbContextOptions(string dbName)
        {
            return new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }

        [Fact]
        public async Task GetUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var options = GetDbContextOptions("GetUsers_Test");
            
            // Seed the database
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                context.Users.Add(new User { 
                    Id = 1, 
                    Username = "user1", 
                    Email = "user1@example.com",
                    FirstName = "First1",
                    LastName = "Last1",
                    Password = "password123"
                });
                context.Users.Add(new User { 
                    Id = 2, 
                    Username = "user2", 
                    Email = "user2@example.com",
                    FirstName = "First2",
                    LastName = "Last2",
                    Password = "password456"
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new UserDAO(context, _mockConfiguration.Object);
                var users = await dao.GetUsers();

                // Assert
                Assert.Equal(2, users.Count());
            }
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserById_Test");
            
            // Seed the database
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                context.Users.Add(new User { 
                    Id = 1, 
                    Username = "user1", 
                    Email = "user1@example.com",
                    FirstName = "First",
                    LastName = "Last",
                    Password = "password123"
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new UserDAO(context, _mockConfiguration.Object);
                var user = await dao.GetUserById(1);

                // Assert
                Assert.NotNull(user);
                Assert.Equal("user1", user.Username);
            }
        }

        [Fact]
        public async Task AddUser_ShouldAddNewUser()
        {
            // Arrange
            var options = GetDbContextOptions("AddUser_Test");
            var newUser = new User { 
                Username = "newuser", 
                Email = "newuser@example.com",
                FirstName = "New",
                LastName = "User",
                Password = "password123"
            };
        
            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new UserDAO(context, _mockConfiguration.Object);
                await dao.AddUser(newUser);
            }
        
            // Assert
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                Assert.Equal(1, await context.Users.CountAsync());
                var user = await context.Users.FirstAsync();
                Assert.Equal("newuser", user.Username);
                Assert.Equal("newuser@example.com", user.Email);
            }
        }

        [Fact]
        public async Task UpdateUser_ShouldUpdateExistingUser()
        {
            // Arrange
            var options = GetDbContextOptions("UpdateUser_Test");
            
            // Seed the database
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                context.Users.Add(new User { 
                    Id = 1, 
                    Username = "originaluser", 
                    Email = "original@example.com",
                    FirstName = "Original",
                    LastName = "User",
                    Password = "originalpass"
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new UserDAO(context, _mockConfiguration.Object);
                var user = await context.Users.FindAsync(1);
                user.Username = "updateduser";
                user.Email = "updated@example.com";
                await dao.UpdateUser(user);
            }

            // Assert
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var user = await context.Users.FindAsync(1);
                Assert.Equal("updateduser", user.Username);
                Assert.Equal("updated@example.com", user.Email);
            }
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveUser()
        {
            // Arrange
            var options = GetDbContextOptions("DeleteUser_Test");
            
            // Seed the database
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                context.Users.Add(new User { 
                    Id = 1, 
                    Username = "user1", 
                    Email = "user1@example.com",
                    FirstName = "First",
                    LastName = "Last",
                    Password = "password123"
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new UserDAO(context, _mockConfiguration.Object);
                await dao.DeleteUser(1);
            }

            // Assert
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                Assert.Equal(0, await context.Users.CountAsync());
            }
        }

        [Fact]
        public async Task GetUserByUsername_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserByUsername_Test");
            
            // Seed the database
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                context.Users.Add(new User { 
                    Id = 1, 
                    Username = "testuser", 
                    Email = "test@example.com",
                    FirstName = "Test",
                    LastName = "User",
                    Password = "testpass"
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new UserDAO(context, _mockConfiguration.Object);
                var user = await dao.GetUserByUsername("testuser");

                // Assert
                Assert.NotNull(user);
                Assert.Equal("testuser", user.Username);
            }
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var options = GetDbContextOptions("GetUserByEmail_Test");
            
            // Seed the database
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                context.Users.Add(new User { 
                    Id = 1, 
                    Username = "testuser", 
                    Email = "test@example.com",
                    FirstName = "First",
                    LastName = "Last",
                    Password = "password123"
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new UserDAO(context, _mockConfiguration.Object);
                var user = await dao.GetUserByEmail("test@example.com");

                // Assert
                Assert.NotNull(user);
                Assert.Equal("test@example.com", user.Email);
            }
        }

        // Continua ad aggiornare tutti gli altri metodi di test allo stesso modo
    }
}