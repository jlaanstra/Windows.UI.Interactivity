using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;

namespace Windows.UI.Interactivity
{
    /// <summary>
    /// Encapsulates state information and zero or more ICommands into an attachable object.
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// This is an infrastructure class. Behavior authors should derive from Behavior&lt;T&gt; instead of from this class.
    /// </remarks>
    public abstract class Behavior : InteractivityBase
    {
        internal Behavior(Type associatedObjectTypeConstraint)
        {
            this.AssociatedObjectTypeConstraint = associatedObjectTypeConstraint;
        }

        /// <summary>
        /// Attaches to the specified object.
        /// 
        /// </summary>
        /// <param name="frameworkElement">The object to attach to.</param><exception cref="T:System.InvalidOperationException">The Behavior is already hosted on a different element.</exception><exception cref="T:System.InvalidOperationException">dependencyObject does not satisfy the Behavior type constraint.</exception>
        public override async void Attach(FrameworkElement frameworkElement)
        {
            if (frameworkElement == this.AssociatedObject)
            {
                return;
            }
            if (this.AssociatedObject != null)
            {
                throw new InvalidOperationException("Cannot host behavior multiple times.");
            }
            if (frameworkElement != null && !this.AssociatedObjectTypeConstraint.GetTypeInfo().IsAssignableFrom(frameworkElement.GetType().GetTypeInfo()))
            {
                throw new InvalidOperationException("Type constraint violated.");
            }
            else
            {
                this.AssociatedObject = frameworkElement;
                this.OnAssociatedObjectChanged();

                base.Attach(frameworkElement);
                //we need to fix the datacontext for databinding to work
                await this.ConfigureDataContextAsync();
                this.OnAttached();
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// 
        /// </summary>
        public override void Detach()
        {
            base.Detach();
            this.OnDetaching();
            this.AssociatedObject = null;
            this.OnAssociatedObjectChanged();
        }        
    }
}
