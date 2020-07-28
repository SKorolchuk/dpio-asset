using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Deeproxio.Asset.DAL.Contracts;
using Deeproxio.Asset.DAL.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minio.DataModel;
using Moq;

namespace Deeproxio.Asset.DAL.Tests
{
    [TestClass]
    public class StorageRepositoryTests
    {
        private MockRepository _mockRepository;
        private Mock<IStorageContext> _storageContextMock;
        private Mock<IStorageSettings> _storageSettingsMock;
        private Mock<ILogger<StorageRepository>> _loggerMock;
        private StorageRepository _target;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose);
            _storageContextMock = _mockRepository.Create<IStorageContext>();
            _storageSettingsMock = _mockRepository.Create<IStorageSettings>();
            _loggerMock = _mockRepository.Create<ILogger<StorageRepository>>();

            _storageSettingsMock.Setup(settings => settings.BucketName).Returns("BucketName");

            _target = new StorageRepository(_storageContextMock.Object, _storageSettingsMock.Object, _loggerMock.Object);
        }

        [TestCleanup]
        public void Clean()
        {
            _mockRepository.Verify();
        }

        [TestMethod]
        public void Constructor_WhenAnyDependencyIsNull_ShouldThrowNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new StorageRepository(
                    null,
                    _storageSettingsMock.Object,
                    _loggerMock.Object)
                );
            Assert.ThrowsException<ArgumentNullException>(() =>
                new StorageRepository(
                    _storageContextMock.Object,
                    null,
                    _loggerMock.Object)
                );
            Assert.ThrowsException<ArgumentNullException>(() =>
                new StorageRepository(
                    _storageContextMock.Object,
                    _storageSettingsMock.Object,
                    null)
                );
        }

        [TestMethod]
        public async Task DeleteAsync_WhenStorageContextThrowsError_ShouldLogWarningAndReturnFalse()
        {
            StubEnsureBucketExists();

            _storageContextMock
                .Setup(context => context.StorageObjects.RemoveObjectAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Throws<ArgumentOutOfRangeException>()
                .Verifiable();

            Assert.IsFalse(await _target.DeleteAsync(string.Empty));
        }

        [TestMethod]
        public async Task DeleteAsync_WhenStorageContextAcceptOperation_ShouldReturnTrue()
        {
            StubEnsureBucketExists();

            _storageContextMock
                .Setup(context => context.StorageObjects.RemoveObjectAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Returns(Task.CompletedTask)
                .Verifiable();

            Assert.IsTrue(await _target.DeleteAsync(string.Empty));
        }

        [TestMethod]
        public void GetByIdAsync_WhenStorageContextThrowsError_ShouldRethrowException()
        {
            StubEnsureBucketExists();

            _storageContextMock
                .Setup(context => context.StorageObjects.GetObjectAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<Action<Stream>>(),
                        It.IsAny<ServerSideEncryption>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Throws<ArgumentOutOfRangeException>()
                .Verifiable();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.GetByIdAsync(string.Empty, null));
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenStorageContextAcceptOperation_ShouldReturnTrue()
        {
            StubEnsureBucketExists();

            _storageContextMock
                .Setup(context => context.StorageObjects.GetObjectAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<Action<Stream>>(),
                        It.IsAny<ServerSideEncryption>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _target.GetByIdAsync(string.Empty, null);
        }

        [TestMethod]
        public void PutAsync_WhenStorageContextThrowsError_ShouldRethrowException()
        {
            StubEnsureBucketExists();

            _storageContextMock
                .Setup(context => context.StorageObjects.PutObjectAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<Stream>(),
                        It.IsAny<long>(),
                        It.IsAny<string>(),
                        It.IsAny<Dictionary<string, string>>(),
                        It.IsAny<ServerSideEncryption>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Throws<ArgumentOutOfRangeException>()
                .Verifiable();

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _target.PutAsync(string.Empty, Mock.Of<Stream>()));
        }

        [TestMethod]
        public async Task PutAsync_WhenStorageContextAcceptOperation_ShouldReturnTrue()
        {
            StubEnsureBucketExists();

            _storageContextMock
                .Setup(context => context.StorageObjects.PutObjectAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<Stream>(),
                        It.IsAny<long>(),
                        It.IsAny<string>(),
                        It.IsAny<Dictionary<string, string>>(),
                        It.IsAny<ServerSideEncryption>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Returns(Task.CompletedTask)
                .Verifiable();

            Assert.IsTrue(await _target.PutAsync(string.Empty, Mock.Of<Stream>()));
        }

        private void StubEnsureBucketExists()
        {
            _storageContextMock
                .Setup(context => context.BucketObjects.BucketExistsAsync(
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Returns(Task.FromResult(false));

            _storageContextMock
                .Setup(context => context.BucketObjects.MakeBucketAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()
                        )
                ).Returns(Task.CompletedTask);
        }
    }
}
