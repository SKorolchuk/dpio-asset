using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Deeproxio.Asset.DAL.Contracts;
using Deeproxio.Asset.DAL.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;

namespace Deeproxio.Asset.DAL.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AssetRepositoryTests
    {
        private MockRepository _mockRepository;
        private AssetRepository _target;
        private Mock<IAssetDataContext> _assetDataContextMock;
        private Mock<IMongoCollection<BLL.Contract.Entities.Asset>> _assetCollectionMock;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _assetDataContextMock = _mockRepository.Create<IAssetDataContext>();
            _target = new AssetRepository(_assetDataContextMock.Object);
            _assetCollectionMock = new Mock<IMongoCollection<BLL.Contract.Entities.Asset>>();

            SetupInitialAssets();
        }

        [TestCleanup]
        public void Clean()
        {
            _mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Constructor_WhenAssetDataContextIsNull_ShouldThrowNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new AssetRepository(null));
        }

        [TestMethod]
        public void CreateAsync_WhenAssetDataContextThrowsError_ShoudRethrowException()
        {
            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.CreateAsync(GetAsset()));
        }

        [TestMethod]
        public async Task CreateAsync_WhenAssetIsNull_ShouldNotSaveAssetAndReturnFalse()
        {
            Assert.IsFalse(await _target.CreateAsync(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenAssetIsNotNull_ShouldSaveAssetAndReturnTrue()
        {
            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Returns(_assetCollectionMock.Object);

            var newAsset = GetAsset();

            Assert.IsTrue(await _target.CreateAsync(newAsset));
        }

        [TestMethod]
        public void DeleteAsync_WhenAssetDataContextThrowsError_ShouldRethrowException()
        {
            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Returns(_assetCollectionMock.Object);
            _assetCollectionMock
                .Setup(collection => collection.DeleteOneAsync(
                        It.IsAny<FilterDefinition<BLL.Contract.Entities.Asset>>(),
                        It.IsAny<CancellationToken>()
                        )
                )
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.DeleteAsync(string.Empty));
        }

        [TestMethod]
        public async Task DeleteAsync_WhenAssetDataContextRejectDelete_ShouldReturnFalse()
        {
            var deleteResultMock = _mockRepository.Create<DeleteResult>();

            deleteResultMock.Setup(result => result.IsAcknowledged).Returns(false);

            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Returns(_assetCollectionMock.Object);
            _assetCollectionMock
                .Setup(collection => collection.DeleteOneAsync(
                        It.IsAny<FilterDefinition<BLL.Contract.Entities.Asset>>(),
                        It.IsAny<CancellationToken>()
                        )
                ).Returns(Task.FromResult(deleteResultMock.Object));

            Assert.IsFalse(await _target.DeleteAsync(string.Empty));
        }

        [TestMethod]
        public async Task DeleteAsync_WhenAssetDataContextAcceptDelete_ShouldReturnTrue()
        {
            var deleteResultMock = _mockRepository.Create<DeleteResult>();

            deleteResultMock.Setup(result => result.IsAcknowledged).Returns(true);

            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Returns(_assetCollectionMock.Object);
            _assetCollectionMock
                .Setup(collection => collection.DeleteOneAsync(
                        It.IsAny<FilterDefinition<BLL.Contract.Entities.Asset>>(),
                        It.IsAny<CancellationToken>()
                        )
                ).Returns(Task.FromResult(deleteResultMock.Object));

            Assert.IsTrue(await _target.DeleteAsync(string.Empty));
        }

        [TestMethod]
        public void GetAllAsync_WhenAssetDataContextThrowsError_ShouldRethrowException()
        {
            _assetCollectionMock
                .Setup(collection => collection.FindAsync(
                        It.IsAny<FilterDefinition<BLL.Contract.Entities.Asset>>(),
                        It.IsAny<FindOptions<BLL.Contract.Entities.Asset, BLL.Contract.Entities.Asset>>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Throws<ArgumentOutOfRangeException>();

            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Returns(_assetCollectionMock.Object);

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.GetAllAsync());
        }

        [TestMethod]
        public async Task GetAllAsync_WhenAssetDataContextFindAssets_ShouldReturnAssets()
        {
            var expected = new List<BLL.Contract.Entities.Asset> { GetAsset(), GetAsset() };

            var assetCollectionCursorMock = new Mock<IAsyncCursor<BLL.Contract.Entities.Asset>>();

            assetCollectionCursorMock
                .Setup(cursor => cursor.Current)
                .Returns(expected);
            assetCollectionCursorMock
                .SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            assetCollectionCursorMock
                .SetupSequence(cursor => cursor.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                 .Returns(Task.FromResult(false));

            _assetCollectionMock
                .Setup(collection => collection.FindAsync(
                        It.IsAny<FilterDefinition<BLL.Contract.Entities.Asset>>(),
                        It.IsAny<FindOptions<BLL.Contract.Entities.Asset, BLL.Contract.Entities.Asset>>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(assetCollectionCursorMock.Object);

            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Returns(_assetCollectionMock.Object);

            var actual = await _target.GetAllAsync();

            Assert.AreEqual(actual.Count(), expected.Count);
        }

        [TestMethod]
        public void GetByIdAsync_WhenAssetDataContextThrowsError_ShouldRethrowException()
        {
            _assetCollectionMock
                 .Setup(collection => collection.FindAsync(
                         It.IsAny<FilterDefinition<BLL.Contract.Entities.Asset>>(),
                         It.IsAny<FindOptions<BLL.Contract.Entities.Asset, BLL.Contract.Entities.Asset>>(),
                         It.IsAny<CancellationToken>()
                     )
                 )
                 .Throws<ArgumentOutOfRangeException>();

            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Returns(_assetCollectionMock.Object);

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.GetByIdAsync(string.Empty));
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenAssetDataContextFindAssets_ShouldReturnAssets()
        {
            var expected = new List<BLL.Contract.Entities.Asset> { GetAsset(), GetAsset() };

            var assetCollectionCursorMock = new Mock<IAsyncCursor<BLL.Contract.Entities.Asset>>();

            assetCollectionCursorMock.Setup(_ => _.Current).Returns(expected);
            assetCollectionCursorMock
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            assetCollectionCursorMock
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                 .Returns(Task.FromResult(false));

            _assetCollectionMock.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<BLL.Contract.Entities.Asset>>(),
                It.IsAny<FindOptions<BLL.Contract.Entities.Asset, BLL.Contract.Entities.Asset>>(),
                 It.IsAny<CancellationToken>())).ReturnsAsync(assetCollectionCursorMock.Object);

            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Returns(_assetCollectionMock.Object);

            var actual = await _target.GetByIdAsync(string.Empty);

            Assert.AreEqual(expected[0], actual);
        }


        [TestMethod]
        public async Task UpdateAsync_WhenAssetIsNull_ShouldNotUpdateAssetAndReturnFalse()
        {
            Assert.IsFalse(await _target.UpdateAsync(null));
        }

        [TestMethod]
        public void UpdateAsync_WhenAssetDataContextThrowsError_ShouldRethrowException()
        {
            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Returns(_assetCollectionMock.Object);
            _assetCollectionMock
                .Setup(collection => collection.ReplaceOneAsync(
                        It.IsAny<FilterDefinition<BLL.Contract.Entities.Asset>>(),
                        It.IsAny<BLL.Contract.Entities.Asset>(),
                        It.IsAny<ReplaceOptions>(),
                        It.IsAny<CancellationToken>()
                        )
                )
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.UpdateAsync(GetAsset()));
        }

        [TestMethod]
        public async Task UpdateAsync_WhenAssetDataContextRejectDelete_ShouldReturnFalse()
        {
            var replaceOneResultMock = _mockRepository.Create<ReplaceOneResult>();

            replaceOneResultMock.Setup(result => result.IsAcknowledged).Returns(false);

            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Returns(_assetCollectionMock.Object);
            _assetCollectionMock
                .Setup(collection => collection.ReplaceOneAsync(
                        It.IsAny<FilterDefinition<BLL.Contract.Entities.Asset>>(),
                        It.IsAny<BLL.Contract.Entities.Asset>(),
                        It.IsAny<ReplaceOptions>(),
                        It.IsAny<CancellationToken>()
                        )
                )
                .Returns(Task.FromResult(replaceOneResultMock.Object));

            Assert.IsFalse(await _target.UpdateAsync(GetAsset()));
        }

        [TestMethod]
        public async Task UpdateAsync_WhenAssetDataContextAcceptDelete_ShouldReturnTrue()
        {
            var replaceOneResultMock = _mockRepository.Create<ReplaceOneResult>();

            replaceOneResultMock.Setup(result => result.IsAcknowledged).Returns(true);

            _assetDataContextMock
                .Setup(assetRepository => assetRepository.Assets)
                .Returns(_assetCollectionMock.Object);
            _assetCollectionMock
                                .Setup(collection => collection.ReplaceOneAsync(
                        It.IsAny<FilterDefinition<BLL.Contract.Entities.Asset>>(),
                        It.IsAny<BLL.Contract.Entities.Asset>(),
                        It.IsAny<ReplaceOptions>(),
                        It.IsAny<CancellationToken>()
                        )
                )
                .Returns(Task.FromResult(replaceOneResultMock.Object));

            Assert.IsTrue(await _target.UpdateAsync(GetAsset()));
        }

        private void SetupInitialAssets()
        {
            _assetCollectionMock.Object.InsertOne(GetAsset());
        }

        private BLL.Contract.Entities.Asset GetAsset(string id = null)
        {
            var assetId = id ?? Guid.NewGuid().ToString();

            return new BLL.Contract.Entities.Asset
            {
                Id = assetId,
                StorageId = Guid.NewGuid().ToString(),
                Info = new BLL.Contract.Entities.AssetInfo
                {
                    BlobExtension = "BlobExtension",
                    BlobMimeType = "BlobMimeType",
                    MediaType = BLL.Contract.Entities.AssetType.Undefined,
                    Metadata = new Dictionary<string, string>(),
                    Name = "Name",
                    StorePrefix = "StorePrefix"
                }
            };
        }
    }
}
