using FluentValidation;

namespace Deeproxio.Asset.API.Validation
{
    public class AssetValidator : AbstractValidator<Asset>
    {
        public AssetValidator()
        {
            RuleFor(asset => asset.Id).NotEmpty().WithMessage("ID cannot be empty");
            RuleFor(asset => asset.Info).SetValidator(new AssetInfoValidator());
        }
    }
}
