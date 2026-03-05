namespace TaskManager.Enums;

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
    Member
}

public enum TaskState
{
    New,
    InProgress,
    Completed,
    Cancelled
}

public enum MessageType
{
    User,
    System
}