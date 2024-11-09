using System;
using Azure;
using Azure.Data.Tables;
using FastRide_Server.Services.Enums;

namespace FastRide_Server.Services.Entities;

public class UserEntity : ITableEntity
{
    /// <summary>
    /// Gets or sets the name identifier.
    /// </summary>
    public string PartitionKey { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string RowKey { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    /// <summary>
    /// Gets or sets the user type.
    /// </summary>
    public UserType UserType { get; set; }
}