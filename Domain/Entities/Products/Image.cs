using Domain.SeedWork;

namespace Domain.Entities.Products;

public class Image : AuditableEntity
{
    public byte[] Content { get; protected set; }
    public long Size  { get; protected set; }
    public string FileName { get; protected set; }
    public Product Product { get; protected set; }

    protected Image()
    {
        
    }
    
    protected Image(byte[] content, long size, string fileName)
    {
        Content = content;
        Size = size;
        FileName = fileName;
    }

    public static Image Create(byte[] content, long size, string fileName)
    {
        return new Image(content, size, fileName);
    }
}