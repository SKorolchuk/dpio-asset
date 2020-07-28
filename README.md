# Deeproxio Asset Management API

- Build Status (develop branch)
  - [![Build Status](https://dev.azure.com/deeproxio/Deeproxio/_apis/build/status/CI&CD/Deeproxio.Asset.API?branchName=develop)](https://dev.azure.com/deeproxio/Deeproxio/_build/latest?definitionId=11&branchName=develop) 
- Deeproxio.Asset.API.Client NuGet (Azure Artifacts Public): 
  - [![Deeproxio.Asset.API.Client package in pub-dpio-feed feed in Azure Artifacts](https://feeds.dev.azure.com/deeproxio/bdffb589-fa1b-4175-a1f2-258a26440e0e/_apis/public/Packaging/Feeds/99dd3995-6c0d-4a36-8762-ffca067e2672/Packages/38e6e5b2-7eab-4e3d-a9df-924fbdc7150e/Badge)](https://dev.azure.com/deeproxio/Public%20Artifacts/_packaging?_a=package&feed=99dd3995-6c0d-4a36-8762-ffca067e2672&package=38e6e5b2-7eab-4e3d-a9df-924fbdc7150e&preferRelease=true)

Deeproxio.Asset.API is gRPC service for Deeproxio platform which used for file and file metadata management.

Asset API is backed by [Minio](https://min.io/) AWS S3-like cloud native file object storage and [MongoDB](https://www.mongodb.com/) database as a persistence storage.

Technologies: 
- `ASP.NET Core 3+`
- `gRPC`
- `Protobuf`
- `FluentValidation`
- `AutoMapper`
- `MongoDB.Driver`
- `Minio`
- `Polly`
- `Serilog`
- `MSTest Framework`

Service requires access to `Minio` and `MongoDB` instances in standalone version, but Helm chart k8s deployment contains `Minio` and `MongoDB` dependencies.

## Docker

- docker image preparation

  ```bash
  # cd in root git repository
  docker build -t dpio-asset-api:latest -f ./Deeproxio.Asset.API/Dockerfile .
  ```

- docker image standalone usage

  ```bash
  # run command after image build
  docker run -d --name dpio-asset-api --restart always -p 4300:80 --link local-mongo --link local-minio -e AssetsDatabaseSettings__ConnectionString=mongodb://local-mongo:27017 -e StorageSettings__EndpointUrl=local-minio:9000 -e StorageSettings__AccessKey=<your_minio_accesskey> -e StorageSettings__SecretKey=<your_minio_accesskey> -e ENVIRONMENT=Production dpio-asset-api:latest
  ```

  P.S. `local-minio` and `local-mongo` are Minio and MongoDB containers deployed in the same docker environment. Replace to any correct name if it's need. Also replace `ConnectionString`, `AccessKey`, `SecretKey` and `EndpointUrl` values to correct settings.

## Helm

1. Run `docker image preparation` step and create image in local cache.

2. (Optional) Push image to any accesible by your k8s cluster container registry.

3. Create helm package and install release with specification of any required setting from `values.yaml`.

    ```bash
    # cd in chart\dpio-asset-api directory
    helm package .
    helm install test-asset . --set {option_name}=[override_value] --wait
    ```

4. Use `kubectl proxy` or `kubectl port-forward` to access service instance.

## Development

1. Use [BloomRPC](https://github.com/uw-labs/bloomrpc) for debugging gRPC services.
