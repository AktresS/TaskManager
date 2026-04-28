using System;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Models;

namespace TaskManager.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<ProjectMessage> ProjectMessages { get; set; }
    public DbSet<WorkItem> WorkItems { get; set; }
    public DbSet<WorkItemAssignee> WorkItemAssignees { get; set; }
    public DbSet<WorkItemMessage> WorkItemMessages { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectMemberConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectMessageConfiguration());
        modelBuilder.ApplyConfiguration(new WorkItemConfiguration());
        modelBuilder.ApplyConfiguration(new WorkItemAssigneeConfiguration());
        modelBuilder.ApplyConfiguration(new WorkItemMessageConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.HasIndex(x => x.Email).IsUnique();
    }
}

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(x => x.ProjectId);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedProjects)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.HasKey(x => x.ProjectMemberId);

        builder.HasOne(x => x.Project)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.ProjectMemberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.ProjectId, a.UserId }).IsUnique();
    }
}

public class ProjectMessageConfiguration : IEntityTypeConfiguration<ProjectMessage>
{
    public void Configure(EntityTypeBuilder<ProjectMessage> builder)
    {
        builder.HasKey(x => x.ProjectMessageId);

        builder.HasOne(x => x.Project)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.ProjectMessages)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.HasKey(x => x.WorkItemId);

        builder.HasOne(x => x.Project)
            .WithMany(x => x.WorkItems)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedWorkItems)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.State)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(x => x.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}

public class WorkItemAssigneeConfiguration : IEntityTypeConfiguration<WorkItemAssignee>
{
    public void Configure(EntityTypeBuilder<WorkItemAssignee> builder)
    {
        builder.HasKey(x => x.WorkItemAssigneeId);

        builder.HasOne(x => x.WorkItem)
            .WithMany(x => x.Assignees)
            .HasForeignKey(x => x.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.AssignedWorkItems)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.WorkItemId, a.UserId }).IsUnique();
    }
}

public class WorkItemMessageConfiguration : IEntityTypeConfiguration<WorkItemMessage>
{
    public void Configure(EntityTypeBuilder<WorkItemMessage> builder)
    {
        builder.HasKey(x => x.WorkItemMessageId);

        builder.HasOne(x => x.WorkItem)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.WorkItemMessages)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.RefreshTokenId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}