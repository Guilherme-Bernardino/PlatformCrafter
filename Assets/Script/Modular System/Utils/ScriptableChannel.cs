using UnityEngine;
using UnityEngine.Events;

namespace PlatformCrafterModularSystem
{
    public class ScriptableChannel<T> : ScriptableObject
    {
        public delegate void ChannelListener(T o);

        [SerializeField] private event ChannelListener _onChannelEvent;

        // Should I expose this?
        [SerializeField] private UnityEvent _unityChannelEvent;


        [SerializeField] private T _valueToInject;

        public void InjectValue()
        {
            SendMessage(_valueToInject);
        }


        public event ChannelListener OnChannelEvent
        {
            add { _onChannelEvent += value; }
            remove { _onChannelEvent -= value; }
        }

        public void AddChannelListener(ChannelListener s)
        {
            _onChannelEvent += s;
        }

        public void RemoveChannelListener(ChannelListener s)
        {
            _onChannelEvent -= s;
        }

        public void SendMessage(T message)
        {
            _onChannelEvent?.Invoke(message);
            _unityChannelEvent?.Invoke();
        }


    }


}
