using ReadersClubCore.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReadersClubDashboard.ViewModels
{
    public class StoryVM
    {
        public Story Story { get; set; }
        public double? AverageRating { get; set; }
    }
}
