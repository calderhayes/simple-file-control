namespace FileControl
{
  using System;

  /// <summary>
  ///
  /// </summary>
  public class FileException
    : Exception
  {
    /// <summary>
    ///
    /// </summary>
    private const string DefaultMessage = "An error occured in the File";

    /// <summary>
    ///
    /// </summary>
    public FileException()
      : base(DefaultMessage)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    public FileException(string message)
      : base(message)
    {
    }

    public FileException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
