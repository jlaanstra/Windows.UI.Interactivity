using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Interactivity
{
    public abstract class InteractivityBase : FrameworkElement, IAttachedObject
    {
        private FrameworkElement associatedObject;
        private Type associatedObjectTypeConstraint;
        private TaskCompletionSource<object> tcs;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractivityBase" /> class.
        /// </summary>
        public InteractivityBase()
        {
            this.tcs = new TaskCompletionSource<object>();
        }

        #endregion

        /// <summary>
        /// Gets the object to which this behavior is attached.
        /// 
        /// </summary>
        protected FrameworkElement AssociatedObject
        {
            get { return this.associatedObject; }
            set { this.associatedObject = value; }
        }

        FrameworkElement IAttachedObject.AssociatedObject
        {
            get { return this.AssociatedObject; }
        }

        internal event EventHandler AssociatedObjectChanged;

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected virtual void OnDetaching()
        {
        }

        protected void OnAssociatedObjectChanged()
        {
            if (this.AssociatedObjectChanged == null)
            {
                return;
            }
            this.AssociatedObjectChanged(this, new EventArgs());
        }
        
        /// <summary>
        /// Gets the type constraint of the associated object.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The associated object type constraint.
        /// </value>
        protected virtual Type AssociatedObjectTypeConstraint
        {
            get { return this.associatedObjectTypeConstraint; }
            set { this.associatedObjectTypeConstraint = value; }
        }

        public virtual void Attach(FrameworkElement frameworkElement)
        {
            if(frameworkElement != null)
            {
                frameworkElement.AddDataContextChangedHandler(this.DataContextChanged);
                if (frameworkElement.DataContext != null && !tcs.Task.IsCompleted)
                {
                    this.DataContextChanged(frameworkElement, EventArgs.Empty);
                }
            }
        }

        public virtual void Detach()
        {
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.RemoveDataContextChangedHandler(this.DataContextChanged);
            }
        }

        private void DataContextChanged(object sender, EventArgs e)
        {
            FrameworkElement elem = sender as FrameworkElement;
            if (elem != null)
            {
                object oldDC = this.DataContext;
                this.SetBinding(FrameworkElement.DataContextProperty,
                        new Binding
                        {
                            Path = new PropertyPath("DataContext"),
                            Source = elem
                        }
                    );
                object newDC = this.DataContext;

                this.OnDataContextChanged(oldDC, newDC);

                if (!tcs.Task.IsCompleted)
                {
                    tcs.SetResult(newDC);
                }
            }
        }

        protected virtual void OnDataContextChanged(object oldValue, object newValue)
        {
        }

        /// <summary>
        /// Configures data context. 
        /// </summary>
        protected async Task<object> ConfigureDataContextAsync()
        {
            return await tcs.Task;
        }
    }    
}
