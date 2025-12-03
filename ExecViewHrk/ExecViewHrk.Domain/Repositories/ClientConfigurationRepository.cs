using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using ExecViewHrk.EfClient;
using System.Data.Entity.Validation;
using ExecViewHrk.EfAdmin;
namespace ExecViewHrk.Domain.Repositories
{
    public class ClientConfigurationRepository :RepositoryBase,IClientConfigurationRepository
    {
        public List<ClientConfigurationVM> getClientConfigurationList(int employerId)
        {          
            var ConfigurationValue = _contextAdmin.ClientConfigurations.Where(x => x.Employer.EmployerId == employerId)
                .Select(m => m.ClientConfigId);
           var clientConfigurationsList = _contextAdmin.ClientConfigurations.Where(x => x.Employer.EmployerId == employerId)
                    .Select(m => new ClientConfigurationVM
                    {
                        ClientConfigId = m.ClientConfigId,
                        EmployerId = m.EmployerId,
                        ConfigurationName = m.ConfigurationName,
                        ConfigurationValue = m.ConfigurationValue,
                        EmployerName = m.Employer.EmployerName                        
                    }).OrderBy(m => m.ConfigurationName)
                    .AsEnumerable()
                    .Select(m=>new ClientConfigurationVM {
                        ClientConfigId = m.ClientConfigId,
                        EmployerId = m.EmployerId,
                        ConfigurationName = m.ConfigurationName,
                        ConfigurationValue = m.ConfigurationValue,
                        FiscalMonthText = m.ConfigurationName == "Fiscal Month" ? System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(m.ConfigurationValue.Value) : m.ConfigurationValue.Value.ToString()
                    })
                    .ToList();
            return clientConfigurationsList;
        }

        public ClientConfigurationVM getClientConfigurationDetails(int ClientConfigId)
        {
            var clientConfigurationVM = _contextAdmin.ClientConfigurations
                .Select(m => new ClientConfigurationVM
                {
                    ClientConfigId = m.ClientConfigId,
                    EmployerId = m.EmployerId,
                    ConfigurationName = m.ConfigurationName,
                    ConfigurationValue = m.ConfigurationValue
                    //FiscalMonth=m.ConfigurationValue
                }).Where(m => m.ClientConfigId == ClientConfigId).FirstOrDefault();
            return clientConfigurationVM;
        }

        public ClientConfigurationVM clientConfigurationSave(ClientConfigurationVM clientConfigurationVM)
        {
            ClientConfiguration ClientConfigurationEFClient = _contextAdmin.ClientConfigurations.Where(m => m.ClientConfigId == clientConfigurationVM.ClientConfigId).SingleOrDefault();
            if (ClientConfigurationEFClient != null)
            {
                ClientConfigurationEFClient.EmployerId = clientConfigurationVM.EmployerId;
                // ClientConfigurationEFClient.ConfigurationValue = clientConfigurationVM.FiscalMonth != 0 ? clientConfigurationVM.FiscalMonth : clientConfigurationVM.ConfigurationValue;
                ClientConfigurationEFClient.ConfigurationValue = clientConfigurationVM.ConfigurationValue;
                _contextAdmin.SaveChanges();
            }            
            return clientConfigurationVM;
        }
    }
}

  