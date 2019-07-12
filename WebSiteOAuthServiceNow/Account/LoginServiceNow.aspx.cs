using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Account_LoginServiceNow : System.Web.UI.Page
{
    /// <summary>
    /// The OAuth 2.0 client object to use to obtain authorization and authorize outgoing HTTP requests.
    /// </summary>
    private static readonly WebServerClient Client;

    /// <summary>
    /// The details about the sample OAuth-enabled WCF service that this sample client calls into.
    /// </summary>
    private static AuthorizationServerDescription authServerDescription = new AuthorizationServerDescription
    {
        TokenEndpoint = new Uri(ConfigurationManager.AppSettings["tokenURL"]),
        AuthorizationEndpoint = new Uri(ConfigurationManager.AppSettings["authorizationURL"]),
    };

    /// <summary>
    /// Initializes static members of the <see cref="SampleWcf2"/> class.
    /// </summary>
    static Account_LoginServiceNow()
    {
        Client = new WebServerClient(authServerDescription, ConfigurationManager.AppSettings["clientID"], ConfigurationManager.AppSettings["clientSecret"]);
    }

    /// <summary>
    /// Gets or sets the authorization details for the logged in user.
    /// </summary>
    /// <value>The authorization details.</value>
    /// <remarks>
    /// Because this is a sample, we simply store the authorization information in memory with the user session.
    /// A real web app should store at least the access and refresh tokens in this object in a database associated with the user.
    /// </remarks>
    private static IAuthorizationState Authorization
    {
        get { return (AuthorizationState)HttpContext.Current.Session["Authorization"]; }
        set { HttpContext.Current.Session["Authorization"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Check to see if we're receiving a end user authorization response.
            var authorization = Client.ProcessUserAuthorization();
            if (authorization != null)
            {
                // We are receiving an authorization response.  Store it and associate it with this user.
                Authorization = authorization;
                Response.Redirect(Request.Path); // get rid of the /?code= parameter
            }
        }

        if (Authorization != null)
        {
            this.authorizationLabel.Text = "Authorization received!";
            this.authorizationTokenLabel.Text = JsonConvert.SerializeObject(Authorization);
            if (Authorization.AccessTokenExpirationUtc.HasValue)
            {
                TimeSpan timeLeft = Authorization.AccessTokenExpirationUtc.Value - DateTime.UtcNow;
                this.authorizationLabel.Text += string.Format(CultureInfo.CurrentCulture, "  (access token expires in {0} minutes)", Math.Round(timeLeft.TotalMinutes, 1));
            }
        }

        this.getIncidents.Enabled = Authorization != null;
    }

    protected void getAuthorizationButton_Click(object sender, EventArgs e)
    {
        Client.RequestUserAuthorization();
    }

    protected void getIncidents_Click(object sender, EventArgs e)
    {
        try
        {
            if (Authorization == null)
            {
                throw new InvalidOperationException("No access token!");
            }

            if (Authorization.AccessTokenExpirationUtc.HasValue)
            {
                if (Client.RefreshAuthorization(Authorization, TimeSpan.FromSeconds(30)))
                {
                    TimeSpan timeLeft = Authorization.AccessTokenExpirationUtc.Value - DateTime.UtcNow;
                    this.authorizationLabel.Text += string.Format(CultureInfo.CurrentCulture, " - just renewed for {0} more minutes)", Math.Round(timeLeft.TotalMinutes, 1));
                }
            }
            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Authorization] ="Bearer " + Authorization.AccessToken;
            this.getIncidentsLabel.Text = webClient.DownloadString("https://"+ ConfigurationManager.AppSettings["instance"] + ".service-now.com/api/now/v2/table/incident?sysparm_limit=1");
        }
        catch (SecurityAccessDeniedException)
        {
            this.getIncidentsLabel.Text = "Access denied!";
        }
        catch (MessageSecurityException)
        {
            this.getIncidentsLabel.Text = "Access denied!";
        }
        catch (Exception error)
        {
            this.getIncidentsLabel.Text = error.Message;
        }
    }

    private void GetCurrentUser()
    {/*
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        request.Headers.Add("Authorization", Authorization.AccessToken);
        request.Method = "Post";*/
    }
}