using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace Anixe.QualityTools
{
  public static class AssertHelper
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
        var expectedMsg = expectedXml.ToString();
        var actualMsg = actualXml.ToString();

        var sb = new StringBuilder(expectedMsg.Length + actualMsg.Length + ex.Message.Length + 70)
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

    public static void AreJsonObjectsEqual(string expected, string actual)
    {
      var actualObject = JToken.Parse(actual);
      var expectedObject = JToken.Parse(expected);
      actualObject.Should().BeEquivalentTo(expectedObject);
    }

    public static void AreJsonObjectsSemanticallyEqual(string expected, string actual, IList<string>? excludePaths = null)
    {
      var expectedObject = JsonConvert.DeserializeObject<JToken>(expected);
      var actualObject = JsonConvert.DeserializeObject<JToken>(actual);

      try
      {
        SemanticallyEqual(expectedObject!, actualObject!, excludePaths);
      }
      catch (Xunit.Sdk.XunitException ex)
      {
        var expectedMsg = JsonConvert.SerializeObject(expectedObject, Formatting.Indented);
        var actualMsg = JsonConvert.SerializeObject(actualObject, Formatting.Indented);

        var sb = new StringBuilder(expectedMsg.Length + actualMsg.Length + ex.Message.Length + 70)
          .AppendLine("################### Expected:")
          .AppendLine(expectedMsg)
          .AppendLine()
          .AppendLine("******************* Actual:")
          .AppendLine(actualMsg)
          .AppendLine()
          .AppendLine(ex.Message);

        throw new Xunit.Sdk.XunitException(sb.ToString());
      }
    }

    public static void AssertCollection<T>(List<T>? expected, List<T> actual, Action<T, T> assertItem)
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

    private static void SemanticallyEqual(JToken left, JToken right, IList<string>? excludePaths = null)
    {
      if (left.Type != right.Type)
      {
        throw new Xunit.Sdk.XunitException($"Token of path '{left.Path}' of type {left.Type} is different from '{right.Path}' of type {right.Type}");
      }

      if (SkipExcluded(left.Path, excludePaths))
      {
        return;
      }

      switch (left.Type)
      {
        case (JTokenType.Object):
          var leftObject = (JObject)left;
          var rightObject = (JObject)right;

          if (leftObject.Count != rightObject.Count)
          {
            throw new Xunit.Sdk.XunitException($"Objects for path {left.Path} are different.{Environment.NewLine}Expected:{Environment.NewLine}{leftObject.ToString()}{Environment.NewLine}Actual:{Environment.NewLine}{rightObject.ToString()}");
          }

          foreach (var leftObjectItem in leftObject)
          {
            JToken? rightObjectItem;

            if (rightObject.TryGetValue(leftObjectItem.Key, out rightObjectItem))
            {
              if (leftObjectItem.Value != null && rightObjectItem != null)
              {
                SemanticallyEqual(leftObjectItem.Value, rightObjectItem, excludePaths);
              }
            }
            else
            {
              throw new Xunit.Sdk.XunitException($"Property: '{leftObject.Path}.{leftObjectItem.Key}' is missing in actual object{Environment.NewLine}Expected:{Environment.NewLine}{leftObject.ToString()}{Environment.NewLine}Actual:{Environment.NewLine}{rightObject.ToString()}");
            }
          }

          break;
        case (JTokenType.Array):
          var leftChildren = (JArray)left;
          var rightChildren = (JArray)right;

          if (leftChildren.Count != rightChildren.Count)
          {
            throw new Xunit.Sdk.XunitException($"Arrays for path {left.Path} have different length.{Environment.NewLine}Expected {left.ToString()}{Environment.NewLine}Actual:{Environment.NewLine}{right.ToString()}");
          }

          for (int i = 0; i < leftChildren.Count; i++)
          {
            SemanticallyEqual(leftChildren[i], rightChildren[i], excludePaths);
          }

          break;

        default: // non composite data structure found
          if (!JToken.DeepEquals(left, right))
          {
            throw new Xunit.Sdk.XunitException($"Values for path {left.Path} are different.{Environment.NewLine}Expected: {left.ToString()}{Environment.NewLine}Actual: {right.ToString()}");
          }

          break;
      }
    }

    private static bool SkipExcluded(string path, IList<string>? excludePaths)
    {
      if (excludePaths == null)
      {
        return false;
      }

      return excludePaths.Contains(path);
    }
  }
}