namespace HQ4P.Tools.ManNic.NicManagement.API
{
    internal interface INicData : INetworkSettings
    {
        uint InterfaceIndex { get; }
        string Caption { get; }
        string Description { get; set; }
        string Id { get; }
        string MacAddress { get; set; }
        bool IpEnabled { get; set; }
    }
}