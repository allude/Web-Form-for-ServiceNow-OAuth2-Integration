<%@ Page Title="ServiceNow OAuth Example" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="LoginServiceNow.aspx.cs" Inherits="Account_LoginServiceNow" Async="true" %>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>

    <fieldset title="Authorization">
		<p>
			Check off the operations you&#39;d like to authorization this client to make on
			behalf of your account on the resource server.<br />
			Note that an authorization request may not actually result in you being prompted
			to grant authorization if you&#39;ve granted it previously.&nbsp; The authorization
			server remembers what you&#39;ve already approved.&nbsp; But even if you&#39;ve
			requested and received authorization for all three scopes above, you can request
			access tokens for subsets of this set of scopes to limit what you can do below.
		</p>
		<asp:Button ID="getAuthorizationButton" runat="server" Text="Request Authorization"
			OnClick="getAuthorizationButton_Click" />
		<asp:Label ID="authorizationLabel" runat="server" />
		<asp:Label ID="authorizationTokenLabel" runat="server" />
	</fieldset>
	<br />
	<asp:Button ID="getIncidents" runat="server" Text="Get Incidents" OnClick="getIncidents_Click" />
	<asp:Label ID="getIncidentsLabel" runat="server" />
</asp:Content>

