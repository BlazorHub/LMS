namespace LMS.IdP.Models
{
    public enum MessageType
    {
        Primary,
        Secondary,
        Success,
        Danger,
        Warning,
        Info,
        Light,
        Dark
    }

    public class MessageViewModel
    {
        public MessageType Type { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string ReturnUrl { get; set; }

        public int RedirectSeconds { get; set; } = 3;
    }
}
