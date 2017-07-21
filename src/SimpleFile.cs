namespace SimpleFileControl
{
  internal class SimpleFile
    : IFile
  {
    public SimpleFile(string fileId, string fileName, string fileStoreId)
    {
      this.FileId = fileId;
      this.FileName = fileName;
      this.FileStoreId = fileStoreId;
    }

    public string FileId { get; set; }
    public string FileName { get; set; }
    public string FileStoreId { get; set; }
  }
}
