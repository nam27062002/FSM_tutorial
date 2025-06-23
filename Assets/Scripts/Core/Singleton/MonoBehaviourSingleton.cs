using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(-10000)]
    public abstract class MonobehaviorSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance => instance;
        public static bool HasInstance => instance != null;

        protected virtual void OnSingletonAwake()
        {
        }

        protected virtual void OnSingletonDestroy()
        {
        }

        protected virtual void OnSingletonEnable()
        {
        
        }

        protected virtual void OnSingletonDisable()
        {
        
        }

        private void Awake()
        {
            if (!HasInstance)
            {
                instance = this as T;
                OnSingletonAwake();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            OnSingletonEnable();
        }

        private void OnDisable()
        {
            OnSingletonDisable();
        }
    
        private void OnDestroy()
        {
            if (instance == this as T)
            {
                OnSingletonDestroy();
                instance = null;
            }
        }
    }
}