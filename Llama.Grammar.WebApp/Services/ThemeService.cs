using MudBlazor;
using Microsoft.JSInterop;

namespace Llama.Grammar.WebApp.Services;

public class ThemeService
{
    public bool IsDarkMode { get; set; } = false;
    public event Action? OnThemeChanged;

    public MudTheme Theme => new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#6c757d",
            Secondary = "#868e96",
            Tertiary = "#adb5bd",
            Background = "#ffffff",
            BackgroundGray = "#f8f9fa",
            Surface = "#ffffff",
            DrawerBackground = "#ffffff",
            DrawerText = "rgba(0,0,0, 0.87)",
            AppbarBackground = "#6c757d",
            AppbarText = "#ffffff",
            TextPrimary = "rgba(0,0,0, 0.87)",
            TextSecondary = "rgba(0,0,0, 0.6)",
            ActionDefault = "rgba(0,0,0, 0.54)",
            ActionDisabled = "rgba(0,0,0, 0.26)",
            ActionDisabledBackground = "rgba(0,0,0, 0.12)",
            Divider = "rgba(0,0,0, 0.12)",
            DividerLight = "rgba(0,0,0, 0.06)",
            TableLines = "rgba(0,0,0, 0.12)",
            LinesDefault = "rgba(0,0,0, 0.12)",
            LinesInputs = "rgba(0,0,0, 0.42)",
            TextDisabled = "rgba(0,0,0, 0.38)"
        },
        PaletteDark = new PaletteDark()
        {
            Primary = "#adb5bd",
            Secondary = "#868e96",
            Tertiary = "#6c757d",
            Background = "#212529",
            BackgroundGray = "#343a40",
            Surface = "#343a40",
            DrawerBackground = "#343a40",
            DrawerText = "#ffffff",
            AppbarBackground = "#343a40",
            AppbarText = "#ffffff",
            TextPrimary = "#ffffff",
            TextSecondary = "rgba(255,255,255, 0.7)",
            ActionDefault = "rgba(255,255,255, 0.54)",
            ActionDisabled = "rgba(255,255,255, 0.26)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Divider = "rgba(255,255,255, 0.12)",
            DividerLight = "rgba(255,255,255, 0.06)",
            TableLines = "rgba(255,255,255, 0.12)",
            LinesDefault = "rgba(255,255,255, 0.12)",
            LinesInputs = "rgba(255,255,255, 0.42)",
            TextDisabled = "rgba(255,255,255, 0.38)"
        }
    };

    public async Task InitializeFromSystemPreference(IJSRuntime jsRuntime)
    {
        try
        {
            var systemPrefersDark = await jsRuntime.InvokeAsync<bool>("eval", "window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches");
            SetDarkMode(systemPrefersDark);
        }
        catch
        {
            // Fallback to light mode if system preference detection fails
            SetDarkMode(false);
        }
    }

    public void SetDarkMode(bool isDarkMode)
    {
        IsDarkMode = isDarkMode;
        OnThemeChanged?.Invoke();
    }


}