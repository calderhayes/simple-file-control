namespace FileControl.Store
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using System.Security.AccessControl;
  using System.Text.RegularExpressions;

  public class LocalDiskStore
    : IFileStore
  {
    /// <summary>
    ///
    /// </summary>
    /// <param name="fileStoreId"></param>
    /// <param name="activeRoot"></param>
    /// <param name="archiveRoot"></param>
    public LocalDiskStore(
      string fileStoreId,
      string activeRoot,
      string archiveRoot)
    {
      this.FileStoreId = fileStoreId;
      this.CheckIfDirectoryIsValid(activeRoot);

      if (archiveRoot != null)
      {
        this.CheckIfDirectoryIsValid(archiveRoot);
      }

      this.ActiveRoot = activeRoot;
      this.ArchiveRoot = archiveRoot;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public string FileStoreId { get; }

    /// <summary>
    ///
    /// </summary>
    public FileStorageType StorageType => FileStorageType.LocalDisk;

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public string CurrentDirectory
    {
      get
      {
        var directoryName = DateTime.UtcNow.ToString("YYYY-MM-DD");

        var path = Path.Combine(this.ActiveRoot, directoryName);

        if (Directory.Exists(path))
          return path;

        Directory.CreateDirectory(path);
        return path;
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private string ActiveRoot { get; }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private string ArchiveRoot { get; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="inputStream"></param>
    /// <returns></returns>
    public async Task<IFile> CreateFile(string fileName, Stream inputStream)
    {
      var name = Path.GetRandomFileName();
      var filePath = Path.Combine(this.CurrentDirectory, name);
      using (var fileStream = File.Open(filePath, FileMode.CreateNew))
      {
        await inputStream.CopyToAsync(fileStream);
      }

      return new SimpleFile(filePath, fileName, this.FileStoreId);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public Task<IFile> CreateFile(string fileName, byte[] bytes)
    {
      using (var memoryStream = new MemoryStream(bytes))
      {
        return this.CreateFile(fileName, memoryStream);
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="base64"></param>
    /// <returns></returns>
    public Task<IFile> CreateFile(string fileName, string base64)
    {
      if (string.IsNullOrEmpty(base64) ||
        !((base64.Length % 4 == 0) && Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None)))
      {
        throw new FileException("Image source is not a valid base64 string");
      }

      var bytes = Convert.FromBase64String(base64);
      return this.CreateFile(fileName, bytes);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public IFile CreateTempFile()
    {
      return this.CreateTempFile(Path.GetRandomFileName());
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public IFile CreateTempFile(string fileName)
    {
      return this.CreateFileModel(Path.GetTempFileName(), fileName);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public Stream GetStream(string fileId, FileMode mode)
    {
      return this.GetStream(fileId, mode, FileAccess.Read);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="mode"></param>
    /// <param name="access"></param>
    /// <returns></returns>
    public Stream GetStream(string fileId, FileMode mode, FileAccess access)
    {
      return this.GetStream(fileId, mode, access, FileShare.None);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="mode"></param>
    /// <param name="access"></param>
    /// <param name="share"></param>
    /// <returns></returns>
    public Stream GetStream(string fileId, FileMode mode, FileAccess access, FileShare share)
    {
      if (!File.Exists(fileId))
      {
        throw new FileException($"{fileId} does not exist!");
      }

      return File.Open(fileId, mode, access, share);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="archive"></param>
    /// <returns></returns>
    public string DeleteFile(string fileId, bool archive)
    {
      if (archive)
      {
        if (this.ArchiveRoot == null)
        {
          throw new FileException("Attempting to archive a file when no ArchiveRoot has been set");
        }

        var name = Path.GetRandomFileName();
        var archivePath = Path.Combine(this.ArchiveRoot, name);
        File.Move(fileId, archivePath);

        return archivePath;
      }
      else
      {
        File.Delete(fileId);
        return null;
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public IFile CopyFile(string fileId)
    {
      return this.CopyFile(fileId, Path.GetRandomFileName());
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public IFile CopyFile(string fileId, string fileName)
    {
      var name = Path.GetRandomFileName();
      var path = Path.Combine(this.CurrentDirectory, name);
      File.Copy(fileId, path);

      return this.CreateFileModel(path, fileName);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public IFile CopyFileAsTemp(string fileId)
    {
      return this.CopyFileAsTemp(fileId, Path.GetRandomFileName());
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public IFile CopyFileAsTemp(string fileId, string fileName)
    {
      var tempFile = Path.GetTempFileName();
      File.Copy(fileId, tempFile);

      return this.CreateFileModel(tempFile, fileName);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private SimpleFile CreateFileModel(string fileId, string fileName)
    {
      return new SimpleFile(fileId, fileName, this.FileStoreId);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    private bool CheckIfDirectoryIsValid(string directory)
    {
      if (directory == null)
        throw new ArgumentNullException(nameof(CheckIfDirectoryIsValid));

      if (!Directory.Exists(directory))
        return false;

      return HasWriteAccessToFolder(directory);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    private bool HasWriteAccessToFolder(string folderPath)
    {
        // Not supported by platform ? Windows only I am guessing
        /*try
        {
            // Attempt to get a list of security permissions from the folder.
            // This will raise an exception if the path is read only or do not have access to view the permissions.
            var directoryInfo = new DirectoryInfo(folderPath);
            DirectorySecurity ds = directoryInfo.GetAccessControl();
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }*/
        return true;
    }
  }
}
