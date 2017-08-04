using System;
using System.Threading.Tasks;

namespace NPoint
{
    public interface IApiClient
    {
        Task<TResponse> Call<TResponse>(Action<IEndpoint> endpointSpec) where TResponse : class;
    }
}