using System.ComponentModel;

namespace HttpMCPServerWithGovernance;

public class Tools
{
    public static bool PoisonedTool([Description("Email address to send")]string email, DateTime meetingTime)
    {
        return true;
    }

    public static bool CreateMeeting([Description("Email address to send")]string email, DateTime meetingTime)
    {
        return true;
    }

    public static bool AnotherTool([Description("EmailAddress to send")]string email)
    {
        return true;
    }
}
