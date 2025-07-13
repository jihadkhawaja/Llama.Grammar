using MudBlazor;

namespace Llama.Grammar.WebApp.Services;

public class ThemeService
{
    public bool IsDarkMode { get; private set; } = false;
    public event Action? OnThemeChanged;

    public MudTheme Theme => new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#594ae2",
            Secondary = "#ff4081",
            Tertiary = "#1bc5bd",
            Background = "#ffffff",
            BackgroundGray = "#f5f5f5",
            Surface = "#ffffff",
            DrawerBackground = "#ffffff",
            DrawerText = "rgba(0,0,0, 0.87)",
            AppbarBackground = "#594ae2",
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
            Primary = "#776be7",
            Secondary = "#ff4081",
            Tertiary = "#1bc5bd",
            Background = "#121212",
            BackgroundGray = "#1e1e1e",
            Surface = "#1e1e1e",
            DrawerBackground = "#1e1e1e",
            DrawerText = "#ffffff",
            AppbarBackground = "#1e1e1e",
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

    public void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        OnThemeChanged?.Invoke();
    }
}