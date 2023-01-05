using Example.Modules.ColorBarPalettesDialog.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace ColorBarPalettesDialog
{
    public class ColorBarPalettesDialogModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<ViewColorBarPalettes>();
        }
    }
}