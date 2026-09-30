namespace Application.UseCases.Products.Dtos;

public class ImageDto
{
    public byte[] Content { get; set; }
    public string FileName { get; set; }
    public long Size  { get; set; }
}