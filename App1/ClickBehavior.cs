using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Interactivity;

namespace App1
{
    public class ClickBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ClickBehavior), new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)this.GetValue(ClickBehavior.CommandProperty); }
            set { this.SetValue(ClickBehavior.CommandProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Tapped += AssociatedObject_Tapped;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Tapped -= AssociatedObject_Tapped;
        }

        void AssociatedObject_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (this.Command != null)
            {
                this.Command.Execute(null);
            }
        }
    }
}
