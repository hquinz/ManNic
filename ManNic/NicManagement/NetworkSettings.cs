
using System;
using System.Collections.Generic;
using System.Linq;
using HQ4P.Tools.ManNic.NicManagement.API;

namespace HQ4P.Tools.ManNic.NicManagement
{
    internal class NetworkSettings : INetworkSettings
    {
        public bool DhcpEnabled { get; set; }

        public List<string> IpAddresses { get; private set; }

        public List<string> IpSubNetMask { get; private set; }
        public List<string> DefaultIpGateways { get; private set; }
        public void SetIpAdresses(string[] ipAddresses)
        {
            IpAddresses = ipAddresses?.ToList() ?? new List<string>();
        }
        public void SetIpSubNetMasks(string[] subNetMasks)
        {
            IpSubNetMask = subNetMasks?.ToList() ?? new List<string>();
        }

        public void SetDefaultIpGateways(string[] gatewayAddresses)
        {
            DefaultIpGateways = gatewayAddresses?.ToList() ?? new List<string>();
        }
        public void SetIpAddress(string ipAddress, int index)
        {
            if(HadToAddItem(IpAddresses, index, ipAddress)) return;
            IpAddresses[index] = ipAddress;
        }
        public void SetIpSubNetMask(string mask, int index)
        {
            if (HadToAddItem(IpSubNetMask, index, mask)) return;
            IpSubNetMask[index] = mask;
        }
        public void SetDefaultIpGateway(string gatwayAdress, int index)
        {
            if (HadToAddItem(DefaultIpGateways, index, gatwayAdress)) return;
            DefaultIpGateways[index] = gatwayAdress;
        }

        public bool IpAddressIsZero(int index)
        {
            CheckIndex(IpAddresses, index);
            return IpAddressTools.CheckAddressForZero(IpAddresses[index]);
        }

        public bool DefaultIpGatewayIsZero(int index)
        {
            CheckIndex(DefaultIpGateways, index);
            return IpAddressTools.CheckAddressForZero(DefaultIpGateways[index]);
        }

        public bool DefaultIpGatewayOk(int index)
        {
            if (DefaultIpGateways == null || DefaultIpGateways.Count <=0) return true;
            CheckIndex(DefaultIpGateways, index);
            return IpAddressTools.CheckAddressForZero(DefaultIpGateways[index]) || CheckProperGateway(index);
        }


        private bool HadToAddItem(List<string> list, int index, string value)
        {
            if (index < list.Count) return false;

            while (index > list.Count )
            {
                list.Add("");
            }

            list.Add(value);
            return true;
        }
        private void CheckIndex(List<string> data, int index)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException("Are you kidding? No Arrayindex lower than zero allowed");
            }
            if (index >= data.Count)
            {
                throw new IndexOutOfRangeException($"Only {data.Count} elements; Index {index} not allowed");
            }

        }
        
        private bool CheckProperGateway(int index)
        {

            if (string.IsNullOrEmpty(IpAddresses[index]) ||
                string.IsNullOrEmpty(IpSubNetMask[index]) ||
                string.IsNullOrEmpty(DefaultIpGateways[index])) return true;

            var ipAddress = IpAddresses[index];
            var subNetMask = IpSubNetMask[index];
            var gateway = DefaultIpGateways[index];

            var isIpv6 = subNetMask.Split('.',':').Length <= 1;

            return !isIpv6 ? IpAddressTools.CheckProperGatewayIpV4(ipAddress, subNetMask, gateway)
                           : IpAddressTools.CheckProperGatewayIpV6(ipAddress, subNetMask, gateway);
        }

    }


}

