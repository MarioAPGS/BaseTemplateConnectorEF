using Core.Models;
using Core.Models.Security.DbItems;

namespace Infrastructure.Repositories.Security
{
    public interface IRepositorySecurity
    {
        public Response<Log> LogInfo(Log log);
        public Response<Log> LogWarn(Log log);
        public Response<Log> LogError(Log log);
        public Response<AuthToken> LogIn(string email, string password);
        public Response ValidateToken(string token, string table, string method);
        public Response ValidateLogin(string token, bool meesageOverdue);
        public Response<Tool> GetTools(string token);
        public Response<Tool> CreateTool(string token, string toolJson);
        public Response<Tool> DeleteTool(string token, int id);
    }
}
