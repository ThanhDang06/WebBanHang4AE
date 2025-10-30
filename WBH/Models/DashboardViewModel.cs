using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WBH.Models
{
    public class OrderSummaryItem
    {
        public int IDOrder { get; set; }
        public DateTime DateOrder { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderProductItem
    {
        public int IDProduct { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal => Quantity * Price;
    }

    public class MonthlySpend
    {
        public string MonthLabel { get; set; } // e.g. "2025-10"
        public decimal Amount { get; set; }
    }

    public class DashboardViewModel
    {
        // Overview
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public int PendingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public DateTime? LastOrderDate { get; set; }

        // Lists
        public List<OrderSummaryItem> RecentOrders { get; set; } = new List<OrderSummaryItem>();
        public List<OrderProductItem> LatestOrderProducts { get; set; } = new List<OrderProductItem>();

        // Chart
        public List<MonthlySpend> MonthlySpending { get; set; } = new List<MonthlySpend>();
    }
}