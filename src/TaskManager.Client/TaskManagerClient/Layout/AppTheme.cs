
using MudBlazor;

namespace TaskManagerClient.Layout;

public static class AppTheme
{
    public static MudTheme Dark => new()
    {
        PaletteDark = new PaletteDark()
        {
            Primary = "#7c6af7",
            PrimaryContrastText = "#ffffff",
            Surface = "#1e1e1e",
            Background = "#141414",
            AppbarBackground = "#1a1a1a",
            DrawerBackground = "#161616",
            TextPrimary = "#eaeaea",
            TextSecondary = "#888888",
            ActionDefault = "#eaeaea",
            LinesDefault = "#333333",
            Divider = "#2a2a2a",
        },
        Typography = new Typography()
        {
            Default = new DefaultTypography()
            {
                FontFamily = new[] { "Segoe UI", "sans-serif" }
            }
        }
    };
}
