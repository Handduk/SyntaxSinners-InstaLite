using Entity;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Test.Repositories
{
    [TestClass]
    public class CommentRepositoryTest
    {
        private DbContextOptions<InstaLiteContext> _options;
        private InstaLiteContext _context;
        private ICommentRepository _commentRepository;

        [TestInitialize]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<InstaLiteContext>()
                .UseInMemoryDatabase(databaseName: "InstaLite")
                .Options;

            _context = new InstaLiteContext(_options);
            _commentRepository = new CommentRepository(_context);
        }

        [TestMethod]
        public async Task AddComment_ShouldAddCommentToDatabase()
        {
            // Arrange
            var commentToAdd = new Comment
            {
                Id = 1,
                ImageComment = "Test Comment",
                CreatedAt = DateTime.Now,
                UserId = 1,
                PostId = 1,
                Username = "TestUser"
            };

            // Act
            var addedComment = await _commentRepository.AddComment(commentToAdd);

            // Assert
            using (InstaLiteContext context = new(_options))
            {
                Comment commentInDatabase = await context.Comments.FindAsync(commentToAdd.Id);

                addedComment.Should().NotBeNull();
                addedComment.Should().BeEquivalentTo(commentToAdd);
                commentInDatabase.Should().NotBeNull();
                commentInDatabase.Should().BeEquivalentTo(commentToAdd);
            }
        }

        [TestMethod]
        public async Task GetComment_ShouldReturnCommentById()
        {
            // Arrange
            var commentId = 1;
            var comment = new Comment
            {
                Id = commentId,
                ImageComment = "Test Comment",
                CreatedAt = DateTime.Now,
                UserId = 1,
                PostId = 1,
                Username = "TestUser"
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            // Act
            var retrievedComment = await _commentRepository.GetComment(commentId);

            // Assert
            retrievedComment.Should().NotBeNull();
            retrievedComment.Should().BeEquivalentTo(comment);
        }

        [TestMethod]
        public async Task GetComments_ShouldReturnListOfComments()
        {
            // Arrange
            var comments = new List<Comment>
            {
                new Comment {
                    Id = 1,
                    ImageComment = "Comment 1", 
                    CreatedAt = DateTime.Now, 
                    UserId = 1,
                    PostId = 1,
                    Username = "User1"
                },
                new Comment {
                    Id = 2,
                    ImageComment = "Comment 2", 
                    CreatedAt = DateTime.Now, 
                    UserId = 2,
                    PostId = 1,
                    Username = "User2"
                },
            };

            _context.Comments.AddRange(comments);
            _context.SaveChanges();

            // Act
            var result = await _commentRepository.GetComments();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(comments.Count);

            result.Should().ContainEquivalentOf(comments[0]);
            result.Should().ContainEquivalentOf(comments[1]);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
