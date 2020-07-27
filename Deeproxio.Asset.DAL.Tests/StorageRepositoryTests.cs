using System;
using Deeproxio.Asset.DAL.Contracts;
using Deeproxio.Asset.DAL.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _storageContextMock = _mockRepository.Create<IStorageContext>();
            _storageSettingsMock = _mockRepository.Create<IStorageSettings>();
            _loggerMock = _mockRepository.Create<ILogger<StorageRepository>>();

            _target = new StorageRepository(_storageContextMock.Object, _storageSettingsMock.Object, _loggerMock.Object);
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
    }
}
