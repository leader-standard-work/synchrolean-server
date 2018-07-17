using SynchroLean.Core.Models;
using SynchroLean.Controllers.Resources;

namespace SynchroLean.Profile
{
    public class ApplicationProfile: AutoMapper.Profile
    {
        public ApplicationProfile()
        {
            CreateMap<UserTask, UserTaskResource>()
                .ReverseMap();

            CreateMap<UserAccount, UserAccountResource>()
                .ReverseMap();

            CreateMap<Team, TeamResource>()
                .ReverseMap();
        }
    }
}