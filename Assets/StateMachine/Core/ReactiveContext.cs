using System;
using System.Collections.Generic;

namespace StateMachine
{
    /// <summary>
    /// A generic context class that provides reactive property change notifications.
    /// </summary>
    /// <typeparam name="T">The derived context type</typeparam>
    public abstract class ReactiveContext<T> where T : ReactiveContext<T>
    {
        // Dictionary to store property change callbacks
        private readonly Dictionary<string, List<Action>> propertyChangedCallbacks = 
            new Dictionary<string, List<Action>>();
            
        /// <summary>
        /// Registers a callback to be invoked when a property changes.
        /// </summary>
        /// <param name="propertyName">The name of the property to monitor</param>
        /// <param name="callback">The callback to invoke when the property changes</param>
        /// <returns>The context for method chaining</returns>
        public T OnPropertyChanged(string propertyName, Action callback)
        {
            if (!propertyChangedCallbacks.ContainsKey(propertyName))
            {
                propertyChangedCallbacks[propertyName] = new List<Action>();
            }
            
            propertyChangedCallbacks[propertyName].Add(callback);
            return this as T;
        }
        
        /// <summary>
        /// Notifies that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (propertyChangedCallbacks.TryGetValue(propertyName, out var callbacks))
            {
                foreach (var callback in callbacks)
                {
                    callback();
                }
            }
        }
        
        /// <summary>
        /// Sets a property value and notifies if it changed.
        /// </summary>
        /// <typeparam name="TValue">The type of the property</typeparam>
        /// <param name="field">Reference to the backing field</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>True if the property changed, false otherwise</returns>
        protected bool SetProperty<TValue>(ref TValue field, TValue value, string propertyName)
        {
            if (EqualityComparer<TValue>.Default.Equals(field, value))
            {
                return false;
            }
            
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
    }
}
