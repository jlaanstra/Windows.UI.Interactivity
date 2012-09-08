using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Windows.UI.Interactivity
{
    public class DataContextChangedDetector
    {
        private InteractivityBase binder;
        private TaskCompletionSource<object> tcs;
        private FrameworkElement associatedObject;

        public DataContextChangedDetector(InteractivityBase binder, FrameworkElement associatedObject)
        {
            this.binder = binder;
            this.associatedObject = associatedObject;
            this.tcs = new TaskCompletionSource<object>();
        }

        public Task WaitForDataContext()
        {
            if (this.associatedObject.DataContext != null && !this.tcs.Task.IsCompleted)
            {
                this.Complete(this.associatedObject.DataContext);
            }
            else
            {
                this.associatedObject.AddDataContextChangedHandler(this.OnDataContextChanged);
            }
            return tcs.Task;
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            this.associatedObject.RemoveDataContextChangedHandler(this.OnDataContextChanged);
            if (this.associatedObject != null && this.associatedObject.DataContext != null)
            {
                if (!this.tcs.Task.IsCompleted)
                {
                    this.Complete(this.associatedObject.DataContext);
                }
            }
        }

        private void Complete(object dataContext)
        {
            binder.SetBinding(FrameworkElement.DataContextProperty,
                        new Binding
                        {
                            Path = new PropertyPath("DataContext"),
                            Source = this.associatedObject
                        }
                    );
            tcs.SetResult(dataContext);
        }
    }
}
