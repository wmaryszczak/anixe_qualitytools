using System;
using System.IO;
using System.Threading.Tasks;

namespace Anixe.QualityTools
{
  public class TmpFile : IDisposable, IAsyncDisposable
  {
    private static readonly object syncObj = new();
    public string FullPath { get; }
    public string Filename { get; }

    private TmpFile(string fullPath)
    {
      this.FullPath = fullPath;
      this.Filename = Path.GetFileName(fullPath);
    }

    public Stream GetStream() => new FileStream(this.FullPath, FileMode.Open, FileAccess.ReadWrite);

    public StreamWriter OpenWrite() => new(this.FullPath);

    public static TmpFile Create(string path, string? filenameOrExtension = null)
    {
      var ext = Path.GetExtension(filenameOrExtension);
      if (ext == filenameOrExtension)
      {
        filenameOrExtension = $@"{Guid.NewGuid()}.{filenameOrExtension}";
      }
      var fullPath = Path.Combine(path, filenameOrExtension);
      lock (syncObj)
      {
        File.Create(fullPath).Close();
      }
      return new TmpFile(fullPath);
    }

    public void Dispose()
    {
      try
      {
        lock (syncObj)
        {
          if (File.Exists(this.FullPath))
          {
            File.Delete(this.FullPath);
          }
        }
      }
      catch { }
    }

    public async ValueTask DisposeAsync()
    {
      try
      {
        lock (syncObj)
        {
          if (File.Exists(this.FullPath))
          {
            File.Delete(this.FullPath);
          }
        }
      }
      catch { }
      finally
      {
        await Task.Yield(); //force async call
      }
    }
  }
}
