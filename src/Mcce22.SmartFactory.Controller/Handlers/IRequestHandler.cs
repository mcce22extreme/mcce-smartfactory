using Mcce22.SmartFactory.Controller.Models;

namespace Mcce22.SmartFactory.Controller.Handlers
{
    public interface IRequestHandler
    {
        Task HandleRequest(RequestModel model);
    }
}
