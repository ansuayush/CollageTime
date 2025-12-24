using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.EfClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ExecViewHrk.Domain.Repositories
{
   public class GeofenceRepository : RepositoryBase, IGeofenceRepository
    {
        public bool SaveGeofence(
    string GeofenceName,
    string Coordinate,    
    string latitude,
    string longitude,
    string Radius,   
    string CreatedBy)
        {
            try
            {
                Execute(
                    "sp_SaveGeofence",
                    new
                    {
                        GeofenceName = GeofenceName,
                        Coordinate = Coordinate,                      
                        Latitude = latitude,
                        Longitude = longitude,
                        Radius = Radius,
                        CreatedBy = CreatedBy
                    }
                    
                );                
                return true;
            }
            catch (Exception ex)
            {
               
                throw ex; // or return false if you prefer
            }
        }

        public List<GeofenceDM> GetGeofenceDetails()
        {
            return Query<GeofenceDM>("usp_GetGeofenceDetails").ToList();
        }



    }  

}
