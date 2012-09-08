using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Windows.UI.Interactivity
{
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// Identifies the InheritedDataContext DependencyProperty.
        /// </summary>
        private static readonly DependencyProperty InheritedDataContextProperty =
            DependencyProperty.RegisterAttached(
                "InheritedDataContext",
                typeof(object),
                typeof(FrameworkElementExtensions),
                new PropertyMetadata(null, OnInheritedDataContextChanged));
 
        /// <summary>
        /// Handles changes to the InheritedDataContext DependencyProperty.
        /// </summary>
        /// <param name="d">Instance with property change.</param>
        /// <param name="e">Property change details.</param>
        private static void OnInheritedDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EventHandler handler = d.GetValue(DataContextChangedHandlerProperty) as EventHandler;
            if (handler!=null)
            {
                handler(d,EventArgs.Empty);
            }
        }
 
        /// <summary>
        /// Identifies the DataContextChangedHandler DependencyProperty.
        /// </summary>
        private static readonly DependencyProperty DataContextChangedHandlerProperty =
            DependencyProperty.RegisterAttached(
                "DataContextChangedHandler",
                typeof(EventHandler),
                typeof(FrameworkElementExtensions),
                null);
 
        /// <summary>
        /// Adds the data context changed handler.
        /// </summary>
        /// <param name="element">Element to which the handler is added</param>
        /// <param name="handler">The handler to add</param>
        public static void AddDataContextChangedHandler(this FrameworkElement element, EventHandler handler)
        {
            if (element==null || handler==null)
            {
                return;
            }
 
            if (element.GetValue(InheritedDataContextProperty) == null)
            {
                element.SetBinding(InheritedDataContextProperty, new Binding());
            }
 
            EventHandler currentHandler = (EventHandler)element.GetValue(DataContextChangedHandlerProperty);
            currentHandler += handler;
            element.SetValue(DataContextChangedHandlerProperty, currentHandler);
        }
 
        /// <summary>
        /// Removes the data context changed handler.
        /// </summary>
        /// <param name="element">The element from which the handler has to be removed</param>
        /// <param name="handler">The handler to remove</param>
        public static void RemoveDataContextChangedHandler(this FrameworkElement element, EventHandler handler)
        {
            if (element == null || handler == null)
            {
                return;
            }
 
            EventHandler currentHandler = (EventHandler)element.GetValue(DataContextChangedHandlerProperty);
            currentHandler -= handler;
            if (currentHandler == null)
            {
                element.ClearValue(DataContextChangedHandlerProperty);
                element.ClearValue(InheritedDataContextProperty);
            }
            else
            {
                element.SetValue(DataContextChangedHandlerProperty, currentHandler);
            }
        }
    }
}
