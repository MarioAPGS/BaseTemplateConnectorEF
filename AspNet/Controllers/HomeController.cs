using System.Net;
using Core.Models;
using Core.Models.Plumberlab.DbItem;
using Core.Models.Plumberlab.AppItem;
using Infrastructure.Providers;
using Infrastructure.Repositories.Base;
using Infrastructure.Repositories.Security;
using Logic;
using Logic.Managers;
using Microsoft.AspNetCore.Mvc;

namespace PlumberlabApi.Controllers;

[ApiController]
public class HomeController : ControllerBase
{
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    readonly TemplateManager templateManager;
    readonly IDockerProvider _dockerProvider;
    public HomeController(IRepositoryBase _repositoryBase, IRepositorySecurity repositorySecurity)
    {
        templateManager = new TemplateManager(_repositoryBase, repositorySecurity);
    }

    #region Template

    [HttpGet("templates")]
    public Response<Template> GetTemplates()
    {
        Log.Info(Functions.RequestToString(Request));

        try
        {
            return templateManager.GetTemplates(Request, null);
        }
        catch (Exception ex)
        {
            Log.Error("Controller GetTemplates error");
            Log.Error(ex);

            return new Response<Template>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("template/{id}")]
    public Response<Template> GetTemplate(int id)
    {
        Log.Info(Functions.RequestToString(Request));

        try
        {
            //var resultTemplates = templateManager.GetTemplates(Request, new[] { new Filter("Id", FilterType.Equal, id) });
            var resultTemplates = templateManager.GetTemplates(Request, new[] { Filter.ById(id) });
            if (resultTemplates.Success)
                return new Response<Template>(true, new[] { resultTemplates.Data.FirstOrDefault() }, resultTemplates.Message, HttpStatusCode.OK);
            else
                return resultTemplates;

        }
        catch (Exception ex)
        {
            Log.Error("Controller GetTemplate error");
            Log.Error(ex);

            return new Response<Template>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost("template")]
    public Response<Template> CreateTemplate([FromBody] Template rule)
    {
        Log.Info(Functions.RequestToString(Request));
        try
        {
            return templateManager.CreateTemplate(Request, rule);
        }
        catch (Exception ex)
        {
            Log.Error("Controller CreateTemplate error");
            Log.Error(ex);

            return new Response<Template>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    [HttpPut("template")]
    public Response<Template> UpdateTemplate([FromBody] Template rule)
    {
        Log.Info(Functions.RequestToString(Request));
        try
        {
            return templateManager.UpdateTemplate(Request, rule);
        }
        catch (Exception ex)
        {
            Log.Error("Controller CreateTemplate error");
            Log.Error(ex);

            return new Response<Template>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    [HttpDelete("template")]
    public Response DeleteTemplate(int id)
    {
        Log.Info(Functions.RequestToString(Request));
        try
        {
            return templateManager.DeleteTemplate(Request, id);
        }
        catch (Exception ex)
        {
            Log.Error("Controller DeleteTemplate error");
            Log.Error(ex);

            return new Response<Template>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
        }
    }
    #endregion

}
