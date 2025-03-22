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
    public class UserBLTests
    {
        private readonly Mock<IUserDAO> _mockUserDAO;
        private readonly UserBL _userBL;

        public UserBLTests()
        {
            _mockUserDAO = new Mock<IUserDAO>();
            _userBL = new UserBL(_mockUserDAO.Object);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "user1@example.com" },
                new User { Id = 2, Username = "user2", Email = "user2@example.com" }
            };

            _mockUserDAO.Setup(dao => dao.GetUsers())
                .ReturnsAsync(expectedUsers);

            // Act
            var result = await _userBL.GetUsers();

            // Assert
            Assert.Equal(expectedUsers.Count, result.Count());
            Assert.Equal(expectedUsers[0].Id, result.ElementAt(0).Id);
            Assert.Equal(expectedUsers[1].Id, result.ElementAt(1).Id);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var expectedUser = new User { Id = 1, Username = "user1", Email = "user1@example.com" };

            _mockUserDAO.Setup(dao => dao.GetUserById(1))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userBL.GetUserById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(expectedUser.Username, result.Username);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserDAO.Setup(dao => dao.GetUserById(999))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userBL.GetUserById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddUser_ShouldCallDAOMethod()
        {
            // Arrange
            var user = new User { Username = "newuser", Email = "newuser@example.com" };

            // Act
            await _userBL.AddUser(user);

            // Assert
            _mockUserDAO.Verify(dao => dao.AddUser(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_ShouldCallDAOMethod()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com", FirstName = "Test", LastName = "User", Password = "password" };
            _mockUserDAO.Setup(dao => dao.GetUserById(1)).ReturnsAsync(user);
            
            // Act
            await _userBL.UpdateUser(user);
            
            // Assert
            _mockUserDAO.Verify(dao => dao.UpdateUser(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldCallDAOMethod()
        {
            // Arrange
            int userId = 1;

            // Act
            await _userBL.DeleteUser(userId);

            // Assert
            _mockUserDAO.Verify(dao => dao.DeleteUser(userId), Times.Once);
        }
    }
}