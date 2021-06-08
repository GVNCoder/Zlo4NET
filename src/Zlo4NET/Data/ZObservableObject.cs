using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Zlo4NET.Helpers;

namespace Zlo4NET.Data
{
    /// <inheritdoc />
    public abstract class ZObservableObject : INotifyPropertyChanged
    {
        #region Public event

        /// <summary>
        /// Occurs, when property changed self value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // Public event

        #region Protected methods

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            // if nothing changed => return
            if (Equals(storage, value))
            {
                // value is not changed
                return false;
            }

            // set value
            storage = value;

            // up event
            OnPropertyChanged(propertyName);

            // value is changed
            return true;
        }

        internal void UpdateAll()
        {
            var type = GetType();
            var inheritProperties = ZObservableHelper.GetObservableObjectPropertiesFromType(type);

            foreach (var inheritProperty in inheritProperties)
            {
                var value = (ZObservableObject) inheritProperty.GetValue(this);
                // recursive call
                value?.UpdateAll();
            }

            var observableProperties = ZObservableHelper.GetObservablePropertiesFromType(type);
            _RaisePropertyChangeEvent(observableProperties.Select(pi => pi.Name));
        }

        internal void UpdateByName(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        private void _RaisePropertyChangeEvent(IEnumerable<string> propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                OnPropertyChanged(propertyName);
            }
        }

        #endregion // Protected methods
    }
}