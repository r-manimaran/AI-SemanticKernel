using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandoffOrchestrationApp;

public sealed class OrderStatusPlugin
{
    [KernelFunction]
    public string CheckOrderStatus(string orderId) => $"Order {orderId} is shipped and will arrive in 2-3 days.";
}

public sealed class OrderRefundPlugin
{
    [KernelFunction]
    public string ProcessRefund(string orderId, string reason) => $"Refund for Order {orderId} has been processed successfully.";
}

public sealed class OrderReturnPlugin
{
    [KernelFunction]
    public string ProcessReturn(string orderId, string reason) => $"Return for the Order {orderId} is processed successfully";
}
