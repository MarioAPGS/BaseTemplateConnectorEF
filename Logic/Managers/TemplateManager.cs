
using Core.Models;
using Core.Models.Plumberlab.AppItem;
using Core.Models.Plumberlab.DbItem;
using Infrastructure.Providers;
using Infrastructure.Repositories.Base;
using Infrastructure.Repositories.Security;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;

namespace Logic.Managers
{
    public class PlumberlabManager
    {
        private readonly IRepositoryBase repositoryBase;
        private readonly SecurityManager securityManager;

        private readonly string TABLE_TEMPLATE = "Template";
        public PlumberlabManager(IRepositoryBase _repositoryBase, IRepositorySecurity repositorySecurity)
        {
            repositoryBase = _repositoryBase;
            securityManager = new SecurityManager(repositorySecurity);
        }
        public Response<Template> GetTemplates(HttpRequest request, IEnumerable<Filter> filters = null)
        {
            var validation = securityManager.ValidateToken(request, TABLE_TEMPLATE, request.Method);
            if (validation != null && validation.Success)
            {
                var result = repositoryBase.Get<Template>(validation.Message, filters);

                if (result.Success)
                    securityManager.LogInfo(request, TABLE_TEMPLATE, (int)result.Rows, result.Message);
                else
                    securityManager.LogError(request, TABLE_TEMPLATE, result.Message);

                return result;
            }
            else
            {
                securityManager.LogError(request, TABLE_TEMPLATE, validation.Message);
                return Response.Cast<Template>(validation);
            }
        }
        public Response<Template> UpdateTemplate(HttpRequest request, Template template)
        {
            var validation = securityManager.ValidateToken(request, TABLE_TEMPLATE, request.Method);
            if (validation != null && validation.Success)
            {
                var result = repositoryBase.Update<Template>(template, validation.Message);

                if (result.Success)
                    securityManager.LogInfo(request, TABLE_TEMPLATE, (int)result.Rows, result.Message);
                else
                    securityManager.LogError(request, TABLE_TEMPLATE, result.Message);

                return result;
            }
            else
            {
                securityManager.LogError(request, TABLE_TEMPLATE, validation.Message);
                return Response.Cast<Template>(validation);
            }
        }
        public Response<Template> CreateTemplate(HttpRequest request, Template template)
        {
            var validation = securityManager.ValidateToken(request, TABLE_TEMPLATE, request.Method);
            if (validation != null && validation.Success)
            {
                template.UserId = validation.Message;
                var result = repositoryBase.Add<Template>(template);

                if (result.Success)
                    securityManager.LogInfo(request, TABLE_TEMPLATE, (int)result.Rows, result.Message);
                else
                    securityManager.LogError(request, TABLE_TEMPLATE, result.Message);

                return result;
            }
            else
            {
                securityManager.LogError(request, TABLE_TEMPLATE, validation.Message);
                return Response.Cast<Template>(validation);
            }
        }
        public Response DeleteTemplate(HttpRequest request, int id)
        {
            var validation = securityManager.ValidateToken(request, TABLE_TEMPLATE, request.Method);
            if (validation != null && validation.Success)
            {
                var result = repositoryBase.Remove<Template>(id, validation.Message);
                if (result.Success)
                    securityManager.LogInfo(request, TABLE_TEMPLATE, (int)result.Rows, result.Message);
                else
                    securityManager.LogError(request, TABLE_TEMPLATE, result.Message);
                return result;
            }
            else
            {
                securityManager.LogError(request, TABLE_TEMPLATE, validation.Message);
                return validation;
            }
        }

    }
}