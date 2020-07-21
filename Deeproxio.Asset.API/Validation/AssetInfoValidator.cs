using FluentValidation;

namespace Deeproxio.Asset.API.Validation
{
    public class AssetInfoValidator : AbstractValidator<AssetInfo>
    {
        public AssetInfoValidator()
        {
            RuleFor(info => info.Name).NotEmpty().WithMessage("Name cannot be empty");
            RuleFor(info => info.StorePrefix).NotEmpty().WithMessage("Store Prefix cannot be empty");
            RuleFor(info => info.BlobExtension).NotEmpty().WithMessage("File Extension cannot be empty");
            RuleFor(info => info.BlobMimeType).NotEmpty().WithMessage("File Mime Type cannot be empty");
        }
    }
}
