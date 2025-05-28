using System;
using Azure;
using Azure.Data.Tables;
using FastRide.Server.Services.Attributes;
using FastRide.Server.Services.Constants;
using FastRide.Server.Services.Enums;

namespace FastRide.Server.Services.Entities;

[TableName(TableNames.Users)]
public class UserEntity : ITableEntity
{
    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string PartitionKey { get; set; }

    /// <summary>
    /// Gets or sets the name identifier.
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
    
    public double Rating { get; set; }
    
    public string PictureUrl { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public string UserName { get; set; }
}