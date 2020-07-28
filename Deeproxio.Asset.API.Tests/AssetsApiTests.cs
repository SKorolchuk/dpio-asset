using AutoMapper;
using Deeproxio.Asset.API.Services.v1;
using Deeproxio.Asset.BLL.Contract.Services;
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
    }
}
