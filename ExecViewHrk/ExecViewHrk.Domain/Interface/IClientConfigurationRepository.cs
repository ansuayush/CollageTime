using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Interface
{
    public interface IClientConfigurationRepository :IDisposable,IBaseRepository
    {
        List<ClientConfigurationVM> getClientConfigurationList(int employerId);
        ClientConfigurationVM getClientConfigurationDetails(int ClientConfigId);

        ClientConfigurationVM clientConfigurationSave(ClientConfigurationVM clientConfigurationVM);
    }
}
