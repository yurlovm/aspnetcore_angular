using WEBAPP.Models;

namespace WEBAPP.DAL
{
    public interface ICrisesRepository
    {
        Crisis[] GetAllCrises();
        Crisis GetCrisisById(int id);
    }
}