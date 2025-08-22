namespace Ape.Sample.UI
{
    public class HorizontalAccountViewBase : AccountViewBase
    {
        protected override string nicknameTextPath => "Left/Nickname/ValueText";
        protected override string levelTextPath => "Left/Level/ValueText";
        protected override string changeNicknameButtonPath => "Right/ChangeNicknameButton";
        protected override string increaseLevelButtonPath => "Right/IncreaseLevelButton";
    }
}