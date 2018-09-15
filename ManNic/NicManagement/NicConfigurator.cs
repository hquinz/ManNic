using System.Linq;
using System.Management;
using HQ4P.Tools.ManNic.NicManagement.API;

namespace HQ4P.Tools.ManNic.NicManagement
{
    internal class NicConfigurator : NetworkSettings, INicConfigurator
    {
        public uint InterfaceIndex { get; private set; }
        public string Caption { get; private set; }
        public string Description { get; set; }

        public string Id => $"({InterfaceIndex}):{Caption} ({Description})";
        public string MacAddress { get; set; }

        public bool IpEnabled { get; set; }

        internal NicConfigurator(uint interfaceIndex, string caption, string description)
        {
            InterfaceIndex = interfaceIndex;
            Caption = caption;
            Description = description;
        }


        public void DoNicConfiguration()
        {
            if (ActivationControll()) return;
            SetIpConfiguration();
        }

        /// <returns>true when disabled (use as brakecondition)</returns>
        private bool ActivationControll()
        {

            using (var networkAdapterManagement = new ManagementClass("Win32_NetworkAdapter"))
            using (var networkAdapters = networkAdapterManagement.GetInstances())
            using (var adapter = networkAdapters.Cast<ManagementObject>().Single(managementObject =>
                (uint)managementObject["InterfaceIndex"] == InterfaceIndex))
            {
                var request = IpEnabled ? "Enable" : "Disable";
                adapter.InvokeMethod(request, null);
            }
            return !IpEnabled;
        }

        private void SetIpConfiguration()
        {
            //note only working for IPv4   
            using (var networkConfigMng = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            using (var networkConfigs = networkConfigMng.GetInstances())
            using (var nicConfig = networkConfigs.Cast<ManagementObject>().Single(managementObject =>
                (uint)managementObject["InterfaceIndex"] == InterfaceIndex))
            {
                if (DhcpEnabled)
                {
                    nicConfig.InvokeMethod("EnableDHCP", null);
                    nicConfig.InvokeMethod("SetDNSServerSearchOrder", null);
                    return;
                }

                using (var newIp = nicConfig.GetMethodParameters("EnableStatic"))
                {

                    newIp["IPAddress"] = new[] {IpAddresses[0]};
                    newIp["SubnetMask"] = new[] {IpSubNetMask[0]};
                    nicConfig.InvokeMethod("EnableStatic", newIp, null);
                }

                using (var newGateway = nicConfig.GetMethodParameters("SetGateways"))
                {
                    newGateway["DefaultIPGateway"] = new[] { DefaultIpGateways[0]};
                    newGateway["GatewayCostMetric"] = new[] { 1 };
                    nicConfig.InvokeMethod("SetGateways", newGateway, null);
                }

            }

        }
    }


}

