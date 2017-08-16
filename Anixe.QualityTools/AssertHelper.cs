using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Anixe.QualityTools
{

  public class AssertHelper
  {
    public static void AreXmlDocumentsEqual(string expected, string actual)
    {
      var actualXml = XDocument.Parse(actual);
      var expectedXml = XDocument.Parse(expected);
      try
      {
        actualXml.Should().BeEquivalentTo(expectedXml);
      }
      catch (Xunit.Sdk.XunitException ex)
      {
        var sb = new StringBuilder()
          .AppendLine(ex.Message)
          .AppendLine()
          .AppendLine("################### expected:")
          .AppendLine(expectedXml.ToString())
          .AppendLine()
          .AppendLine("################### actual:")
          .AppendLine(actualXml.ToString());

        throw new Xunit.Sdk.XunitException(sb.ToString());
      }
    }

    private static XDocument Load(string text)
    {
      using (var reader = XmlReader.Create(new StringReader(text), new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment }))
      {
        reader.MoveToContent();
        return XDocument.Load(reader);
      }
    }  
      
    public static void AreJsonObjectsEqual(string expected, string actual)
    {
      var expectedObject = JsonConvert.DeserializeObject<JToken>(expected);
      var actualObject = JsonConvert.DeserializeObject<JToken>(actual);
      try
      {
        if (!JToken.DeepEquals(expected, actual))
        {
          Xunit.Assert.Equal(expectedObject.ToString(), actualObject.ToString());
        }
      }
      catch (Xunit.Sdk.XunitException ex)
      {
        var sb = new StringBuilder()
          .AppendLine("################### expected:")
          .AppendLine(JsonConvert.SerializeObject(expectedObject, Newtonsoft.Json.Formatting.Indented))
          .AppendLine()
          .AppendLine("################### actual:")
          .AppendLine(JsonConvert.SerializeObject(actualObject, Newtonsoft.Json.Formatting.Indented))
          .AppendLine()
          .AppendLine(ex.Message);

        throw new Xunit.Sdk.XunitException(sb.ToString());
      }
    }

    public static void AssertCollection<T>(List<T> expected, List<T> actual, Action<T, T> assertItem)
    {
      if (expected == null)
      {
        Assert.Null(actual);
      }
      else
      {
        Assert.NotNull(actual);
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0; i < expected.Count; i++)
        {
          assertItem(expected[i], actual[i]);
        }
      }
    }    
  }
}