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
    public abstract class AttachableCollection<T> : DependencyObjectCollection<T>, IAttachedObject, ILifetimeTarget
        where T : FrameworkElement, IAttachedObject
    {
        private bool isLoaded = false;

        /// <summary>
        /// The saved weak reference to the associated object
        /// </summary>
        private WeakReference<FrameworkElement> savedReference;

        #region Properties
        
        /// <summary>
        /// Snapshot collection
        /// </summary>
        //private Collection<T> Snapshot { get; set; }

        #endregion

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
            //this.Snapshot = new Collection<T>();
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
                    this.savedReference = new WeakReference<FrameworkElement>(frameworkElement);
                    //this.AssociatedObject = frameworkElement;
                    // The observer will be kept alive by the event handlers
                    new WeakLifetimeObserver(frameworkElement, this);
                }
                //this.OnAttached();
            }
        }

        ///// <summary>
        ///// Handles the Unloaded event of the AssociatedObject control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        //private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    Debug.WriteLine("{0} unloaded", this.GetType());
        //    if(this.AssociatedObject != null)
        //    {
        //        this.AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
        //        this.Detach();
        //    }
        //}

        /// <summary>
        /// Handles the Loaded event of the AssociatedObject control.
        /// This is used when a behavior or trigger is attached to an itemcontainer,
        /// which is virtualized and uses recycling.
        /// </summary>
        public void AssociatedObjectLoaded()
        {
            Debug.WriteLine("{0} {2} loaded to {1}", this.GetType(), (this.AssociatedObject != null) ? this.AssociatedObject.DataContext : null, this.GetHashCode());
            //only try to recover when not attached
            if (this.AssociatedObject == null && !isLoaded)
            {
                isLoaded = true;
                FrameworkElement associatedObject;
                if (savedReference != null && savedReference.TryGetTarget(out associatedObject))
                {
                    if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                    {
                        this.AssociatedObject = associatedObject;
                    }
                    this.OnAttached();
                }
            }
        }

        /// <summary>
        /// Handles the Unloaded event of the AssociatedObject control.
        /// </summary>
        public void AssociatedObjectUnloaded()
        {
            Debug.WriteLine("{0} {2} unloaded from {1}", this.GetType(), (this.AssociatedObject != null) ? this.AssociatedObject.DataContext : null, this.GetHashCode());
            if (this.AssociatedObject != null && isLoaded)
            {
                isLoaded = false;
                this.Detach();
            }
        }

        /// <summary>
        /// Detaches the currently associated DependencyObject
        /// </summary>
        public void Detach()
        {
            //Debug.WriteLine("{0} detaching", this.GetType());
            this.OnDetaching();
            this.AssociatedObject = null;
        }

        public FrameworkElement AssociatedObject { get; private set; } 

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
                        try
                        {
                            //this.VerifyAdd(item);
                            this.ItemAdded(item);
                        }
                        finally
                        {
                            //this.Snapshot.Insert(base.IndexOf(item), item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (T item in e.OldItems)
                    {
                        this.ItemRemoved(item);
                        //this.Snapshot.Remove(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (T item in e.OldItems)
                    {
                        this.ItemRemoved(item);
                        //this.Snapshot.Remove(item);
                    }
                    foreach (T item in e.NewItems)
                    {
                        try
                        {
                            //this.VerifyAdd(item);
                            this.ItemAdded(item);
                        }
                        finally
                        {
                            //this.Snapshot.Insert(base.IndexOf(item), item);
                        }
                    }
                    break;

                case (NotifyCollectionChangedAction.Replace | NotifyCollectionChangedAction.Remove):
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (T item in this) //.Snapshot)
                    {
                        this.ItemRemoved(item);
                    }

                    //this.Snapshot = new Collection<T>();

                    foreach (T item in this)
                    {
                        //this.VerifyAdd(item);
                        this.ItemAdded(item);
                    }
                    break;

                default:
                    return;
            }
        }

        ///// <summary>
        ///// Verifies an item is not already present in the collection
        ///// </summary>
        ///// <param name="item">Item to verify</param>
        //private void VerifyAdd(T item)
        //{
        //    if (this.Snapshot.Contains(item))
        //    {
        //        throw new InvalidOperationException();
        //    }
        //} 

        #endregion
    }
}
