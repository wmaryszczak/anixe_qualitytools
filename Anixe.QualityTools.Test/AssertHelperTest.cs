using Xunit;

namespace Anixe.QualityTools.Test
{
    public class AssertHelperTest
    {
        [Fact]
        public void It_DoesNothing_When_JsonsAreSemanticallyEqual_Empty()
        {
            var a = "{}";
            var b = "{}";

            var ex = Record.Exception(() => AssertHelper.AreJsonObjectsSemanticallyEqual(a, b));

            Assert.Null(ex);
        }

        [Fact]
        public void It_DoesNothing_When_JsonsAreSemanticallyEqual_NoNested()
        {
            var a = @"{ ""a"": 1,    ""b"": ""foo"" }";
            var b = @"{""b"": ""foo"", ""a"": 1 }";

            var ex = Record.Exception(() => AssertHelper.AreJsonObjectsSemanticallyEqual(a, b));

            Assert.Null(ex);
        }

        [Fact]
        public void It_DoesNothing_When_JsonsAreSemanticallyEqual()
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
        public void It_Fails_When_JsonArraySequenceIsDifferent()
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
              ex.Message,
@"################### expected:
{
  ""arr"": [
    1,
    2,
    3
  ]
}

################### actual:
{
  ""arr"": [
    3,
    2,
    1
  ]
}

Assert.Equal() Failure
          ↓ (pos 0)
Expected: 1
Actual:   3
          ↑ (pos 0)
"
            );
        }

        [Fact]
        public void It_Fails_When_MissingProperty()
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
                ex.Message,
@"################### expected:
{
  ""arr"": [
    {
      ""a"": 1
    }
  ]
}

################### actual:
{
  ""arr"": [
    {
      ""a"": 1,
      ""b"": 2
    }
  ]
}

{
  ""a"": 1
} is different from {
  ""a"": 1,
  ""b"": 2
}
");
        }

      [Fact]
      public void It_Fails_WhenDifferentValuesForSameKey()
      {
        var a = @"
                {
                    ""a"": {}
                }
            ";

            var b = @"
                {
                    ""a"": 2
                }
            ";

            var ex = Record.Exception(() => AssertHelper.AreJsonObjectsSemanticallyEqual(a, b));

            Assert.NotNull(ex);

            Assert.Equal(
                ex.Message,
@"################### expected:
{
  ""a"": {}
}

################### actual:
{
  ""a"": 2
}

Token of path 'a' and type Object is different from 'a' of type Integer
");
      }
    }

}
