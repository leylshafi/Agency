using Agency.Areas.Admin.ViewModels;

namespace Agency.Services
{
    public interface IAuthService
    {
        Task Register(RegisterVM registerVM);
    }
}
