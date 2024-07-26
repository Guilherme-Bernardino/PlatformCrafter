using System;
using UnityEngine;

namespace OI.ScriptableTypes
{

    public class ScriptablePrefVariable<T> : ScriptableInitVariable<T> 
    {
        [SerializeField] private string _prefKey;

        public string PrefKey => _prefKey;

        //[SerializeField] private T _defaultValue;
        

        public virtual void Save()
        {
            if (KeyIsValid())
            {
                if (Value != null)
                {
                    Write();
                    PlayerPrefs.Save();
                }
            }
        }

        public virtual void Load()
        {
            if (!KeyIsValid())
            {
                Value = _initValue;
                return;
            }

            if (PlayerPrefs.HasKey(_prefKey))
            {
                try
                {
                    Read();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading " + _prefKey + " from PlayerPrefs: " + e.Message);
                    Value = _initValue;
                }
            }

            else
            {
                Value = _initValue;
            }

        }

        private bool KeyIsValid()
        {
            return _prefKey != null && _prefKey != "";
        }

        private void OnEnable()
        {
            Load();
        }

        public override T Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                Save();
            }
        }

        public virtual void Write ()
        {
            if (Value!=null)
            {
                PlayerPrefs.SetString(_prefKey, JsonUtility.ToJson(Value));
            }
        }

        public virtual void Read ()
        {
            // Creates a new object of type T
            T o = (T)JsonUtility.FromJson(PlayerPrefs.GetString(_prefKey), typeof(T));
            Value = o;
        }


        public override void RefreshEvent()
        {
            base.RefreshEvent();
            Save();
        }

    }


}
