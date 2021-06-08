using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#pragma warning disable 1591

// ReSharper disable InconsistentNaming

namespace Zlo4NET.ReactiveApi
{
    /// <inheritdoc />
    public abstract class ZReactiveObject : IZReactive
    {
        #region IReactive interface

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Protected methods

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            // check if it can be updated
            var defaultEqualityComparer = EqualityComparer<T>.Default;
            if (defaultEqualityComparer.Equals(field, value))
            {
                return false;
            }

            // update field
            field = value;

            // raise event
            OnPropertyChanged(propertyName);

            return true;
        }

        protected virtual void ReactivateWholeObject()
        {
            var reactiveProperties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes<ZReactivePropertyAttribute>().Any());

            foreach (var reactiveProperty in reactiveProperties)
            {
                //var propertyType = reactiveProperty.PropertyType;
                //if (! propertyType.IsPrimitive)
                //{
                //    // recursive call
                //    var propertyValue = reactiveProperty.GetValue(this) as ReactiveObject;

                //    propertyValue?.ReactivateWholeObject();
                //}

                // raise event
                OnPropertyChanged(reactiveProperty.Name);
            }
        }

        #endregion
    }
}