using WebApi.Models;

namespace WebApi
{
    public static class TicketsService
    {
        public static List<CustomerSupportTicket> GetCustomerSupportTickets()
        {
            var tickets = new List<CustomerSupportTicket>
            {
                new CustomerSupportTicket
                {
                    TicketId = "1",
                    CustomerName = "John Doe",
                    CustomerEmail = "johndoe@email.com",
                    IssueDescription = "Unable to access my account after password reset.",
                    Status = "Resolved",
                    Priority = "High",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    SatisfactionRating = 5,
                    ResolvedAt = DateTime.UtcNow.AddDays(-1),
                    ResolutionNotes = "Account was locked and unlocked"
                },
                new CustomerSupportTicket
                {
                    TicketId = "2",
                    CustomerName = "Alice Smith",
                    CustomerEmail = "alice.smith@email.com",
                    IssueDescription = "Received wrong item in my order.",
                    Status = "Resolved",
                    Priority = "Medium",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    SatisfactionRating = 4,
                    ResolvedAt = DateTime.UtcNow.AddDays(-3),
                    ResolutionNotes = "Replacement item shipped and customer notified"
                },
                new CustomerSupportTicket
                {
                    TicketId = "3",
                    CustomerName = "Michael Brown",
                    CustomerEmail = "michael.b@email.com",
                    IssueDescription = "App crashes on launch.",
                    Status = "Open",
                    Priority = "High",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    SatisfactionRating = 0,
                    ResolvedAt = null,
                    ResolutionNotes = null
                },
                new CustomerSupportTicket
                {
                    TicketId = "4",
                    CustomerName = "Emma Wilson",
                    CustomerEmail = "emma.wilson@email.com",
                    IssueDescription = "Subscription was canceled without notice.",
                    Status = "In Progress",
                    Priority = "High",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    SatisfactionRating = 0,
                    ResolvedAt = null,
                    ResolutionNotes = null
                },
                new CustomerSupportTicket
                {
                    TicketId = "5",
                    CustomerName = "David Johnson",
                    CustomerEmail = "d.johnson@email.com",
                    IssueDescription = "Double charged for a single transaction.",
                    Status = "Resolved",
                    Priority = "High",
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    SatisfactionRating = 3,
                    ResolvedAt = DateTime.UtcNow.AddDays(-5),
                    ResolutionNotes = "Refund issued for duplicate charge"
                },
                new CustomerSupportTicket
                {
                    TicketId = "6",
                    CustomerName = "Sophia Lee",
                    CustomerEmail = "sophia.lee@email.com",
                    IssueDescription = "Can't update billing information.",
                    Status = "Open",
                    Priority = "Medium",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    SatisfactionRating = 0,
                    ResolvedAt = null,
                    ResolutionNotes = null
                },
                new CustomerSupportTicket
                {
                    TicketId = "7",
                    CustomerName = "Daniel Martinez",
                    CustomerEmail = "daniel.m@email.com",
                    IssueDescription = "Missing features after recent update.",
                    Status = "In Progress",
                    Priority = "Low",
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    SatisfactionRating = 0,
                    ResolvedAt = null,
                    ResolutionNotes = null
                },
                new CustomerSupportTicket
                {
                    TicketId = "8",
                    CustomerName = "Olivia Garcia",
                    CustomerEmail = "olivia.g@email.com",
                    IssueDescription = "Not receiving email notifications.",
                    Status = "Resolved",
                    Priority = "Low",
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    SatisfactionRating = 4,
                    ResolvedAt = DateTime.UtcNow.AddDays(-4),
                    ResolutionNotes = "Notification settings updated and verified"
                },
                new CustomerSupportTicket
                {
                    TicketId = "9",
                    CustomerName = "James Anderson",
                    CustomerEmail = "james.anderson@email.com",
                    IssueDescription = "Order status not updating.",
                    Status = "Open",
                    Priority = "Medium",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    SatisfactionRating = 0,
                    ResolvedAt = null,
                    ResolutionNotes = null
                },
                new CustomerSupportTicket
                {
                    TicketId = "10",
                    CustomerName = "Chloe Thompson",
                    CustomerEmail = "chloe.t@email.com",
                    IssueDescription = "Unable to reset password.",
                    Status = "Resolved",
                    Priority = "High",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    SatisfactionRating = 5,
                    ResolvedAt = DateTime.UtcNow.AddDays(-2),
                    ResolutionNotes = "Password reset link sent and confirmed"
                }
            };
            return tickets;

        }
    }
}
