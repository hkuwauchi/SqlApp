using Prism.Mvvm;

namespace SqlApp.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Unity Application";
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public MainWindowViewModel()
        {

        }
    }
}
