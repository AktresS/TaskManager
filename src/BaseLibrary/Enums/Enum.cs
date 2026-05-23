using System;

namespace BaseLibrary.Enums;

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum MemberRole
{
    Owner,
    Admin,
    Member
}

public enum TaskState
{
    New,
    InProgress,
    Completed,
    OnHold
}

public enum NotificationType
{
    DeadlineReminder,
    NewProjectMessage,
    NewWorkItemMessage
}

public enum MessageType
{
    User,
    System
}