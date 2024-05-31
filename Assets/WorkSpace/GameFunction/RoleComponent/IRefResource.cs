using JetBrains.Annotations;
using WorkSpace.Global.CharacterIllustration;

namespace WorkSpace.GameFunction.RoleComponent
{
    public interface IRefResource
    {
        [CanBeNull]
        public SpriteAsset FindSpriteAsset(string typeCode);

        [CanBeNull]
        public SpriteResourceLoader FindRefResourceLocation(string rawSpriteName);
    }
}