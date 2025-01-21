using BlazorApp.Client.Infrastructure.Settings;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace BlazorApp.Client.Shared
{
    public partial class MainLayout : IDisposable
    {
        private bool _isDarkMode = true;
        private MudTheme? _theme = null;

        private bool _rightToLeft = false;
        private async Task RightToLeftToggle(bool value)
        {
            _rightToLeft = value;
            await Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            _theme = BlazorAppTheme.DefaultTheme;
            _isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();
            _rightToLeft = await _clientPreferenceManager.IsRTL();
            _interceptor.RegisterEvent();
        }

        private async Task DarkMode()
        {
            _isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();

            await Task.CompletedTask;
        }

        public string DarkLightModeButtonIcon => _isDarkMode switch
        {
            true => Icons.Material.Rounded.AutoMode,
            false => Icons.Material.Outlined.DarkMode,
        };

        public void Dispose()
        {
            _interceptor.DisposeEvent();
        }
    }
}