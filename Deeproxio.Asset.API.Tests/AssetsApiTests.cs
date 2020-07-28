using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Deeproxio.Asset.API.Services.v1;
using Deeproxio.Asset.BLL.Contract.Services;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Testing;
using Grpc.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Deeproxio.Asset.API.Tests
{
    [TestClass]
    public class AssetsApiTests
    {
        private MockRepository _mockRepository;
        private Mock<IAssetService> _assetServiceMock;
        private Mock<IMapper> _mapperMock;
        private AssetsApi _target;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _assetServiceMock = _mockRepository.Create<IAssetService>();
            _mapperMock = _mockRepository.Create<IMapper>();

            _target = new AssetsApi(_assetServiceMock.Object, _mapperMock.Object);
        }

        [TestCleanup]
        public void Clean()
        {
            _mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task Upload_WhenUploadCompleted_ShouldReturnOKResult()
        {
            var testAsset = new Asset()
            {
                Id = "assetId",
                Info = new AssetInfo(),
                Blob = ByteString.Empty
            };

            var testAssetModel = new BLL.Contract.Entities.Asset();

            _mapperMock
                .Setup(mapper => mapper.Map<BLL.Contract.Entities.Asset>(testAsset))
                .Returns(testAssetModel);

            _assetServiceMock
                .Setup(assetService => assetService.PutAsync(
                    testAssetModel,
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            var uploadResult = await _target.Upload(testAsset, GetFakeContext("Upload"));

            Assert.AreEqual((int)HttpStatusCode.OK, uploadResult.Code);
        }

        [TestMethod]
        public async Task Upload_WhenUploadRejected_ShouldReturnBadRequestResult()
        {
            var testAsset = new Asset()
            {
                Id = "assetId",
                Info = new AssetInfo(),
                Blob = ByteString.Empty
            };

            var testAssetModel = new BLL.Contract.Entities.Asset();

            _mapperMock
                .Setup(mapper => mapper.Map<BLL.Contract.Entities.Asset>(testAsset))
                .Returns(testAssetModel);

            _assetServiceMock
                .Setup(assetService => assetService.PutAsync(
                    testAssetModel,
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            var uploadResult = await _target.Upload(testAsset, GetFakeContext("Upload"));

            Assert.AreEqual((int)HttpStatusCode.BadRequest, uploadResult.Code);
        }

        [TestMethod]
        public async Task Download_WhenAssetFound_ShouldReturnAsset()
        {
            var testAssetRequest = new AssetRequest()
            {
                Id = "assetId"
            };

            var testAssetModel = new BLL.Contract.Entities.Asset();

            var expected = new Asset();

            _mapperMock
                .Setup(mapper => mapper.Map<Asset>(testAssetModel))
                .Returns(expected);

            _assetServiceMock
                .Setup(assetService => assetService.GetByIdAsync(
                    testAssetRequest.Id,
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(testAssetModel));

            var actual = await _target.Download(testAssetRequest, GetFakeContext("Download"));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Download_WhenAssetServiceThrowsError_ShouldRethrowException()
        {
            var testAssetRequest = new AssetRequest()
            {
                Id = "assetId"
            };

            _assetServiceMock
                .Setup(assetService => assetService.GetByIdAsync(
                    testAssetRequest.Id,
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()))
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => 
                _target.Download(testAssetRequest, GetFakeContext("Download")));
        }

        [TestMethod]
        public async Task Info_WhenAssetFound_ShouldReturnAsset()
        {
            var testAssetRequest = new AssetRequest()
            {
                Id = "assetId"
            };

            var testAssetInfoModel = new BLL.Contract.Entities.AssetInfo();

            var expected = new AssetInfo();

            _mapperMock
                .Setup(mapper => mapper.Map<AssetInfo>(testAssetInfoModel))
                .Returns(expected);

            _assetServiceMock
                .Setup(assetService => assetService.GetInfoByIdAsync(
                    testAssetRequest.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(testAssetInfoModel));

            var actual = await _target.Info(testAssetRequest, GetFakeContext("Info"));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Info_WhenAssetServiceThrowsError_ShouldRethrowException()
        {
            var testAssetRequest = new AssetRequest()
            {
                Id = "assetId"
            };

            _assetServiceMock
                .Setup(assetService => assetService.GetInfoByIdAsync(
                    testAssetRequest.Id,
                    It.IsAny<CancellationToken>()))
                .Throws<ArgumentOutOfRangeException>();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>
                _target.Info(testAssetRequest, GetFakeContext("Info")));
        }

        [TestMethod]
        public async Task Delete_WhenDeleteCompleted_ShouldReturnOKResult()
        {
            var testAssetRequest = new AssetRequest()
            {
                Id = "assetId"
            };

            _assetServiceMock
                .Setup(assetService => assetService.DeleteAsync(
                    testAssetRequest.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            var deleteResult = await _target.Delete(testAssetRequest, GetFakeContext("Delete"));

            Assert.AreEqual((int)HttpStatusCode.OK, deleteResult.Code);
        }

        [TestMethod]
        public async Task Delete_WhenDeleteRejected_ShouldReturnBadRequestResult()
        {
            var testAssetRequest = new AssetRequest()
            {
                Id = "assetId"
            };

            _assetServiceMock
                .Setup(assetService => assetService.DeleteAsync(
                    testAssetRequest.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            var deleteResult = await _target.Delete(testAssetRequest, GetFakeContext("Delete"));

            Assert.AreEqual((int)HttpStatusCode.BadRequest, deleteResult.Code);
        }

        [TestMethod]
        public async Task UpdateMetadata_WhenUpdateCompleted_ShouldReturnOKResult()
        {
            var updateRequest = new UpdateMetadataRequest()
            {
                Id = "assetId",
                AssetInfo = new AssetInfo()
            };

            var testAssetInfoModel = new BLL.Contract.Entities.AssetInfo();

            _mapperMock
                .Setup(mapper => mapper.Map<BLL.Contract.Entities.AssetInfo>(updateRequest.AssetInfo))
                .Returns(testAssetInfoModel);

            _assetServiceMock
                .Setup(assetService => assetService.PutMetadataAsync(
                    updateRequest.Id,
                    testAssetInfoModel,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            var updateMetadataResult = await _target.UpdateMetadata(updateRequest, GetFakeContext("UpdateMetadata"));

            Assert.AreEqual((int)HttpStatusCode.OK, updateMetadataResult.Code);
        }

        [TestMethod]
        public async Task UpdateMetadata_WhenUpdateRejected_ShouldReturnBadRequestResult()
        {
            var updateRequest = new UpdateMetadataRequest()
            {
                Id = "assetId",
                AssetInfo = new AssetInfo()
            };

            var testAssetInfoModel = new BLL.Contract.Entities.AssetInfo();

            _mapperMock
                .Setup(mapper => mapper.Map<BLL.Contract.Entities.AssetInfo>(updateRequest.AssetInfo))
                .Returns(testAssetInfoModel);

            _assetServiceMock
                .Setup(assetService => assetService.PutMetadataAsync(
                    updateRequest.Id,
                    testAssetInfoModel,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            var updateMetadataResult = await _target.UpdateMetadata(updateRequest, GetFakeContext("UpdateMetadata"));

            Assert.AreEqual((int)HttpStatusCode.BadRequest, updateMetadataResult.Code);
        }

        private ServerCallContext GetFakeContext(string methodName)
        {
            return TestServerCallContext.Create(
                methodName,
                null,
                DateTime.UtcNow.AddHours(1),
                new Metadata(),
                CancellationToken.None,
                "127.0.0.1",
                null,
                null,
                (metadata) => TaskUtils.CompletedTask,
                () => new WriteOptions(),
                (writeOptions) => { });
        }
    }
}
