namespace FileControl.Store
{
  using System.IO;
  using System.Threading.Tasks;

  public interface IFileStore
  {
    string FileStoreId { get; }
    FileStorageType StorageType { get; }

    Task<IFile> CreateFile(string fileName, Stream inputStream);

    Task<IFile> CreateFile(string fileName, byte[] bytes);

    Task<IFile> CreateFile(string fileName, string base64);

    IFile CreateTempFile();

    IFile CreateTempFile(string fileName);

    Stream GetStream(string fileId, FileMode mode);

    Stream GetStream(string fileId, FileMode mode, FileAccess access);

    Stream GetStream(string fileId, FileMode mode, FileAccess access, FileShare share);

    string DeleteFile(string fileId, bool archive);

    IFile CopyFile(string fileId);

    IFile CopyFile(string fileId, string fileName);

    IFile CopyFileAsTemp(string fileId);

    IFile CopyFileAsTemp(string fileId, string fileName);
  }
}
