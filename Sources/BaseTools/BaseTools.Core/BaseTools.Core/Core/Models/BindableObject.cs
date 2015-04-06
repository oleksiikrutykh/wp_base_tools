namespace BaseTools.Core.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    /// <summary>
    /// Contains logic for notifying UI about property change.  
    /// </summary>
    [DataContract]
    public class BindableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Event used for notifying bindings about property change.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise PropertyChanged for specified property mane
        /// </summary>
        /// <param name="propertyName">Name of changed property</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "RaisePropertyChanged is standart name of method for PropertyChanged raising.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter required for use CallerMemberName attribute.")]
        protected void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //protected void UpdateField<T>(ref T field, T newValue, [CallerMemberName]string propertyName = "")
        //{
        //    if (field != null && newValue != null)
        //    {
 
        //    }

        //    if (field.Equals(newValue))
        //    {

        //    }
        //}
        //    if (object.Equals(item, field))
        //    {
 
        //    }
        //}
    }
}
