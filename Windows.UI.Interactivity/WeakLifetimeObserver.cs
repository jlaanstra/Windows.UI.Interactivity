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
    /// Fires off notifications about the specified object's lifetime events (Loaded and Unloaded), without keeping a hard reference to the target.
    /// </summary>
    public class WeakLifetimeObserver
    {
        private WeakReference<ILifetimeTarget> weakReference;

        public WeakLifetimeObserver(FrameworkElement observedObject, ILifetimeTarget reference)
        {
            this.weakReference = new WeakReference<ILifetimeTarget>(reference);

            observedObject.Loaded += observedObject_Loaded;
            observedObject.Unloaded += observedObject_Unloaded;
        }

        ~WeakLifetimeObserver()
        {
            Debug.WriteLine("Lifetime Observer finalized");
        }

        private void observedObject_Loaded(object sender, RoutedEventArgs e)
        {
            ILifetimeTarget reference;
            if (this.weakReference.TryGetTarget(out reference))
            {
                reference.AssociatedObjectLoaded();
            }
        }

        private void observedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            ILifetimeTarget reference;
            if (this.weakReference.TryGetTarget(out reference))
            {
                reference.AssociatedObjectUnloaded();
            }
        }
    }
}
