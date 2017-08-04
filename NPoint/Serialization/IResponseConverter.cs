using System.Net.Http;
using System.Threading.Tasks;

namespace NPoint.Serialization
{
    public interface IResponseConverter
    {
        Task<TResponse> Convert<TResponse>(HttpResponseMessage response) where TResponse : class;
    }
}