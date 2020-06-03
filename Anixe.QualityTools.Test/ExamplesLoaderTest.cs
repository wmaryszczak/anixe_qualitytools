using System;
using System.Threading.Tasks;
using Xunit;

namespace Anixe.QualityTools.Test
{
  public class ExamplesLoaderTest
  {
    [Fact]
    public void Should_Load_Examples()
    {
      Assert.Equal("{\"name\": \"test\"}", TestExample.ReadAllText(this.GetType(), "json"));
      Assert.Equal("<test>ExamplesLoaderTest<test>", TestExample.ReadAllText(this.GetType()));
      Assert.Equal("<test>Should_Load_Examples<test>", TestExample.LoadTestFixture(this.GetType()));
      Assert.Equal("<test>Should_Load_Examples<test>", TestExample.LoadTestFixture());
    }

    [Fact]
    public async Task Should_Load_ExamplesAsync()
    {
      Assert.Equal("{\"name\": \"test\"}", TestExample.ReadAllText(this.GetType(), "json"));
      Assert.Equal("<test>ExamplesLoaderTest<test>", TestExample.ReadAllText(this.GetType()));
      Assert.Equal("<test>Should_Load_Examples<test>", TestExample.LoadTestFixture(this.GetType()));
      Assert.Equal("<test>Should_Load_Examples<test>", TestExample.LoadTestFixture());
    }
  }
}