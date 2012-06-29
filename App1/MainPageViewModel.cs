using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;

namespace App1
{
    public class MainPageViewModel
    {
        public MainPageViewModel()
        {
            TestCommand = new DelegateCommand(x => true, x => this.ShowMessage());
        }

        public DelegateCommand TestCommand { get; set; }

        public void ShowMessage()
        {
            MessageDialog dialog = new MessageDialog("Hello");
            dialog.ShowAsync();
        }
    }
}
