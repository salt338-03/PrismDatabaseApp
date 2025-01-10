using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismDatabaseApp.Services
{
    public interface IThemeService
    {
        void SetTheme(bool isDarkTheme);
    }

    public class ThemeService : IThemeService
    {
        private readonly PaletteHelper _paletteHelper;

        public ThemeService()
        {
            _paletteHelper = new PaletteHelper();
        }

        public void SetTheme(bool isDarkTheme)
        {
            var theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(isDarkTheme ? BaseTheme.Dark : BaseTheme.Light);
            _paletteHelper.SetTheme(theme);
        }
    }

}
