using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        private Mock<IStorageRepository> _storageRepositoryMock;
        private Mock<IStorageItemPathProvider> _storageItemPathProviderMock;
        private AssetService _target;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _assetRepositoryMock = _mockRepository.Create<IAssetRepository>();
            _storageRepositoryMock = _mockRepository.Create<IStorageRepository>();
            _storageItemPathProviderMock = _mockRepository.Create<IStorageItemPathProvider>();

            _target = new AssetService(_assetRepositoryMock.Object, _storageRepositoryMock.Object, _storageItemPathProviderMock.Object);
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
                    _storageRepositoryMock.Object,
                    _storageItemPathProviderMock.Object)
                );
            Assert.ThrowsException<ArgumentNullException>(() =>
                new AssetService(
                    _assetRepositoryMock.Object,
                    null,
                    _storageItemPathProviderMock.Object)
                );
            Assert.ThrowsException<ArgumentNullException>(() =>
                new AssetService(
                    _assetRepositoryMock.Object,
                    _storageRepositoryMock.Object,
                    null)
                );
        }

        [TestMethod]
        [DataRow("assetId")]
        public void GetByIdAsync_WhenAssetRepositoryThrowError_ShouldRethrowException(string assetId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.GetByIdAsync(
                assetId,
                Mock.Of<Stream>(),
                CancellationToken.None)
            );
        }

        [TestMethod]
        [DataRow("assetId")]
        public async Task GetByIdAsync_WhenAssetRepositoryReturnsNull_ShouldReturnNull(string assetId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Contract.Entities.Asset>(null));

            Assert.IsNull(await _target.GetByIdAsync(
                assetId,
                Mock.Of<Stream>(),
                CancellationToken.None));
        }

        [TestMethod]
        [DataRow("assetId", "assetStorageId")]
        public void GetByIdAsync_WhenStorageRepositoryThrowError_ShouldRethrowException(string assetId, string assetStorageId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Contract.Entities.Asset { StorageId = assetStorageId }));

            _storageRepositoryMock
                .Setup(storageRepository => storageRepository.GetByIdAsync(assetStorageId, It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.GetByIdAsync(
                assetId,
                Mock.Of<Stream>(),
                CancellationToken.None)
            );
        }

        [TestMethod]
        [DataRow("assetId", "assetStorageId")]
        public async Task GetByIdAsync_WhenDataFound_ShouldReturnAsset(string assetId, string assetStorageId)
        {
            var expected = new Contract.Entities.Asset { StorageId = assetStorageId };

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expected));

            _storageRepositoryMock
                .Setup(storageRepository => storageRepository.GetByIdAsync(assetStorageId, It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var actual = await _target.GetByIdAsync(
                assetId,
                Mock.Of<Stream>(),
                CancellationToken.None);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("assetId")]
        public void GetInfoByIdAsync_WhenAssetRepositoryThrowError_ShouldRethrowException(string assetId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.GetInfoByIdAsync(
                assetId,
                CancellationToken.None)
            );
        }

        [TestMethod]
        [DataRow("assetId")]
        public async Task GetInfoByIdAsync_WhenAssetRepositoryReturnsNull_ShouldReturnNull(string assetId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Contract.Entities.Asset>(null));

            Assert.IsNull(await _target.GetInfoByIdAsync(
                assetId,
                CancellationToken.None));
        }

        [TestMethod]
        [DataRow("assetId")]
        public async Task GetInfoByIdAsync_WhenDataFound_ShouldReturnAssetInfo(string assetId)
        {
            var expected = new Contract.Entities.Asset { Info = new Contract.Entities.AssetInfo() };

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expected));

            var actual = await _target.GetInfoByIdAsync(
                assetId,
                CancellationToken.None);

            Assert.AreEqual(expected.Info, actual);
        }

        [TestMethod]
        [DataRow("assetId")]
        public void PutMetadataAsync_WhenAssetRepositoryThrowError_ShouldRethrowException(string assetId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.PutMetadataAsync(
                assetId,
                null,
                CancellationToken.None)
            );
        }

        [TestMethod]
        [DataRow("assetId")]
        public async Task PutMetadataAsync_WhenAssetRepositoryReturnsNull_ShouldReturnFalse(string assetId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Contract.Entities.Asset>(null));

            Assert.IsFalse(await _target.PutMetadataAsync(
                assetId,
                null,
                CancellationToken.None));
        }

        [TestMethod]
        [DataRow("assetId")]
        public void PutMetadataAsync_WhenDataFoundAndUpdateRejected_ShouldRethrowException(string assetId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Contract.Entities.Asset()));

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.UpdateAsync(It.IsAny<Contract.Entities.Asset>(), It.IsAny<CancellationToken>()))
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.PutMetadataAsync(
                assetId,
                null,
                CancellationToken.None)
            );
        }

        [TestMethod]
        [DataRow("assetId")]
        public async Task PutMetadataAsync_WhenDataFoundAndUpdated_ShouldReturnTrue(string assetId)
        {
            var expected = new Contract.Entities.AssetInfo();

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Contract.Entities.Asset { Info = null }));

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.UpdateAsync(
                    It.Is<Contract.Entities.Asset>(asset => asset.Info.Equals(expected)),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            Assert.IsTrue(await _target.PutMetadataAsync(
                assetId,
                expected,
                CancellationToken.None));
        }

        [TestMethod]
        [DataRow("assetId")]
        public void DeleteAsync_WhenAssetRepositoryThrowError_ShouldRethrowException(string assetId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.DeleteAsync(
                assetId,
                CancellationToken.None)
            );
        }

        [TestMethod]
        [DataRow("assetId")]
        public async Task DeleteAsync_WhenAssetRepositoryReturnsNull_ShouldReturnFalse(string assetId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Contract.Entities.Asset>(null));

            Assert.IsFalse(await _target.DeleteAsync(
                assetId,
                CancellationToken.None));
        }

        [TestMethod]
        [DataRow("assetId")]
        public void DeleteAsync_WhenDataFoundAndStorageDeleteThrowsError_ShouldRethrowException(string assetId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Contract.Entities.Asset()));

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.DeleteAsync(assetId, It.IsAny<CancellationToken>()))
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.DeleteAsync(
                assetId,
                CancellationToken.None)
            );
        }

        [TestMethod]
        [DataRow("assetId")]
        public async Task DeleteAsync_WhenDataFoundAndStorageDeleteRejected_ShouldReturnFalse(string assetId)
        {
            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Contract.Entities.Asset()));

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.DeleteAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            Assert.IsFalse(await _target.DeleteAsync(
                assetId,
                CancellationToken.None));
        }

        [TestMethod]
        [DataRow("assetId1", "assetStorageId2")]
        public async Task DeleteAsync_WhenDataFoundAndUpdated_ShouldReturnTrue(string assetId, string assetStorageId)
        {
            var expected = new Contract.Entities.Asset() { StorageId = assetStorageId };

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expected));

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.DeleteAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            _storageRepositoryMock
                .Setup(storageRepository => storageRepository.DeleteAsync(assetStorageId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            Assert.IsTrue(await _target.DeleteAsync(
                assetId,
                CancellationToken.None));
        }

        [TestMethod]
        [DataRow("assetId")]
        public void PutAsync_WhenAssetRepositoryThrowError_ShouldRethrowException(string assetId)
        {
            var expected = new Contract.Entities.Asset() { Id = assetId };

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.PutAsync(
                expected,
                null,
                CancellationToken.None)
            );
        }

        [TestMethod]
        [DataRow("assetId")]
        public void PutAsync_WhenDataFoundAndUpdateByAssetRepositoryThrowsError_ShouldRethrowException(string assetId)
        {
            var expected = new Contract.Entities.Asset() { Id = assetId };

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expected));

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.UpdateAsync(
                    expected,
                    It.IsAny<CancellationToken>()))
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.PutAsync(
                 expected,
                 null,
                 CancellationToken.None)
            );
        }

        [TestMethod]
        [DataRow("assetId")]
        public async Task PutAsync_WhenDataFoundAndUpdateRejectedByAssetRepository_ShouldReturnFalse(string assetId)
        {
            var expected = new Contract.Entities.Asset() { Id = assetId };

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expected));

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.UpdateAsync(
                    expected,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            Assert.IsFalse(await _target.PutAsync(
                 expected,
                 null,
                 CancellationToken.None));
        }

        [TestMethod]
        [DataRow("assetId", "assetStorageId")]
        public async Task PutAsync_WhenDataFoundAndUpdateAccepted_ShouldReturnTrue(string assetId, string assetStorageId)
        {
            var expected = new Contract.Entities.Asset() { Id = assetId, StorageId = assetStorageId };

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expected));

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.UpdateAsync(
                    expected,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            _storageRepositoryMock
                .Setup(storageRepository => storageRepository.PutAsync(
                    assetStorageId,
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()
                    ))
                .Returns(Task.FromResult(true));

            Assert.IsTrue(await _target.PutAsync(
                 expected,
                 null,
                 CancellationToken.None));
        }

        [TestMethod]
        [DataRow("assetId", "assetStorageId")]
        public async Task PutAsync_WhenDataNotFoundAndUpdateRejectedByStorageRepository_ShouldReturnFalse(string assetId, string assetStorageId)
        {
            var expected = new Contract.Entities.Asset()
            {
                Id = assetId,
                StorageId = assetStorageId,
                Info = new Contract.Entities.AssetInfo
                {
                    StorePrefix = string.Empty
                }
            };

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Contract.Entities.Asset>(null));

            _storageItemPathProviderMock
                .Setup(storageItemPathProvider => storageItemPathProvider.GeneratePath(
                    expected.Info.StorePrefix))
                .Returns(assetStorageId);

            _storageRepositoryMock
                .Setup(storageRepository => storageRepository.PutAsync(
                    assetStorageId,
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()
                    ))
                .Returns(Task.FromResult(false));

            Assert.IsFalse(await _target.PutAsync(
                 expected,
                 null,
                 CancellationToken.None));
        }

        [TestMethod]
        [DataRow("assetId", "assetStorageId")]
        public async Task PutAsync_WhenDataNotFoundAndSaveAccepted_ShouldReturnTrue(string assetId, string assetStorageId)
        {
            var expected = new Contract.Entities.Asset()
            {
                Id = assetId,
                StorageId = assetStorageId,
                Info = new Contract.Entities.AssetInfo
                {
                    StorePrefix = string.Empty
                }
            };

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Contract.Entities.Asset>(null));

            _storageItemPathProviderMock
                .Setup(storageItemPathProvider => storageItemPathProvider.GeneratePath(
                    expected.Info.StorePrefix))
                .Returns(assetStorageId);

            _storageRepositoryMock
                .Setup(storageRepository => storageRepository.PutAsync(
                    assetStorageId,
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()
                    ))
                .Returns(Task.FromResult(true));

            _assetRepositoryMock
                .Setup(assetRepository => assetRepository.CreateAsync(
                    expected,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            Assert.IsTrue(await _target.PutAsync(
                 expected,
                 null,
                 CancellationToken.None));
        }
    }
}
