using System;
using Vehicles.Contracts.V1.Requests.Queries;

namespace Vehicles.Services
{
    public interface IUriService
    {
        Uri GetVehicleUri(string Id);

        Uri GetAllVehiclesUri(PaginationQuery pagination = null);
    }
}