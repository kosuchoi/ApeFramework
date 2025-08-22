using Ape.Runtime.DataModel;
using Ape.Runtime.UI;

namespace Ape.Sample.UI
{
    public class AccountPresenter: PresenterBase<AccountViewBase, AccountPresenter>
    {
        private AccountDataModel _accountDataModel;

        protected override void OnAttach()
        {
            DataModelManager.Instance.TryCreate<AccountDataModel>(out _accountDataModel);
            _accountDataModel.OnDataChanged += OnDataChanged;
            _view.UpdateContent(_accountDataModel.Nickname, _accountDataModel.Level);
        }

        protected override void OnDetach()
        {
            _accountDataModel.OnDataChanged -= OnDataChanged;
            _accountDataModel = null;
        }
        
        public void ChangeNickname()
        {
            _accountDataModel.ChangeNickname();
        }

        public void IncreaseLevel()
        {
            _accountDataModel.IncreaseLevel();
        }
        
        private void OnDataChanged()
        {
            _view.UpdateContent(_accountDataModel.Nickname, _accountDataModel.Level);
        }
    }
}