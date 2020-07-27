using System.Diagnostics.CodeAnalysis;
using Deeproxio.Asset.DAL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Deeproxio.Asset.DAL.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AssetRepositoryTests
    {
        private MockRepository mockRepository;
        private AssetRepository target;

        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            target = new AssetRepository();
        }
    }
}
