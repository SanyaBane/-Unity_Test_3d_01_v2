namespace Assets.Scripts.Stats
{
    public abstract class BaseCachedStat<T> where T : new()
    {
        protected T _wrappedValue = new T();

        public T GetWrapped()
        {
            if (!_isCachedThisFrame)
            {
                Calculate();
                _isCachedThisFrame = true;
            }

            return _wrappedValue;
        }

        private bool _isCachedThisFrame = false;

        public void ResetCache()
        {
            _isCachedThisFrame = false;
        }

        protected abstract void Calculate();
    }
}