namespace SimpleFileControl
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using SimpleFileControl.Store;

  public class FileStoreCollection
    : IFileStoreCollection
  {
    /// <summary>
    ///
    /// </summary>
    private readonly IList<IFileStore> fileStores;

    /// <summary>
    ///
    /// </summary>
    public FileStoreCollection()
    {
      this.fileStores = new List<IFileStore>();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public string DefaultFileStoreId { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public IFileStore DefaultFileStore
    {
      get
      {
        return this.fileStores
          .Where(fs => fs.FileStoreId == this.DefaultFileStoreId)
          .Single();
      }
    }

    public void RegisterFileStore(IFileStore fileStore)
    {
      if (fileStore == null)
      {
        throw new ArgumentNullException(nameof(fileStore));
      }

      if (string.IsNullOrEmpty(fileStore.FileStoreId))
      {
        throw new FileException($"File Stores must have an identifier - FileStoreId");
      }

      if (this.fileStores.Where(fs => fs.FileStoreId == fileStore.FileStoreId).Any())
      {
        throw new FileException("File Store Id must be unique");
      }

      this.fileStores.Add(fileStore);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileStoreId"></param>
    /// <returns></returns>
    public IFileStore GetFileStore(string fileStoreId)
    {
      return this.fileStores
        .Where(fs => fs.FileStoreId == fileStoreId)
        .Single();
    }
  }
}
