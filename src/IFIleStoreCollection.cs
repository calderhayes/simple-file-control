﻿namespace FileControl
{
  using System;
  using FileControl.Store;

  public interface IFileStoreCollection
  {
    string DefaultFileStoreId { get; set; }

    void RegisterFileStore(IFileStore fileStore);

    IFileStore DefaultFileStore { get; }

    IFileStore GetFileStore(string fileStoreId);
  }
}
