namespace Dodgyrabbit.MidiBle
{
    using Tmds.DBus;
    using Xunit;

    public class ApplicationTest
    {
        [Fact]
        public void EmptyApplicationTest()
        {
            var application = new Application("/com/test");
            var result = application.GetManagedObjectsAsync().Result;
            Assert.Equal(0, result.Count);
            Assert.Equal(new ObjectPath("/com/test"), application.ObjectPath);
        }

        [Fact]
        public void Gatt1ServiceTest()
        {
            var application = new Application("/com/test");

            GattService1 service = new GattService1(new ObjectPath("/com/test/gatt1service"), 0, "ABC", false);
            application.AddService(service);

            var result = application.GetManagedObjectsAsync().Result;

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result[new ObjectPath("/com/test/gatt1service/service0")].Count);
        }
    }
}