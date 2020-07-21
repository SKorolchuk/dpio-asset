using System.Collections.Generic;
using Calzolari.Grpc.Domain;
using Calzolari.Grpc.Net.Client.Validation;
using Grpc.Core;
using static Deeproxio.Asset.API.AssetService;

namespace Deeproxio.Asset.API.Client.v1
{
    public class AssetsApiClient : AssetServiceClient
    {
        public IEnumerable<ValidationTrailers> GetValidationErrors(RpcException exception)
        {
            return exception.GetValidationErrors();
        }
    }
}
