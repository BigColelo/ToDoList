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
    public class ActivitiesDAOTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;

        public ActivitiesDAOTests()
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
        }

        private DbContextOptions<ToDoContext> GetDbContextOptions(string dbName)
        {
            return new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }

        [Fact]
        public async Task GetActivities_ShouldReturnAllActivities()
        {
            // Arrange
            var options = GetDbContextOptions("GetActivities_Test");
            
            // Seed the database
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                context.Activities.Add(new Activity { Id = 1, Title = "Test Activity 1", Description = "Description 1", UserId = 1 });
                context.Activities.Add(new Activity { Id = 2, Title = "Test Activity 2", Description = "Description 2", UserId = 1 });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new ActivitiesDAO(context);
                var activities = await dao.GetActivities();

                // Assert
                Assert.Equal(2, activities.Count());
            }
        }

        [Fact]
        public async Task GetActivityById_ShouldReturnActivity_WhenActivityExists()
        {
            // Arrange
            var options = GetDbContextOptions("GetActivityById_Test");
            
            // Seed the database
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                context.Activities.Add(new Activity { Id = 1, Title = "Test Activity", Description = "Description", UserId = 1 });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new ActivitiesDAO(context);
                var activity = await dao.GetActivityById(1);

                // Assert
                Assert.NotNull(activity);
                Assert.Equal("Test Activity", activity.Title);
            }
        }

        [Fact]
        public async Task AddActivity_ShouldAddNewActivity()
        {
            // Arrange
            var options = GetDbContextOptions("AddActivity_Test");
            var newActivity = new Activity { Title = "New Activity", Description = "New Description", UserId = 1 };

            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new ActivitiesDAO(context);
                await dao.AddActivity(newActivity);
            }

            // Assert
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                Assert.Equal(1, await context.Activities.CountAsync());
                var activity = await context.Activities.FirstAsync();
                Assert.Equal("New Activity", activity.Title);
                Assert.Equal("New Description", activity.Description);
            }
        }

        [Fact]
        public async Task UpdateActivity_ShouldUpdateExistingActivity()
        {
            // Arrange
            var options = GetDbContextOptions("UpdateActivity_Test");
            
            // Seed the database
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                context.Activities.Add(new Activity { Id = 1, Title = "Original Activity", Description = "Original Description", UserId = 1 });
                await context.SaveChangesAsync();
            }
        
            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new ActivitiesDAO(context);
                var activity = await context.Activities.FindAsync(1);
                activity.Title = "Updated Activity";
                activity.Description = "Updated Description";
                await dao.UpdateActivity(activity);
            }
        
            // Assert
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var activity = await context.Activities.FindAsync(1);
                Assert.Equal("Updated Activity", activity.Title);
                Assert.Equal("Updated Description", activity.Description);
            }
        }

        [Fact]
        public async Task DeleteActivity_ShouldRemoveActivity()
        {
            // Arrange
            var options = GetDbContextOptions("DeleteActivity_Test");
            
            // Seed the database
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                context.Activities.Add(new Activity { Id = 1, Title = "Test Activity", Description = "Description", UserId = 1 });
                await context.SaveChangesAsync();
            }
        
            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new ActivitiesDAO(context);
                await dao.DeleteActivity(1);
            }
        
            // Assert
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                Assert.Equal(0, await context.Activities.CountAsync());
            }
        }

        [Fact]
        public async Task GetActivitiesByUserId_ShouldReturnUserActivities()
        {
            // Arrange
            var options = GetDbContextOptions("GetActivitiesByUserId_Test");
            
            // Seed the database
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                context.Activities.Add(new Activity { Id = 1, Title = "User 1 Activity 1", Description = "Description 1", UserId = 1 });
                context.Activities.Add(new Activity { Id = 2, Title = "User 1 Activity 2", Description = "Description 2", UserId = 1 });
                context.Activities.Add(new Activity { Id = 3, Title = "User 2 Activity", Description = "Description", UserId = 2 });
                await context.SaveChangesAsync();
            }
        
            // Act
            using (var context = new ToDoContext(options, _mockConfiguration.Object))
            {
                var dao = new ActivitiesDAO(context);
                var activities = await dao.GetActivitiesByUserId(1);
        
                // Assert
                Assert.Equal(2, activities.Count());
                Assert.All(activities, activity => Assert.Equal(1, activity.UserId));
            }
        }
    }
}