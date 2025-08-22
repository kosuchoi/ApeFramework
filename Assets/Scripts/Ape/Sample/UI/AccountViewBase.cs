using TMPro;
using UnityEngine.UI;

namespace Ape.Sample.UI
{
    public abstract class AccountViewBase : Runtime.UI.ViewBase<AccountViewBase, AccountPresenter>
    {
        protected TextMeshProUGUI nicknameText;
        protected TextMeshProUGUI levelText;
        
        protected abstract string nicknameTextPath { get; }
        protected abstract string levelTextPath { get; }
        protected abstract string changeNicknameButtonPath { get; }
        protected abstract string increaseLevelButtonPath { get; }

        protected override void Awake()
        {
            base.Awake();
            
            nicknameText = transform.Find(nicknameTextPath).GetComponent<TextMeshProUGUI>();
            levelText = transform.Find(levelTextPath).GetComponent<TextMeshProUGUI>();
            transform.Find(changeNicknameButtonPath).GetComponent<Button>().onClick
                .AddListener(OnChangeNicknameButtonClick);
            transform.Find(increaseLevelButtonPath).GetComponent<Button>().onClick
                .AddListener(OnIncreaseLevelButtonClick);
        }
        
        protected void OnChangeNicknameButtonClick()
        {
            _presenter.ChangeNickname();
        }

        protected void OnIncreaseLevelButtonClick()
        {
            _presenter.IncreaseLevel();
        }
        
        public void UpdateContent(string nickname, int level)
        {
            nicknameText.text = nickname;
            levelText.text = level.ToString();
        }
    }
}