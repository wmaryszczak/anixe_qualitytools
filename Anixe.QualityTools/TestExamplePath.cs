using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Linq;

namespace Anixe.QualityTools
{

  public class TestExamplePath
  {
    private readonly static string rootPath = LoadProjectRootPath();

    private static string LoadProjectRootPath()
    {
      var dir = GetBaseDir("*.csproj");
      if (!string.IsNullOrEmpty(dir))
      {
        return dir;
      }
      throw new Exception($"Cannot find csproj dir");
    }

    private static string GetBaseDir(string proj)
    {
      var currDir = Environment.CurrentDirectory;
      if (Directory.EnumerateFiles(currDir, proj).Any())
      {
        return currDir;
      }
      while (Directory.GetParent(currDir) != null && !Directory.EnumerateFiles(currDir, proj).Any())
      {
        currDir = Directory.GetParent(currDir).FullName;
      }
      return currDir;
    }


    private readonly string examplesPath;

    public TestExamplePath(string examplesDir = "Examples")
    {
      this.examplesPath = Path.Combine(rootPath, examplesDir);
    }

    public string GetPath(Type testCase, string ext = "xml", string suffix = null)
    {
      var exampleDirPath = GetExamplesDirPath();
      var dir = new DirectoryInfo(exampleDirPath);
      var projectDir = dir.Parent.Name;
      var testCasePath = Path.Combine(testCase.FullName.Replace(projectDir + ".", string.Empty).Split('.'));
      // var examplePath = Path.Combine(exampleDirPath, testCasePath + (suffix ?? string.Empty) + "." + ext);


      var examplePath = Path.Combine(exampleDirPath, testCasePath + (suffix ?? string.Empty) + "." + ext);
      if (!File.Exists(examplePath))
      {
        examplePath = Path.Combine(exampleDirPath, testCasePath, (suffix ?? string.Empty) + "." + ext);
      }

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

    public static byte[] ReadAllBytes(Type testCase, string ext = "xml", string suffix = null)
    {
      var examplePath = GetExamplePath(testCase, ext, suffix);
      return exampleFilesCache.GetOrAdd(examplePath, LoadAsByteArray);     
    }  

    public static string[] ReadAllLines(Type testCase, string ext = "xml", string suffix = null)
    {
      var examplePath = GetExamplePath(testCase, ext, suffix);
      return File.ReadAllLines(examplePath);
    }  

    public static IEnumerable<string> ReadLines(Type testCase, string ext = "xml", string suffix = null)
    {
      var examplePath = GetExamplePath(testCase, ext, suffix);
      return File.ReadLines(examplePath);
    }  

    public static string ReadAllText(Type testCase, string ext = "xml", string suffix = null)
    {
      using(var reader = OpenText(testCase, ext, suffix))
      {
        return reader.ReadToEnd();
      }
    }

    public static Stream OpenRead(Type testCase, string ext = "xml", string suffix = null)
    {
      return new MemoryStream(ReadAllBytes(testCase, ext, suffix));
    }

    public static StreamReader OpenText(Type testCase, string ext = "xml", string suffix = null)
    {
      return new StreamReader(OpenRead(testCase, ext, suffix));
    }
    
    public static string GetExamplesDirPath()
    {
      return path.GetExamplesDirPath();
    }

    public static string GetExamplePath(Type testCase, string ext = "xml", string suffix = null)
    {
      return path.GetPath(testCase, ext, suffix);
    }

    public static string LoadExpectation(Type testCase, string ext = "xml")
    {
      return ReadAllText(testCase, ext, "_Expected");
    }

    public static string LoadPayload(Type testCase, string ext = "xml")
    {
      return ReadAllText(testCase, ext, "_Payload");
    }

    public static string LoadTestFixture(Type testFixture, string ext = "xml", [CallerMemberName] string callerName = "")
    {
      var frame = new StackFrame(1, true);
      Console.WriteLine(frame.GetMethod().DeclaringType.Name);
      return ReadAllText(testFixture, ext, callerName);
    }

    public static string LoadTestFixture(string ext = "xml", [CallerMemberName] string callerName = "")
    {
      var frame = new StackFrame(1, true);
      return ReadAllText(frame.GetMethod().DeclaringType, ext, callerName);
    }

    private static byte[] LoadAsByteArray(string path)
    {
      return File.ReadAllBytes(path);
    }
  }
}
