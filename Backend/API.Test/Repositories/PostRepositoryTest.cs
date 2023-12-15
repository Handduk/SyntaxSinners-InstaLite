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
    public class PostRepositoryTest
    {
        private DbContextOptions<InstaLiteContext> _options;
        private InstaLiteContext _context;
        private IPostRepository _postRepository;

        [TestInitialize]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<InstaLiteContext>()
                .UseInMemoryDatabase(databaseName: "InstaLite")
                .Options;

            _context = new InstaLiteContext(_options);
            _postRepository = new PostRepository(_context);
        }

        [TestMethod]
        public async Task AddPost_ShouldAddPostToDatabase()
        {
            // Arrange
            var post = new Post
            {
                Id = 1,
                Title = "Test Title",
                Description = "Test Content",
                Image = "test-image.jpg",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = 1
            };

            // Act
            var addedPost = await _postRepository.AddPost(post);

            // Assert
            addedPost.Should().NotBeNull();
            addedPost.Should().BeEquivalentTo(post);
        }

        [TestMethod]
        public async Task DeletePostById_ShouldDeletePostIfExists()
        {
            // Arrange
            var postIdToDelete = 1;
            var existingPost = new Post { 
                Id = postIdToDelete, 
                Title = "Existing Post",
                Description = "Existing Post Description",
                Image = "existing-post-image.jpg",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = 1
            };

            using (var context = new InstaLiteContext(_options))
            {
                context.Posts.Add(existingPost);
                context.SaveChanges();
            }

            // Act
            var deletedPost = await _postRepository.DeletePostById(postIdToDelete);

            // Assert
            using (var context = new InstaLiteContext(_options))
            {
                var postInDatabase = await context.Posts.FindAsync(postIdToDelete);

                deletedPost.Should().NotBeNull();
                deletedPost.Should().BeEquivalentTo(existingPost);
                postInDatabase.Should().BeNull();
            }
        }

        [TestMethod]
        public async Task GetPost_ShouldReturnPostById()
        {
            // Arrange
            var postId = 1;
            var expectedPost = new Post 
            { 
                Id = postId, 
                Title = "Test Post",
                Description = "Test Post Description",
                Image = "test-post-image.jpg",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = 1
            };

            using (var context = new InstaLiteContext(_options))
            {
                context.Posts.Add(expectedPost);
                context.SaveChanges();
            }

            // Act
            var retrievedPost = await _postRepository.GetPost(postId);

            // Assert
            retrievedPost.Should().NotBeNull();
            retrievedPost.Should().BeEquivalentTo(expectedPost);
        }

        [TestMethod]
        public async Task GetPosts_ShouldReturnListOfPosts()
        {
            // Arrange
            var posts = new List<Post>
            {
                new Post 
                { 
                    Id = 1, Title = "Post 1",
                    Description = "Post 1 Description",
                    Image = "post-1-image.jpg",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = 1
                },
                new Post 
                { 
                    Id = 2, 
                    Title = "Post 2",
                    Description = "Post 2 Description",
                    Image = "post-2-image.jpg",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UserId = 1
                },
            };

            using (var context = new InstaLiteContext(_options))
            {
                context.Posts.AddRange(posts);
                context.SaveChanges();
            }

            // Act
            var result = await _postRepository.GetPosts();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(posts.Count);

            result.Should().ContainEquivalentOf(posts[0]);
            result.Should().ContainEquivalentOf(posts[1]);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
