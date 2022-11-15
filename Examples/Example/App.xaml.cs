using Example.Modules.ModuleName;
using Example.Services;
using Example.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Example
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<CreateImageService, CreateImageService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleNameModule>();
        }

        private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string? title = "Error";
            string? message = "A system error has occurred. Exit the application.";
            MessageBox.Show(message,
                            title,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error,
                            MessageBoxResult.OK,
                            MessageBoxOptions.DefaultDesktopOnly);

            Shutdown();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            //ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(GetViewModelName);
        }

        private Type? GetViewModelName(Type viewType)
        {
            var viewModelName = $"{viewType.FullName?.Replace("Views", "ViewModels")}ViewModel";
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            return Type.GetType($"{viewModelName}, {viewAssemblyName}");
        }
    }
}
