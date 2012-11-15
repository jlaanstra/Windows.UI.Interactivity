using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Windows.UI.Interactivity
{
    /// <summary>
    /// Implements a collection of attachable objects that can be attached to a DependencyObject
    /// </summary>
    /// <typeparam name="T">The type of elements</typeparam>
    public abstract class AttachableCollection<T> : FrameworkElementCollection<T>, IAttachedObject
        where T : FrameworkElement, IAttachedObject
    {        

        #region Abstracts
        
        /// <summary>
        /// Implemented by derived class notifies an item was added
        /// </summary>
        /// <param name="item">Item added</param>
        internal abstract void ItemAdded(T item);
        /// <summary>
        /// Implemented by derived class notifies an item was removed
        /// </summary>
        /// <param name="item">Item removed</param>
        internal abstract void ItemRemoved(T item);
        /// <summary>
        /// Notifies a DependencyObject has been attached
        /// </summary>
        protected abstract void OnAttached();
        /// <summary>
        /// Notifies a DependencyObject is detaching
        /// </summary>
        protected abstract void OnDetaching(); 

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes the collection
        /// </summary>
        internal AttachableCollection()
        {
            this.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
        }

        ~AttachableCollection()
        {
            Debug.WriteLine("{0} finalized", this.GetType());
        }

        #endregion

        #region IAttachedObject
        
        /// <summary>
        /// Attaches the specified FrameworkElement to the collection
        /// </summary>
        /// <param name="frameworkElement">FrameworkElement to attach</param>
        public void Attach(FrameworkElement frameworkElement)
        {
            //Debug.WriteLine("{0} attached", this.GetType());
            if (frameworkElement != this.AssociatedObject)
            {
                if (this.AssociatedObject != null)
                {
                    throw new InvalidOperationException();
                }
                if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    this.AttachInternal(frameworkElement);                  
                }
            }
        }

        protected virtual void AttachInternal(FrameworkElement frameworkElement)
        {            
        }

        /// <summary>
        /// Detaches the currently associated DependencyObject
        /// </summary>
        public void Detach()
        {
            if (this.AssociatedObject != null)
            {
                //Debug.WriteLine("{0} detaching", this.GetType());
                this.OnDetaching();
                this.AssociatedObject = null;
            }
        }

        public FrameworkElement AssociatedObject { get; protected set; } 

        #endregion

        #region Utils
        
        /// <summary>
        /// Handles elements added to or removed from the collection
        /// </summary>
        /// <param name="sender">source of the event</param>
        /// <param name="e">arguments</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (T item in e.NewItems)
                    {
                        this.ItemAdded(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (T item in e.OldItems)
                    {
                        this.ItemRemoved(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (T item in e.OldItems)
                    {
                        this.ItemRemoved(item);
                    }
                    foreach (T item in e.NewItems)
                    {
                        this.ItemAdded(item);
                    }
                    break;

                case (NotifyCollectionChangedAction.Replace | NotifyCollectionChangedAction.Remove):
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (T item in this)
                    {
                        this.ItemRemoved(item);
                    }

                    foreach (T item in this)
                    {
                        this.ItemAdded(item);
                    }
                    break;

                default:
                    return;
            }
        }

        #endregion
    }
}
