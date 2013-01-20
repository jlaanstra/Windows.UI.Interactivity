using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Interactivity;
using Windows.UI.Xaml;

namespace App1
{
    class SizeChangedBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
        }

        void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine("!!");
        }
    }
}
