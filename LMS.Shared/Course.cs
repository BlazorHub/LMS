using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LMS.Shared
{
    /// <summary>课程</summary>
    public class Course
    {
        /// <summary>课程Id</summary>
        public string Id { get; set; }

        /// <summary>班级名</summary>
        [Display(Name = "班级名"), DataType(DataType.Text), Required, MaxLength(64)]
        public string ClassName { get; set; }

        /// <summary>课程名</summary>
        [Display(Name = "课程名"), DataType(DataType.Text), Required, MaxLength(64)]
        public string Name { get; set; }

        /// <summary>课程内容</summary>
        [Display(Name = "课程内容"), DataType(DataType.Text), Required, MaxLength(2048)]
        public string Content { get; set; }

        /// <summary>开始时间</summary>
        [Display(Name = "开始时间"), DataType(DataType.DateTime), Required]
        public DateTime? StartDateTime { get; set; }

        /// <summary>结束时间</summary>
        [Display(Name = "结束时间"), DataType(DataType.DateTime), Required]
        public DateTime? EndDateTime { get; set; }

        public ICollection<User> Users { get; set; }

        public List<UserCourse> UserCourses { get; set; }

        //public object ToSafe() => new
        //{
        //    this.Id,
        //    this.ClassName,
        //    this.Name,
        //    this.Content,
        //    this.StartDateTime,
        //    this.EndDateTime
        //};

        //public Course SafeUpdate(Course newCourse)
        //{
        //    this.ClassName = newCourse.ClassName;
        //    this.Name = newCourse.Name;
        //    this.Content = newCourse.Content;
        //    this.StartDateTime = newCourse.StartDateTime;
        //    this.EndDateTime = newCourse.EndDateTime;
        //    return this;
        //}

        //public static async Task<int> CreateAsync(Context context, Course course)
        //{
        //    await context.Courses.AddAsync(course);
        //    return await context.SaveChangesAsync();
        //}

        //public static async Task<bool> InCourseAsync(Context context, int userId, int courseId) =>
        //    await context.UserCourses.AnyAsync(x => x.UserId == userId && x.CourseId == courseId);

        //public static async Task<bool> InCourseAsync(Context context, int userId, int courseId, UserRole userRole) =>
        //    await context.UserCourses.AnyAsync(x => x.UserId == userId && x.CourseId == courseId && x.UserRole == userRole);

        //public static async Task<bool> InSameCourseAsync(Context context, int userId1, int userId2) =>
        //    await context.UserCourses.Where(x => x.UserId == userId1 || x.UserId == userId2)
        //        .GroupBy(x => x.CourseId)
        //        .AnyAsync(x => x.Count() > 1);

        //public static IQueryable<Course> GetAllByUserId(Context context, int userId)
        //{
        //    var courseIdList = context.UserCourses.Where(x => x.UserId == userId).Select(x => x.CourseId);
        //    return context.Courses.Where(x => courseIdList.Contains(x.Id));
        //}

        //public static async Task<int> JoinAsync(Context context, int userId, int courseId, UserRole userRole)
        //{
        //    if (await context.UserCourses.AnyAsync(x => x.UserId == userId && x.CourseId == courseId))
        //        return -1;

        //    await context.UserCourses.AddAsync(new UserCourse { UserId = userId, CourseId = courseId, UserRole = userRole });
        //    return await context.SaveChangesAsync();
        //}

        //public static async Task<int> QuitAsync(Context context, int userId, int courseId)
        //{
        //    var userCourse = await context.UserCourses.SingleOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);
        //    if (userCourse == null)
        //        return -1;

        //    // Ensure that there are more than 1 teacher in the course
        //    if (userCourse.UserRole == UserRole.Teacher &&
        //        await context.UserCourses.CountAsync(x => x.UserRole == UserRole.Teacher) == 1)
        //        return -2;

        //    context.UserCourses.Remove(userCourse);
        //    return await context.SaveChangesAsync();
        //}
    }
}
