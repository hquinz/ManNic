# ManNic
Tool to configure NIC IP – Addresses 

For changing IP addresses admin rights are needed. To do so open Visual Studio as Admin and change app.manifest Lines 19 and 20 as shown:
        requestedExecutionLevel level="requireAdministrator" uiAccess="false" />
        <!--requestedExecutionLevel level="asInvoker" uiAccess="false" /-->

IP changing already works
Still working on saving IP Configs in XML file an showing data in tree view
Open Issues 
App crashes if IP NIC is configured with an already existing IP Address

