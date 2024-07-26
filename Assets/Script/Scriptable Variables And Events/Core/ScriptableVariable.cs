using UnityEngine;
using UnityEngine.Events;


namespace OI.ScriptableTypes
{
    // This is a generic class that will allow us to create ScriptableObjects that can be used as variables


    public class ScriptableVariable<T> : ScriptableObject
    {
        public delegate void ValueChangedDelegate(T newValue);

        // The internal value of this variable
        [SerializeField] private T _value;

        // The event that will be triggered when the value changes (using delegates)
        [SerializeField] private event ValueChangedDelegate _onValueChanged;

        // The event that will be triggered when the value changes (using UnityEvents)
        [SerializeField] private UnityEvent<T> _eventForValueChanged;



        // Accessor to change the value and trigger the event
        public event ValueChangedDelegate OnValueChanged
        {
            add { _onValueChanged += value; }
            remove { _onValueChanged -= value; }
        }


        // Creation of Listeners

        public void AddListener(ValueChangedDelegate listener)
        {
            _onValueChanged += listener;
        }

        public void RemoveListener(ValueChangedDelegate listener)
        {
            _onValueChanged -= listener;
        }

        public void RemoveAllListeners()
        {
            _onValueChanged = null;
        }


        // Accessor to change the value and trigger the event
        public virtual T Value
        {
            get => _value;
            set
            {
                // With this we avoid triggering the event if the value is the same
                // Question: do we want to trigger the event if the value is the same?
                //if (_value.Equals(value)) return;

                // Set the new value
                _value = value;

                // Propagate the event (with both delegates and UnityEvents)
                RefreshEvent();
            }
        }


        public virtual void RefreshEvent()
        {
            _onValueChanged?.Invoke(_value);
            _eventForValueChanged?.Invoke(_value);

        }

  
    }

}

