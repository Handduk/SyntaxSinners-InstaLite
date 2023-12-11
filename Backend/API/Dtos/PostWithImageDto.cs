namespace Api.Dtos
{
    public class PostWithImageDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
