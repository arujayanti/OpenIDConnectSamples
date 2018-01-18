# ASP.NET Core 2.0 Azure AD authentication example

This sample application is built on the 2.0 bits to test authentication via Azure AD.

To run it, you'll need to register an application in Azure AD, and fill out the client id and authority either in
appsettings.json or user secrets. You will also need to add a client secret (in user secrets preferrably).

You must also add permissions for the app to the Microsoft Graph API:

1. Find your app in the Azure AD blade's App Registrations tab in Azure Portal
1. Go to Required permissions
1. Click Add
1. Choose *Microsoft Graph* as the API
1. Select *Sign in and read user profile*, *View users' basic profile*, and *View users' email address* under *Delegated permissions*
1. Click Select and Done



*****
The Azure Active Directory (AAD) Graph API provides programmatic access to AAD through REST API endpoints. Applications can use the Graph API to perform CRUD operations on data. 

* We first bind the configuration section "Authentication" to the Open Id Connect options. Then we setup an event handler for when we get an authorization code from AAD we then exchange it for an access token from MS Graph API. We can use ADAL/MSAL (MS Authentication Library) as token then get cached and it handles token referesh automatically.