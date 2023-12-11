namespace Api.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string ImageComment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
