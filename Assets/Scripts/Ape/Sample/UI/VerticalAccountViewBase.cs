namespace Ape.Sample.UI
{
    public class VerticalAccountViewBase : AccountViewBase
    {
        protected override string nicknameTextPath => "Nickname/ValueText";
        protected override string levelTextPath => "Level/ValueText";
        protected override string changeNicknameButtonPath => "ChangeNicknameButton";
        protected override string increaseLevelButtonPath => "IncreaseLevelButton";
    }
}