using System;
using Azure;
using Azure.Data.Tables;
using FastRide.Server.Services.Attributes;
using FastRide.Server.Services.Constants;

namespace FastRide.Server.Services.Entities;

[TableName(TableNames.OnlineDrivers)]
public class OnlineDriversEntity: ITableEntity
{
    /// <summary>
    /// Gets or sets group name.
    /// </summary>
    public string PartitionKey { get; set; }

    /// <summary>
    /// Gets or sets userId.
    /// </summary>
    public string RowKey { get; set; }

    public string Email { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }
}