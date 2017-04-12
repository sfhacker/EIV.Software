/// <summary>
/// 
/// </summary>
namespace EIV.Demo.Model.Tests
{
    using MbUnit.Framework;
    using Model;

    [TestFixture]
    public class ClientTests
    {
        private Client client = null;

        [SetUp]
        public void SetUp()
        {
            this.client = new Client()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Sheppard"
            };
        }

        /// <summary>
        /// Determines whether this instance [can get name].
        /// </summary>
        [Test]
        public void CanGetName()
        {
            Assert.AreEqual(this.client.FirstName, "John");
        }

        /// <summary>
        /// Determines whether this instance [can Get id].
        /// </summary>
        [Test]
        public void CanGetId()
        {
            Assert.AreEqual(this.client.Id, 1);
        }
    }
}