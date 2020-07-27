using System;
using Deeproxio.Asset.BLL.Contract.Repositories;
using Deeproxio.Asset.BLL.Contract.Services;
using Deeproxio.Asset.BLL.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Deeproxio.Asset.BLL.Tests
{
    [TestClass]
    public class AssetServiceTests
    {
        private MockRepository _mockRepository;
        private Mock<IAssetRepository> _assetRepositoryMock;
        private Mock<IStorageRepository> _storageRepository;
        private Mock<IStorageItemPathProvider> _storageItemPathProvider;
        private AssetService _target;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _assetRepositoryMock = _mockRepository.Create<IAssetRepository>();
            _storageRepository = _mockRepository.Create<IStorageRepository>();
            _storageItemPathProvider = _mockRepository.Create<IStorageItemPathProvider>();

            _target = new AssetService(_assetRepositoryMock.Object, _storageRepository.Object, _storageItemPathProvider.Object);
        }

        [TestCleanup]
        public void Clean()
        {
            _mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Constructor_WhenAnyDependencyIsNull_ShouldThrowNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new AssetService(
                    null,
                    _storageRepository.Object,
                    _storageItemPathProvider.Object)
                );
            Assert.ThrowsException<ArgumentNullException>(() =>
                new AssetService(
                    _assetRepositoryMock.Object,
                    null,
                    _storageItemPathProvider.Object)
                );
            Assert.ThrowsException<ArgumentNullException>(() =>
                new AssetService(
                    _assetRepositoryMock.Object,
                    _storageRepository.Object,
                    null)
                );
        }
    }
}
