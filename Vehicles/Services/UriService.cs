using System;
using Microsoft.AspNetCore.WebUtilities;
using Vehicles.Contracts.V1;
using Vehicles.Contracts.V1.Requests.Queries;

namespace Vehicles.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;
        
        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }
        
        public Uri GetVehicleUri(string Id)
        {
            return new Uri(_baseUri + ApiRoutes.Vehicles.Get.Replace("{id:int}", Id));
        }

        public Uri GetAllVehiclesUri(PaginationQuery pagination = null)
        {
            var uri = new Uri(_baseUri);

            if (pagination == null)
            {
                return uri;
            }

            var modifiedUri = QueryHelpers.AddQueryString(_baseUri, "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());
            
            return new Uri(modifiedUri);
        }
    }
}