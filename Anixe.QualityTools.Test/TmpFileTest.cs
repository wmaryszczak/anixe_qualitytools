using System.IO;
using System.Linq;
using Xunit;

namespace Anixe.QualityTools.Test
{
  public class TmpFileTest
  {
    [Fact]
    public void Should_Create_An_Empty_File_With_IDisposable_Lifecycle()
    {
      var path = string.Empty;
      using (var file = TmpFile.Create(".", ".csv"))
      {
        path = file.FullPath;
        Assert.Equal(".csv", Path.GetExtension(path));
        Assert.True(File.Exists(path));
        File.WriteAllText(file.FullPath, "test");
        var content = File.ReadAllText(file.FullPath);
        Assert.Equal("test", content);
      }
      Assert.False(File.Exists(path));
    }

    [Fact]
    public void Should_Create_Multiple_Files_With_Unique_Filename_When_Only_Extension_Is_Provided()
    {
      using var file1 = TmpFile.Create(".", ".csv");
      using var file2 = TmpFile.Create(".", ".csv");
      using var file3 = TmpFile.Create(".", ".csv");
      Assert.Equal(3, Directory.EnumerateFiles(".", "*.csv").ToList().Distinct().Count());
    }

    [Fact]
    public void Should_Create_Only_One_File_When_Filename_Is_Provided()
    {
      using var file1 = TmpFile.Create(".", "test.csv");
      using var file2 = TmpFile.Create(".", "test.csv");
      using var file3 = TmpFile.Create(".", "test.csv");
      Assert.Single(Directory.EnumerateFiles(".", "*.csv").ToList().Distinct());
    }
  }
}
