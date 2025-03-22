using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using todolist.BL;
using todolist.DAO;
using todolist.Models;
using Xunit;

namespace todolist.Tests
{
    public class ActivitiesBLTests
    {
        private readonly Mock<IActivitiesDAO> _mockActivitiesDAO;
        private readonly ActivitiesBL _activitiesBL;

        public ActivitiesBLTests()
        {
            _mockActivitiesDAO = new Mock<IActivitiesDAO>();
            _activitiesBL = new ActivitiesBL(_mockActivitiesDAO.Object);
        }

        [Fact]
        public async Task GetActivities_ShouldReturnAllActivities()
        {
            // Arrange
            var expectedActivities = new List<Activity>
            {
                new Activity { Id = 1, Title = "Test Activity 1", Description = "Description 1", UserId = 1 },
                new Activity { Id = 2, Title = "Test Activity 2", Description = "Description 2", UserId = 1 }
            };

            _mockActivitiesDAO.Setup(dao => dao.GetActivities())
                .ReturnsAsync(expectedActivities);

            // Act
            var result = await _activitiesBL.GetActivities();

            // Assert
            Assert.Equal(expectedActivities.Count, result.Count());
            Assert.Equal(expectedActivities[0].Id, result.ElementAt(0).Id);
            Assert.Equal(expectedActivities[1].Id, result.ElementAt(1).Id);
        }

        [Fact]
        public async Task GetActivityById_ShouldReturnActivity_WhenActivityExists()
        {
            // Arrange
            var expectedActivity = new Activity { Id = 1, Title = "Test Activity", Description = "Description", UserId = 1 };

            _mockActivitiesDAO.Setup(dao => dao.GetActivityById(1))
                .ReturnsAsync(expectedActivity);

            // Act
            var result = await _activitiesBL.GetActivityById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedActivity.Id, result.Id);
            Assert.Equal(expectedActivity.Title, result.Title);
        }

        [Fact]
        public async Task GetActivityById_ShouldReturnNull_WhenActivityDoesNotExist()
        {
            // Arrange
            _mockActivitiesDAO.Setup(dao => dao.GetActivityById(999))
                .ReturnsAsync((Activity)null);

            // Act
            var result = await _activitiesBL.GetActivityById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddActivity_ShouldCallDAOMethod()
        {
            // Arrange
            var activity = new Activity { Title = "New Activity", Description = "New Description", UserId = 1 };
            Activity capturedActivity = null;
            _mockActivitiesDAO.Setup(dao => dao.AddActivity(It.IsAny<Activity>()))
                .Callback<Activity>(a => capturedActivity = a)
                .Returns(Task.CompletedTask);

            // Act
            await _activitiesBL.AddActivity(activity);

            // Assert
            _mockActivitiesDAO.Verify(dao => dao.AddActivity(It.IsAny<Activity>()), Times.Once);
            Assert.NotNull(capturedActivity);
            Assert.Equal(activity.Title, capturedActivity.Title);
            Assert.Equal(activity.Description, capturedActivity.Description);
            Assert.Equal(activity.UserId, capturedActivity.UserId);
            Assert.Equal(ActivityStatus.ToDo, capturedActivity.Status);
        }

        [Fact]
        public async Task UpdateActivity_ShouldCallDAOMethod()
        {
            // Arrange
            var activity = new Activity { Id = 1, Title = "Test Activity", Description = "Test Description" };
            _mockActivitiesDAO.Setup(dao => dao.GetActivityById(1)).ReturnsAsync(activity);
            Activity capturedActivity = null;
            _mockActivitiesDAO.Setup(dao => dao.UpdateActivity(It.IsAny<Activity>()))
                .Callback<Activity>(a => capturedActivity = a)
                .Returns(Task.CompletedTask);
            
            // Act
            await _activitiesBL.UpdateActivity(activity);
            
            // Assert
            _mockActivitiesDAO.Verify(dao => dao.UpdateActivity(It.IsAny<Activity>()), Times.Once);
            Assert.NotNull(capturedActivity);
            Assert.Equal(activity.Id, capturedActivity.Id);
            Assert.Equal(activity.Title, capturedActivity.Title);
            Assert.Equal(activity.Description, capturedActivity.Description);
        }

        [Fact]
        public async Task DeleteActivity_ShouldCallDAOMethod()
        {
            // Arrange
            int activityId = 1;

            // Act
            await _activitiesBL.DeleteActivity(activityId);

            // Assert
            _mockActivitiesDAO.Verify(dao => dao.DeleteActivity(activityId), Times.Once);
        }

        [Fact]
        public async Task GetActivitiesByUserId_ShouldReturnUserActivities()
        {
            // Arrange
            int userId = 1;
            var expectedActivities = new List<Activity>
            {
                new Activity { Id = 1, Title = "Test Activity 1", Description = "Description 1", UserId = userId },
                new Activity { Id = 2, Title = "Test Activity 2", Description = "Description 2", UserId = userId }
            };

            _mockActivitiesDAO.Setup(dao => dao.GetActivitiesByUserId(userId))
                .ReturnsAsync(expectedActivities);

            // Act
            var result = await _activitiesBL.GetActivitiesByUserId(userId);

            // Assert
            Assert.Equal(expectedActivities.Count, result.Count());
            Assert.All(result, activity => Assert.Equal(userId, activity.UserId));
        }
    }
}