using System.Collections.Generic;
using System.Windows.Documents;

namespace HQ4P.Tools.ManNic.NicManagement.API
{
    internal interface INetworkSettings
    {
        bool DhcpEnabled { get; set; }

        List<string> IpAddresses { get;}
        List<string> IpSubNetMask { get;}
        List<string> DefaultIpGateways { get; }

        void SetIpAdresses(string[] addresses);
        void SetIpSubNetMasks(string[] subNetMasks);
        void SetDefaultIpGateways(string[] gatewayAddresses);
        void SetIpAddress(string address, int index);
        void SetIpSubNetMask(string mask, int index);
        void SetDefaultIpGateway(string gatwayAdress, int index);

        bool DefaultIpGatewayOk(int index);



    }
}