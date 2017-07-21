namespace FileControl
{
  public interface IFile
  {
    string FileId { get; }
    string FileName { get; }
    string FileStoreId { get; }
  }
}
