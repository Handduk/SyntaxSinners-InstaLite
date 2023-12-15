using Api.Controllers;
using Api.Dtos;
using AutoMapper;
using AutoMapper.Internal;
using Entity;
using FluentAssertions;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Text;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Api.Test.Controllers
{
    [TestClass]
    public class CommentControllerTest
    {

        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CommentController>> _loggerMock;

        private readonly CommentController sut;

        public CommentControllerTest()
        {
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CommentController>>();

            sut = new CommentController(_commentRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetComments_ReturnsOkResult_WithCommentDtoList()
        {
            // Arrange
            var comments = new List<Comment> { new Comment(), new Comment() };
            var commentDtos = new List<CommentDto> { new CommentDto(), new CommentDto() };

            _commentRepositoryMock.Setup(repo => repo.GetComments()).ReturnsAsync(comments);
            _mapperMock.Setup(mapper => mapper.Map<List<CommentDto>>(It.IsAny<List<Comment>>())).Returns(commentDtos);

            // Act
            var result = await sut.GetComments();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>().Which
                .Value.Should().BeEquivalentTo(commentDtos);

            result.Result.Should().BeOfType<OkObjectResult>().Which
                .StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task GetComments_ReturnsInternalServerError_WithCommentDtoList()
        {
            // Arrange
            var comments = new List<Comment> { new Comment(), new Comment() };
            var commentDtos = new List<CommentDto> { new CommentDto(), new CommentDto() };

            _commentRepositoryMock.Setup(repo => repo.GetComments()).ThrowsAsync(new Exception("Simulated error"));

            _mapperMock.Setup(mapper => mapper.Map<List<CommentDto>>(It.IsAny<List<Comment>>())).Returns(commentDtos);

            // Act
            var result = await sut.GetComments();

            // Assert
            result.Result.Should().BeOfType<ObjectResult>().Which
                .StatusCode.Should().Be(500);
        }

        [TestMethod]
        public async Task GetCommentWithID_ReturnsOkResult_WithCommentDto()
        {
            // Arrange
            var commentId = 1;
            var comment = new Comment();
            var commentDto = new CommentDto();

            _commentRepositoryMock.Setup(repo => repo.GetComment(commentId)).ReturnsAsync(comment);
            _mapperMock.Setup(mapper => mapper.Map<CommentDto>(comment)).Returns(commentDto);

            // Act
            var result = await sut.GetComment(commentId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>().Which
                .Value.Should().BeEquivalentTo(commentDto);
            result.Result.Should().BeOfType<OkObjectResult>().Which
                .StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task GetCommentWithId_ReturnsInternalServerError_WithCommentDtoList()
        {
            // Arrange
            var commentId = 1;
            var comment = new Comment(); 
            var commentDto = new CommentDto();

            _commentRepositoryMock.Setup(repo => repo.GetComment(commentId)).ThrowsAsync(new Exception("Simulated error"));


            _mapperMock.Setup(mapper => mapper.Map<CommentDto>(comment)).Returns(commentDto);

            // Act
            var result = await sut.GetComment(commentId);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>().Which
                .StatusCode.Should().Be(500);
        }

        [TestMethod]
        public void CreateComment_ReturnsOkResult_WhenCommentCreatedSuccessfully()
        {
            // Arrange
            var commentDto = new CommentDto
            {
                Id = 1,
                ImageComment = "Test",
                CreatedAt = DateTime.Now,
                UserId = 1,
                PostId = 1,
                Username = "Test"
            };

            var comment = new Comment
            {
                Id = 1,
                ImageComment = "Test",
                CreatedAt = DateTime.Now,
                UserId = 1,
                PostId = 1,
                Username = "Test"
            };

            _mapperMock.Setup(mapper => mapper.Map<Comment>(commentDto)).Returns(comment);

            // Act
            var result = sut.CreateComment(commentDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which
                .Value.Should().Be("Comment created successfully");
        }

        [TestMethod]
        public void CreateComment_ReturnsBadRequest_WhenImageCommentIsNull()
        {
            // Arrange
            var commentDto = new CommentDto
            {
                Id = 1,
                ImageComment = null,
                CreatedAt = DateTime.Now,
                UserId = 1,
                PostId = 1,
                Username = "Test"
            };

            var comment = new Comment
            {
                Id = 1,
                ImageComment = null,
                CreatedAt = DateTime.Now,
                UserId = 1,
                PostId = 1,
                Username = "Test"
            };

            _mapperMock.Setup(mapper => mapper.Map<Comment>(commentDto)).Returns(comment);

            // Act
            var result = sut.CreateComment(commentDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>().Which
                .StatusCode.Should().Be(400);
        }

    }
}