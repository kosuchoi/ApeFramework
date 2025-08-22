using UnityEngine;

namespace Ape.Runtime.UI
{
    public abstract class ViewBase<TView, TPresenter> : MonoBehaviour
        where TView : ViewBase<TView, TPresenter>
        where TPresenter : PresenterBase<TView, TPresenter>, new()
    {
        protected TPresenter _presenter;
        
        protected virtual void Awake()
        {
            _presenter = new TPresenter();
            _presenter.Setup(this as TView);
        }

        protected virtual void OnEnable()
        {
            _presenter.Attach();
        }

        protected virtual void OnDisable()
        {
            _presenter.Detach();
        }
    }
}