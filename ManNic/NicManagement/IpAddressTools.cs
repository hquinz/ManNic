using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace HQ4P.Tools.ManNic.NicManagement
{
    public static class IpAddressTools
    {

        #region common
        public static bool CheckAddressForZero(string address)
        {

            if (string.IsNullOrEmpty(address)) return true;

            var splitChars = new[] { '.', ':' };
            var adressparts = address.Split(splitChars);
            return adressparts.All(string.IsNullOrEmpty);
        }

        #endregion

        #region ipV4
        public static byte[] GetAddressBytesFromStringIpV4(string address)
        {
            var localAddress = IPAddress.Parse(address);
            return localAddress.GetAddressBytes();
        }

        public static byte[] GetMaskedAddressIpV4(byte[] address, byte[] mask)
        {
            if (address.Length != mask.Length)
            {
                throw new ArgumentException($"Count of Addresselements not equal! (adress: {address.Length}, mask: {mask.Length}");
            }

            var maskedAdress = new byte[address.Length];
            for (var i = 0; i < address.Length; i++)
            {
                maskedAdress[i] = (byte)(address[i] & mask[i]);
            }
            return maskedAdress;
        }

        public static bool CheckProperGatewayIpV4(string ipAddress, string subnetmask, string gateway)
        {
            var subnetparts = GetAddressBytesFromStringIpV4(subnetmask);
            var networkAddressElements = GetMaskedAddressIpV4(GetAddressBytesFromStringIpV4(ipAddress), subnetparts);
            var gatewayNetworkElements = GetMaskedAddressIpV4(GetAddressBytesFromStringIpV4(gateway), subnetparts);

            var networkAddress = new IPAddress(networkAddressElements);
            var gatewayNetwork = new IPAddress(gatewayNetworkElements);

            return networkAddress.Equals(gatewayNetwork);
        }

        #endregion

        #region ipV6
        public static byte[] GetSubnetBytesIpV6(string subnet)
        {
            const int addressLengthInByte = 16;
            var addressparts = new byte[addressLengthInByte];
            if (!int.TryParse(subnet.Trim('/'), out var prefixLength))
            {
                throw new ArgumentException($"IPv6 prefix not convertible: {subnet}");
            }

            var fullbytes = prefixLength / 8;
            var bitsInMixedByte = prefixLength - fullbytes * 8;
            var mixedByte = (byte)(0xFF << (8 - bitsInMixedByte));

            var i = 0;
            for (; i < fullbytes; i++)
            {
                addressparts[i] = 0xFF;
            }

            addressparts[i] = mixedByte;
            i += 1;

            for (; i < addressLengthInByte; i++)
            {
                addressparts[i] = 0x00;
            }

            return addressparts;
        }

        public static bool CheckProperGatewayIpV6(string ipAddress, string subnetmask, string gateway)
        {
            var subnetparts = IpAddressTools.GetSubnetBytesIpV6(subnetmask);

            throw new NotImplementedException("Not done with IPv6 addresses");

            //todo implement v6 addresses properly
            //dosn't work:
            //var networkAddressElements = GetMaskedIpV4Address(GetAddressBytesFromString(ipAddress), subnetparts);

            //return false;
        }


        #endregion

    }
}
