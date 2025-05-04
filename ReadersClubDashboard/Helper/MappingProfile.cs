using AutoMapper;
using ReadersClubCore.Models;
using ReadersClubDashboard.ViewModels;

namespace ReadersClubDashboard.Helper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, CreatedUser>().ReverseMap();
            CreateMap<Story, EditStoryForm>().ReverseMap();
        }
    }
}
