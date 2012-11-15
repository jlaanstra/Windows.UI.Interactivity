using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Windows.UI.Interactivity
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LoadableAttachableCollection<T> : AttachableCollection<T>, ILifetimeTarget where T: FrameworkElement, IAttachedObject
    {
        private bool isLoaded = false;

        /// <summary>
        /// The saved weak reference to the associated object
        /// </summary>
        private WeakReference<FrameworkElement> savedReference;

        protected override void AttachInternal(FrameworkElement frameworkElement)
        {
            base.AttachInternal(frameworkElement);
            // The observer will be kept alive by the event handlers
            new WeakLifetimeObserver(frameworkElement, this);
        }

        /// <summary>
        /// Handles the Loaded event of the AssociatedObject control.
        /// This is used when a behavior or trigger is attached to an itemcontainer,
        /// which is virtualized and uses recycling.
        /// </summary>
        public void AssociatedObjectLoaded(FrameworkElement elem)
        {
            Debug.WriteLine("{0} {2} loaded to {1}", this.GetType(), (this.AssociatedObject != null) ? this.AssociatedObject.DataContext : null, this.GetHashCode());
            //only try to recover when not attached
            if (this.AssociatedObject == null && !isLoaded)
            {
                isLoaded = true;
                if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    this.AssociatedObject = elem;
                }
                this.OnAttached();
            }
        }

        /// <summary>
        /// Handles the Unloaded event of the AssociatedObject control.
        /// </summary>
        public void AssociatedObjectUnloaded(FrameworkElement elem)
        {
            if (isLoaded)
            {
                isLoaded = false;
                Debug.WriteLine("{0} {2} unloaded from {1}", this.GetType(), (this.AssociatedObject != null) ? this.AssociatedObject.DataContext : null, this.GetHashCode());
                this.Detach();
            }
        }
    }
}
