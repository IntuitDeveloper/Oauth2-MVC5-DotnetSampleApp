//using IdentityModel.Client;
using Intuit.Ipp.OAuth2PlatformClient;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Net;
using System.Collections.Generic;

namespace MvcCodeFlowClientManual.Controllers
{
    public class HomeController : Controller
    {

        

        DiscoveryClient discoveryClient;
        DiscoveryResponse doc;
        AuthorizeRequest request;
        public static IList<JsonWebKey> keys;
        public static string scope;
        public static string authorizeUrl;


        public async Task<ActionResult> Index()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Session.Clear();
            Session.Abandon();
            Request.GetOwinContext().Authentication.SignOut("Cookies");

            //Intialize DiscoverPolicy
            DiscoveryPolicy dpolicy = new DiscoveryPolicy();
            dpolicy.RequireHttps = true;
            dpolicy.ValidateIssuerName = true;


            //Assign the Sandbox Discovery url for the Apps' Dev clientid and clientsecret that you use
            //Or
            //Assign the Production Discovery url for the Apps' Production clientid and clientsecret that you use

            string discoveryUrl = ConfigurationManager.AppSettings["DiscoveryUrl"];

            if (discoveryUrl != null && AppController.clientid != null && AppController.clientsecret != null)
            {
                discoveryClient = new DiscoveryClient(discoveryUrl);
            }
            else
            {
                Exception ex= new Exception("Discovery Url missing!");
                throw ex;
            }
            doc = await discoveryClient.GetAsync();

            if (doc.StatusCode == HttpStatusCode.OK)
            {
                //Authorize endpoint
                AppController.authorizeUrl = doc.AuthorizeEndpoint;

                //Token endpoint
                AppController.tokenEndpoint = doc.TokenEndpoint;

                //Token Revocation enpoint
                AppController.revocationEndpoint = doc.RevocationEndpoint;

                //UserInfo endpoint
                AppController.userinfoEndpoint = doc.UserInfoEndpoint;

                //Issuer endpoint
                AppController.issuerEndpoint = doc.Issuer;

                //JWKS Keys
                AppController.keys = doc.KeySet.Keys;
            }

            //Get mod and exponent value
            foreach (var key in AppController.keys)
            {
                if (key.N != null)
                {
                    //Mod
                    AppController.mod = key.N;
                }
                if (key.N != null)
                {
                    //Exponent
                    AppController.expo = key.E;
                }
            }



                return View();
        }

   

        public ActionResult MyAction(string submitButton)
        {
            switch (submitButton)
            {
                case "C2QB":
                    // delegate sending to C2QB Action
                    return (C2QB());
                case "GetAppNow":
                    // call another action to GetAppNow
                    return (GetAppNow());
                case "SIWI":
                    // call another action to SIWI
                    return (SIWI());
                default:
                    // If they've submitted the form without a submitButton, 
                    // just return the view again.
                    return (View());
            }
        }

        private ActionResult C2QB()
        {
            scope = OidcScopes.Accounting.GetStringValue() + " " + OidcScopes.Payment.GetStringValue();
            authorizeUrl = GetAuthorizeUrl(scope);
            // perform the redirect here.
            return Redirect(authorizeUrl);
        }

        private ActionResult GetAppNow()
        {
            scope = OidcScopes.Accounting.GetStringValue() + " " + OidcScopes.Payment.GetStringValue() + " " + OidcScopes.OpenId.GetStringValue() + " " + OidcScopes.Address.GetStringValue()
                 + " " + OidcScopes.Email.GetStringValue() + " " + OidcScopes.Phone.GetStringValue()
                 + " " + OidcScopes.Profile.GetStringValue();
            authorizeUrl = GetAuthorizeUrl(scope);
            // perform the redirect here.
            return Redirect(authorizeUrl);
        }

        private ActionResult SIWI()
        {
            scope = OidcScopes.OpenId.GetStringValue() + " " + OidcScopes.Address.GetStringValue()
                 + " " + OidcScopes.Email.GetStringValue() + " " + OidcScopes.Phone.GetStringValue()
                 + " " + OidcScopes.Profile.GetStringValue();
            authorizeUrl = GetAuthorizeUrl(scope);
            // perform the redirect here.
            return Redirect(authorizeUrl);
        }


        

        private void SetTempState(string state)
        {
            var tempId = new ClaimsIdentity("TempState");
            tempId.AddClaim(new Claim("state", state));
           
            Request.GetOwinContext().Authentication.SignIn(tempId);
        }

        private string GetAuthorizeUrl(string scope)
        {
            var state = Guid.NewGuid().ToString("N");
        
            SetTempState(state);

            //Make Authorization request
            var request = new AuthorizeRequest(AppController.authorizeUrl);

            string url = request.CreateAuthorizeUrl(
               clientId: AppController.clientid,
               responseType: OidcConstants.AuthorizeResponse.Code,
               scope: scope,
               redirectUri: AppController.redirectUrl,
               state: state);

            return url;
        }

        
    }
}