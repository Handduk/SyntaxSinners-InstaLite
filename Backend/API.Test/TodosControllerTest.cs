using Api.Controllers;
using Api.Dtos;
using AutoMapper;
using Entity;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Api.Test
{
    [TestClass]
    public class TodosControllerTest
    {

        private readonly Mock<ITodoRepository> _todoRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TodosController>> _loggerMock;

        private readonly TodosController sut;

        public TodosControllerTest()
        {
            _todoRepositoryMock = new Mock<ITodoRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TodosController>>();

            sut = new TodosController(_todoRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Add_Todo_Should_Return_New_Todo()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Test",
                Completed = false
            };

            var createTodo = new Todo
            {
                Id = 1,
                Title = "Test",
                Completed = false
            };

            _todoRepositoryMock.Setup(x => x.AddTodo(
                               todo)).ReturnsAsync(createTodo);
            // Act
            var response = await sut.PostTodo(todo);

            // Assert
            var result = response.Result as CreatedAtActionResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            Assert.AreEqual(201, result.StatusCode);
        }

        [TestMethod]
        public async Task Get_Todo_By_Id_Should_Return_Todo()
        {
            // Arrange
            var intTodoId = 1;

            var todo = new Todo
            {
                Id = intTodoId,
                Title = "Test",
                Completed = false
            };

            _todoRepositoryMock.Setup(x => x.GetTodo(intTodoId)).ReturnsAsync(todo);
            // Act
            var response = await sut.GetTodo(intTodoId);

            // Assert
            var result = response.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(200, result.StatusCode);
        }


        [TestMethod]
        public async Task Get_All_Todos_Should_Return_List_Of_Todos()
        {
            // Arrange
            var todos = new List<Todo>
            {
                new Todo
                {
                    Id = 1,
                    Title = "Test",
                    Completed = false
                },
                new Todo
                {
                    Id = 2,
                    Title = "Test",
                    Completed = false
                }
            };

            _todoRepositoryMock.Setup(x => x.GetTodos()).ReturnsAsync(todos);
            // Act
            var response = await sut.GetTodos();

            // Assert
            var result = response.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        public async Task Update_Todo_Should_Return_Updated_Todo()
        {
            // Arrange
            var intTodoId = 1;

            var todo = new Todo
            {
                Id = intTodoId,
                Title = "Test",
                Completed = false
            };

            var updateTodo = new Todo
            {
                Id = intTodoId,
                Title = "Test",
                Completed = true
            };

            _todoRepositoryMock.Setup(x => x.UpdateTodo(intTodoId, todo)).ReturnsAsync(updateTodo);
            // Act
            var response = await sut.PutTodo(intTodoId, todo);
            var badRequest = await sut.PutTodo(2, todo);

            // Assert
            var result = response.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(200, result.StatusCode);

            var badRequestResult = badRequest.Result as BadRequestResult;
            Assert.IsNotNull(badRequestResult);


        }

        [TestMethod]
        public async Task Delete_Todo_Should_Return_Deleted_Todo()
        {
            // Arrange
            var intTodoId = 1;

            var todo = new Todo
            {
                Id = intTodoId,
                Title = "Test",
                Completed = false
            };

            _todoRepositoryMock.Setup(x => x.DeleteTodoById(intTodoId)).ReturnsAsync(todo);
            // Act
            var response = await sut.DeleteTodoItem(intTodoId);
            var badRequest = await sut.DeleteTodoItem(2);

            // Assert
            var result = response.Result as NoContentResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual(204, result.StatusCode);

            var badRequestResult = badRequest.Result as NotFoundResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult, typeof(NotFoundResult));
            Assert.AreEqual(404, badRequestResult.StatusCode);
        }

    }
}