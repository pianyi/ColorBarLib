using Prism.Mvvm;

namespace Example.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Color Bar Library Example Window";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
