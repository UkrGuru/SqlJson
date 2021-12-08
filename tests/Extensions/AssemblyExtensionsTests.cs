using Xunit;

namespace System.Reflection.Tests
{
    public class AssemblyExtensionsTests
    {
        [Fact]
        public void ExecResourceTest()
        {
            var ex1 = Assert.Throws<ArgumentNullException>(() => ((Assembly)null).ExecResource(null));
            Assert.Equal("assembly", ex1.ParamName);

            var ex2 = Assert.Throws<ArgumentNullException>(() => Assembly.GetExecutingAssembly().ExecResource(null));
            Assert.Equal("resourceName", ex2.ParamName);
        }
    }
}