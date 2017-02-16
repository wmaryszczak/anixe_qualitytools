using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Anixe.QualityTools
{

  public class TestExamplePath
  {
    private readonly static string rootPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..");
    private readonly string examplesPath;

    public TestExamplePath(string examplesDir = "Examples")
    {
      this.examplesPath = Path.Combine(rootPath, examplesDir);
    }

    public string GetPath(Type testFixture, string ext = "xml")
    {
      var exampleDirPath = GetExamplesDirPath();
      var dir = new DirectoryInfo(exampleDirPath);
      var projectDir = dir.Parent.Name;
      var testFixturePath = Path.Combine(testFixture.FullName.Replace(projectDir + ".", string.Empty).Split('.'));
      var examplePath = Path.Combine(exampleDirPath, testFixturePath + "." + ext);

      return examplePath;
    }  
    
    public string GetExamplesDirPath()
    {
      return examplesPath;    
    }
    
  }

  public static class TestExample
  {
    private readonly static ConcurrentDictionary<string, byte[]> exampleFilesCache = new ConcurrentDictionary<string, byte[]>();  
    private readonly static TestExamplePath path = new TestExamplePath();

    public static byte[] ReadAllBytes(Type testFixture, string ext = "xml")
    {
      var examplePath = path.GetPath(testFixture, ext);
      return exampleFilesCache.GetOrAdd(examplePath, LoadAsByteArray);     
    }  

    public static string[] ReadAllLines(Type testFixture, string ext = "xml")
    {
      var examplePath = path.GetPath(testFixture, ext);
      return File.ReadAllLines(examplePath);
    }  

    public static IEnumerable<string> ReadLines(Type testFixture, string ext = "xml")
    {
      var examplePath = path.GetPath(testFixture, ext);
      return File.ReadLines(examplePath);
    }  

    public static string ReadAllText(Type testFixture, string ext = "xml")
    {
      using(var reader = OpenText(testFixture, ext))
      {
        return reader.ReadToEnd();
      }
    }

    public static Stream OpenRead(Type testFixture, string ext = "xml")
    {
      return new MemoryStream(ReadAllBytes(testFixture, ext));
    }

    public static StreamReader OpenText(Type testFixture, string ext = "xml")
    {
      return new StreamReader(OpenRead(testFixture, ext));
    }
    
    public static string GetExamplesDirPath()
    {
      return path.GetExamplesDirPath();
    }

    public static string GetExamplePath(Type testFixture, string ext = "xml")
    {
      return path.GetPath(testFixture, ext);
    }
    
    private static byte[] LoadAsByteArray(string path)
    {
      return File.ReadAllBytes(path);
    }
  }

}
