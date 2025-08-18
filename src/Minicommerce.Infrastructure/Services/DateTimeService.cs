using System;
using Minicommerce.Application.Common.Interfaces;

namespace Minicommerce.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}