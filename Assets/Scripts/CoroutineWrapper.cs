using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    // https://answers.unity.com/questions/24640/how-do-i-return-a-value-from-a-coroutine.html
    public class CoroutineWrapper
    {
        private readonly MonoBehaviour _ownerGameObject;
        protected IEnumerator _mainIEnumerator;
        private Coroutine _coroutine;

        public Action FinishAction;

        public bool IsInProgress { get; private set; } = false;

        public bool StopCoroutineFlag { get; private set; } = false;

        #region Constructors

        public CoroutineWrapper()
        {
        }

        protected CoroutineWrapper(MonoBehaviour owner)
        {
            _ownerGameObject = owner;
        }

        public CoroutineWrapper(MonoBehaviour owner, IEnumerator mainIEnumerator) : this(owner)
        {
            _mainIEnumerator = mainIEnumerator;
        }

        #endregion

        private IEnumerator Run()
        {
            yield return _mainIEnumerator;

            CoroutineEnds();
        }

        public void StartWrapperCoroutine()
        {
            StopCoroutineFlag = false;
            IsInProgress = true;

            _coroutine = _ownerGameObject.StartCoroutine(Run());
            if (StopCoroutineFlag)
            {
                _ownerGameObject.StopCoroutine(_coroutine);
                CoroutineEnds();
            }
        }

        public void StopWrapperCoroutine()
        {
            StopCoroutineFlag = true;
                
            //if (!IsFinished)
            if (_coroutine != null)
            {
                _ownerGameObject.StopCoroutine(_coroutine);
                CoroutineEnds();
            }
        }

        private void CoroutineEnds()
        {
            IsInProgress = false;
            // _stopCoroutineFlag = false;
            FinishAction?.Invoke();
        }
    }
}