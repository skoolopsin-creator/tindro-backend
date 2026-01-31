using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Analytics.Dtos;

public class DashboardSummaryDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers24h { get; set; }
    public int NewUsers24h { get; set; }
    public int PendingReports { get; set; }
    public decimal Revenue24h { get; set; }
}

