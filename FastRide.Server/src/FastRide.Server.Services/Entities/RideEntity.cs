﻿using System;
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

    public string Destination { get; set; }
    
    public DateTime FinishTime { get; set; }
    
    public double Cost { get; set; }
    
    public string DriverEmail { get; set; }
}