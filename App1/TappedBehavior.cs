using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Interactivity;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using App1.Data;
using Windows.UI.Xaml.Input;

namespace App1
{
    public class TappedBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TappedBehavior), new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)this.GetValue(TappedBehavior.CommandProperty); }
            set { this.SetValue(TappedBehavior.CommandProperty, value); }
        }

        /// <summary>
        /// Dependency property which can be used to bind a parameter which is send to a command as a commandparameter.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
                DependencyProperty.Register(
                    "CommandParameter",
                    typeof(object),
                    typeof(TappedBehavior),
                    new PropertyMetadata(
                        null
                    )
                );

        /// <summary>
        /// Gets or sets the value of the CommandParameter dependency property.
        /// </summary>
        /// <value>
        /// The command parameter.
        /// </value>
        public object CommandParameter
        {
            get { return (object)GetValue(TappedBehavior.CommandParameterProperty); }
            set { SetValue(TappedBehavior.CommandParameterProperty, value); }
        }

        protected override void OnAttached()
        {
            Debug.WriteLine("Attached to {0}", this.AssociatedObject.DataContext);
            base.OnAttached();
            this.AssociatedObject.IsTapEnabled = true;
            this.AssociatedObject.Tapped += AssociatedObject_Tapped;
        }

        protected override void OnDetaching()
        {
            Debug.WriteLine("Detaching from {0}", this.AssociatedObject.DataContext);
            base.OnDetaching();
            this.AssociatedObject.IsTapEnabled = false;
            this.AssociatedObject.Tapped -= AssociatedObject_Tapped;
        }

        void AssociatedObject_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.Command != null && this.Command.CanExecute(this.CommandParameter))
            {
                this.Command.Execute(this.CommandParameter);
            }
        }
    }
}
