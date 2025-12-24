using ExecViewHrk.Domain.Models;
using System.Collections.Generic;

namespace ExecViewHrk.Domain.Interface
{
    public interface IGeofenceRepository
    {
        bool SaveGeofence(string GeofenceName,string Coordinate,string latitude,
            string longitude, string Radius, string CreatedBy);


        List<GeofenceDM> GetGeofenceDetails();
    }
}
