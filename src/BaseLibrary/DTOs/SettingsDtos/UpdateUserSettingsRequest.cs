
namespace BaseLibrary.DTOs.SettingsDtos;

public class UpdateUserSettingsRequest
{
    public string? Name { get; set; }
    public bool NotificationsEnabled { get; set; }
    public bool DeadlineNotificationsEnabled { get; set; }
    public bool MessageNotificationsEnabled { get; set; }
}
