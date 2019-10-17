using Xunit;

namespace Anixe.QualityTools.Test
{
  public class AssertHelperTest
  {
    [Fact]
    public void AreJsonObjectsEqual_Should_Fail_Assertion_If_Objects_Are_Different()
    {
      var a = "{\"x\":\"x\",  \"y\": \"y\"}";
      var b = "{\"y\": \"y\", \"x\":\"x2\"}";
      var ex = Record.Exception(() => AssertHelper.AreJsonObjectsEqual(a, b));
      Assert.NotNull(ex);
    }

    [Fact]
    public void AreJsonObjectsEqual_Should_Compare_Ignoring_Properties_Order()
    {
      var a = "{\"x\":\"x\",  \"y\": \"y\"}";
      var b = "{\"y\": \"y\", \"x\":\"x\"}";
      var ex = Record.Exception(() => AssertHelper.AreJsonObjectsEqual(a, b));
      Assert.Null(ex);
    }

    [Fact]
    public void AreJsonObjectsSemanticallyEqual_WhenBothJsonsAreEmpty_ItDoesNotFail()
    {
      var a = "{}";
      var b = "{}";

      var ex = Record.Exception(() => AssertHelper.AreJsonObjectsSemanticallyEqual(a, b));

      Assert.Null(ex);
    }

    [Fact]
    public void AreJsonObjectsSemanticallyEqual_WhenBothJsonAreOneLevelDeepWithSameBody_ItDoesNotFail()
    {
      var a = @"{ ""a"": 1,    ""b"": ""foo"" }";
      var b = @"{""b"": ""foo"", ""a"": 1 }";

      var ex = Record.Exception(() => AssertHelper.AreJsonObjectsSemanticallyEqual(a, b));

      Assert.Null(ex);
    }

    [Fact]
    public void AreJsonObjectsSemanticallyEqual_WhenBothJsonsHaveSameComplexBody_ItDoesNotFail()
    {
      var a = @"
                {
                    ""str"": ""bar"",
                    ""num"": 12.90,
                    ""arr"": [
                        {
                            ""bool"": true,
                            ""null"": null,
                            ""int"": 1
                        }
                    ],
                    ""obj"": {
                        ""nested"": {
                            ""num"": 123,
                            ""arr1"": [1, 2, 3],
                            ""arr2"": [{}],
                            ""arr3"": [true, false],
                            ""arr4"": [""foobar""]
                        }
                    }
                }
            ";

      var b = @"
                {
                    ""num"": 12.90,
                    ""obj"": {
                        ""nested"": {
                            ""num"": 123,
                            ""arr1"": [1, 2, 3],
                            ""arr4"": [""foobar""],
                            ""arr3"": [true, false],
                            ""arr2"": [{}]
                        }
                    },
                    ""str"": ""bar"",
                    ""arr"": [
                        {
                            ""int"": 1,
                            ""null"": null,
                            ""bool"": true
                        }
                    ]
                }
            ";

      var ex = Record.Exception(() => AssertHelper.AreJsonObjectsSemanticallyEqual(a, b));

      Assert.Null(ex);
    }

    [Fact]
    public void AreJsonObjectsSemanticallyEqual_WhenArraysHaveDifferentValue_ItFails()
    {
      var a = @"
                {
                    ""arr"": [1, 2, 3]
                }
            ";

      var b = @"
                {
                    ""arr"": [3, 2, 1]
                }
            ";

      var ex = Record.Exception(() => AssertHelper.AreJsonObjectsSemanticallyEqual(a, b));

      Assert.NotNull(ex);

      Assert.Equal(
        @"################### Expected:
{
  ""arr"": [
    1,
    2,
    3
  ]
}

******************* Actual:
{
  ""arr"": [
    3,
    2,
    1
  ]
}

Values for path arr[0] are different.
Expected: 1
Actual: 3
", ex.Message, ignoreLineEndingDifferences : true);
    }

    [Fact]
    public void AreJsonObjectsSemanticallyEqual_WhenObjectInArrayIsMissingProperty_ItFails()
    {
      var a = @"
                {
                    ""arr"": [{""a"": 1}]
                }
            ";

      var b = @"
                {
                    ""arr"": [{""a"": 1, ""b"": 2}]
                }
            ";

      var ex = Record.Exception(() => AssertHelper.AreJsonObjectsSemanticallyEqual(a, b));

      Assert.NotNull(ex);

      Assert.Equal(
@"################### Expected:
{
  ""arr"": [
    {
      ""a"": 1
    }
  ]
}

******************* Actual:
{
  ""arr"": [
    {
      ""a"": 1,
      ""b"": 2
    }
  ]
}

Objects for path arr[0] are different.
Expected:
{
  ""a"": 1
}
Actual:
{
  ""a"": 1,
  ""b"": 2
}
", ex.Message, ignoreLineEndingDifferences : true);
    }

    [Fact]
    public void AreJsonObjectsSemanticallyEqual_WhenObjectsHaveDifferentPropertyNames_ItFails()
    {
      var a = @"
                {
                    ""a"": {""b"": 1}
                }
            ";

      var b = @"
                {
                    ""a"": {""c"": 1}
                }
            ";

      var ex = Record.Exception(() => AssertHelper.AreJsonObjectsSemanticallyEqual(a, b));

      Assert.NotNull(ex);

      Assert.Equal(
@"################### Expected:
{
  ""a"": {
    ""b"": 1
  }
}

******************* Actual:
{
  ""a"": {
    ""c"": 1
  }
}

Property: 'a.b' is missing in actual object
Expected:
{
  ""b"": 1
}
Actual:
{
  ""c"": 1
}
", ex.Message, ignoreLineEndingDifferences : true);
    }

    [Fact]
    public void AreJsonObjectsSemanticallyEqual_WhenArraysHaveDifferentLength_ItFails()
    {
      var a = @"
                {
                    ""a"": {
                      ""b"": [
                        {""c"": 1}
                      ]
                    }
                }
            ";

      var b = @"
                {
                    ""a"": {
                      ""b"": [
                        {""c"": 1 },
                        {""d"": 1 }
                      ]
                }
            ";

      var ex = Record.Exception(() => AssertHelper.AreJsonObjectsSemanticallyEqual(a, b));

      Assert.NotNull(ex);

      Assert.Equal(
@"################### Expected:
{
  ""a"": {
    ""b"": [
      {
        ""c"": 1
      }
    ]
  }
}

******************* Actual:
{
  ""a"": {
    ""b"": [
      {
        ""c"": 1
      },
      {
        ""d"": 1
      }
    ]
  }
}

Arrays for path a.b have different length.
Expected [
  {
    ""c"": 1
  }
]
Actual:
[
  {
    ""c"": 1
  },
  {
    ""d"": 1
  }
]
", ex.Message, ignoreLineEndingDifferences : true);
    }

    [Fact]
    public void AreJsonObjectsSemanticallyEqual_WhenPropertiesForObjectAreOfDifferentType_ItFails()
    {
      var a = @"
                {
                    ""a"": {
                      ""b"": 1
                    }
                }
            ";

      var b = @"
                {
                    ""a"": {
                      ""b"": {}
                    }
                }
            ";

      var ex = Record.Exception(() => AssertHelper.AreJsonObjectsSemanticallyEqual(a, b));

      Assert.NotNull(ex);

      Assert.Equal(
@"################### Expected:
{
  ""a"": {
    ""b"": 1
  }
}

******************* Actual:
{
  ""a"": {
    ""b"": {}
  }
}

Token of path 'a.b' of type Integer is different from 'a.b' of type Object
", ex.Message, ignoreLineEndingDifferences : true);
    }
  }
}