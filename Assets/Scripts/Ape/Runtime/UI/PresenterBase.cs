namespace Ape.Runtime.UI
{
    public abstract class PresenterBase<TView, TPresenter>
        where TPresenter : PresenterBase<TView, TPresenter>
    {
        protected TView _view;

        internal void Setup(TView view)
        {
            _view = view;
        }

        internal void Attach()
        {
            OnAttach();
        }

        internal void Detach()
        {
            OnDetach();
        }
        
        protected abstract void OnAttach();
        
        protected abstract void OnDetach();
    }
}