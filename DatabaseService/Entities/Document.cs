namespace DatabaseService.Entities;

public partial class Document
{
    public int Id { get; set; }

    public Guid? UserId { get; set; }

    public string FileName { get; set; } = null!;

    public string OriginalFileName { get; set; } = null!;

    public int? DocumentVersion { get; set; }

    public virtual User? User { get; set; }
}
