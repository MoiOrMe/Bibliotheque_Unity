using UnityEngine;
using System;

namespace MyLibrary.Modules.Stats
{
    [Serializable]
    public class Stat
    {
        public float currentValue;
        public float maxValue;

        public event Action<float, float> OnValueChanged;

        public Stat(float max)
        {
            maxValue = max;
            currentValue = max;
        }

        public void Initialize()
        {
            currentValue = maxValue;
        }

        public void Modify(float amount)
        {
            float oldValue = currentValue;
            currentValue = Mathf.Clamp(currentValue + amount, 0, maxValue);

            if (currentValue != oldValue)
            {
                OnValueChanged?.Invoke(currentValue, maxValue);
            }
        }

        public void SetMax(float newMax)
        {
            maxValue = newMax;
            Modify(0);
        }
    }
}