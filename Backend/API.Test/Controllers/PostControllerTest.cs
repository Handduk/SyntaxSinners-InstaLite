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

namespace Api.Test.Controllers
{
    [TestClass]
    public class PostControllerTest
    {

        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PostController>> _loggerMock;

        private readonly PostController sut;

        public PostControllerTest()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PostController>>();

            sut = new PostController(_postRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task GetPosts_ReturnsOkResult_WithPostDtoList()
        {
            // Arrange
            var posts = new List<Post> { new Post(), new Post() };
            var postDtos = new List<PostDto> { new PostDto(), new PostDto() };

            _postRepositoryMock.Setup(repo => repo.GetPosts()).ReturnsAsync(posts);
            _mapperMock.Setup(mapper => mapper.Map<List<PostDto>>(It.IsAny<List<Post>>())).Returns(postDtos);

            // Act
            var result = await sut.GetPosts();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>().Which
                .Value.Should().BeEquivalentTo(postDtos);

            result.Result.Should().BeOfType<OkObjectResult>().Which
                .StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task GetPostWithID_ReturnsOkResult_WithPostDto()
        {
            // Arrange
            var postId = 1;
            var post = new Post();
            var postDto = new PostDto();

            _postRepositoryMock.Setup(repo => repo.GetPost(postId)).ReturnsAsync(post);
            _mapperMock.Setup(mapper => mapper.Map<PostDto>(post)).Returns(postDto);

            // Act
            var result = await sut.GetPost(postId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>().Which
                .Value.Should().BeEquivalentTo(postDto);
            result.Result.Should().BeOfType<OkObjectResult>().Which
                .StatusCode.Should().Be(200);
        }

        
        [TestMethod]
        public async Task DeletePost_WithValidId_ReturnsOkResultWithDto()
        {
            // Arrange
            var postId = 1;
            var post = new Post
            {
                Id = postId,
                Title = "Test",
                Description = "TestText",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Image = "Image.jpg",
                UserId = 1
            };
            var postDto = new PostDto
            {
                Id = postId,
                Title = "Test",
                Description = "TestText",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Image = "Image.jpg",
                UserId = 1
            };

            _postRepositoryMock.Setup(repo => repo.DeletePostById(postId)).ReturnsAsync(post);
            _mapperMock.Setup(mapper => mapper.Map<PostDto>(post)).Returns(postDto);

            // Act
            var result = await sut.DeletePost(postId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>().Which
                .Value.Should().BeEquivalentTo(postDto);
            result.Result.Should().BeOfType<OkObjectResult>().Which
                .StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task DeletePost_WithNonexistentId_ReturnsNotFoundResult()
        {
            // Arrange
            int postId = 1;

            _postRepositoryMock.Setup(repo => repo.DeletePostById(postId)).ReturnsAsync((Post)null);

            // Act
            var result = await sut.DeletePost(postId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
            result.Result.Should().BeOfType<NotFoundResult>().Which
                .StatusCode.Should().Be(404);
        }

        [TestMethod]
        public async Task DeletePost_WithException_ReturnsBadRequestResult()
        {
            // Arrange
            int postId = 1;

            _postRepositoryMock.Setup(repo => repo.DeletePostById(postId)).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await sut.DeletePost(postId);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            result.Result.Should().BeOfType<BadRequestObjectResult>().Which
                .Value.Should().BeEquivalentTo(new { error = "Failed to delete post." });
        }
    }
}