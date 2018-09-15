using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;

namespace HQ4P.Tools.ManNic.NicManagement
{
    internal class NicCollector
    {
        internal List<NicConfigurator> NetworkAdapter { get; private set; }

        internal NicCollector()
        {
            GetNetworkAdapters();
        }

        internal void GetNetworkAdapters()
        {
            NetworkAdapter = new List<NicConfigurator>();

            var nicNames = getAdapterNames();

            using (var nicManager = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            using (var nicAdapters = nicManager.GetInstances())
            {
                foreach (var nic in nicAdapters)
                {
                    var index = (uint)nic["InterfaceIndex"];
                    var descripton = (string) nic["Description"];
                    var settingId = (string)nic["SettingID"];

                    string nicName;
                    if (!nicNames.ContainsKey(settingId))
                    {
                        if (descripton.StartsWith("WAN") || descripton.StartsWith("Microsoft")) continue;
                        nicName = "DISABLED";
                    }
                    else nicName = nicNames[settingId];

                    if (nicName.Contains("Pseudo")) continue;

                    var nicData = new NicConfigurator(index, nicName, descripton)
                    {
                        MacAddress = (string)nic["MACAddress"]
                       ,IpEnabled = (bool) nic["IPEnabled"]
                       ,DhcpEnabled = (bool)nic["DHCPEnabled"]
                    };
                    
                    nicData.SetIpAdresses((string[])nic["IPAddress"]);
                    nicData.SetIpSubNetMasks((string[])nic["IPSubnet"]);
                    nicData.SetDefaultIpGateways((string[])nic["DefaultIpGateway"]);

                    NetworkAdapter.Add(nicData);
                }
            }
        }

        private Dictionary<string, string> getAdapterNames()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            return nics.ToDictionary(nicData => nicData.Id, nicData => nicData.Name);


        }

    }
}
