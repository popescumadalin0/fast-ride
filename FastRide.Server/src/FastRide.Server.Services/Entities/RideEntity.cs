/*using System;
using Azure;
using Azure.Data.Tables;
using FastRide.Server.Services.Attributes;
using FastRide.Server.Services.Constants;
using FastRide.Server.Services.Enums;

namespace FastRide.Server.Services.Entities;

[TableName(TableNames.Rides)]
public class RideEntity : ITableEntity
{
    /// <summary>
    /// Gets or sets the user email.
    /// </summary>
    public string PartitionKey { get; set; }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public string RowKey { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public double DestinationLng { get; set; }

    public double DestinationLat { get; set; }
    
    public double StartPointLat { get; set; }
 
    public double StartPointLng { get; set; }
    
    public double Cost { get; set; }
    
    public RideStatus Status { get; set; }
    
    public string DriverEmail { get; set; }
    
    public string DriverId { get; set; }
    
    public string UserId { get; set; }
}*/