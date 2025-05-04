namespace ReadersClubDashboard.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }             // Review ID
        public string Comment { get; set; }     // نص التقييم
        public int Rating { get; set; }         // عدد النجوم (من 1 لـ 5)
        public string StoryTitle { get; set; }  // اسم الرواية
        public string UserName { get; set; }    // اسم المستخدم اللي كتب التقييم
    }
}
