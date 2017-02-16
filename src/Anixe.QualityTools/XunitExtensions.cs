namespace Xunit
{
    using System.Globalization;
    using Xunit.Sdk;

  public partial class Assert
  {
    public static void Equals(string expected, string actual, CompareOptions opts)
    {
      var ci = CultureInfo.InvariantCulture.CompareInfo;
      int diff = ci.Compare(expected, actual, CompareOptions.IgnoreSymbols);
      if(diff != 0)
      {
        throw new EqualException(expected, actual);
      }
    }
  }
}
