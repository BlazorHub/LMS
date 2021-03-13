namespace LMS.Shared
{
    public class UserCourse
    {
        public string UserId { get; set; }

        public User User { get; set; }

        public string CourseId { get; set; }

        public Course Course { get; set; }

        public UserRole UserRole { get; set; }
    }
}
