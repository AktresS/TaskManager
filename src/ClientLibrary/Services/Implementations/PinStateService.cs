using System;

namespace ClientLibrary.Services.Implementations;

public class PinStateService
{
    public event Action? OnChanged;
    public void NotifyChanged() => OnChanged?.Invoke();
}
