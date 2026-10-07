namespace ASP_Lesson_14.Services
{
    public interface IEmailSenderCart
    {
        Task SendAsync(string from, string to, string subject, string body);
    }
}
