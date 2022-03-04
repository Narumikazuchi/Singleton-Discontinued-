using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narumikazuchi.Singleton;

namespace UnitTest;

[TestClass]
public partial class UnitTest
{
    [TestMethod]
    public void TestMethod1()
    {
        Iterator.CreateInstance();
        Iterator test = Iterator.Instance;
        Assert.IsNotNull(test);
    }
}

[Singleton]
public partial class Iterator
{ }