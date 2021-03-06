﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using IdentityModel.OidcClient;
using System;
using System.Text;
using Auth0.OidcClient;

namespace XamarinAndroidTestApp
{
    [Activity(Label = "XamarinAndroidTestApp", MainLauncher = true, Icon = "@drawable/icon",
        LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "xamarinandroidtestapp.xamarinandroidtestapp",
        DataHost = "@string/auth0_domain",
        DataPathPrefix = "/android/xamarinandroidtestapp.xamarinandroidtestapp/callback")]
    public class MainActivity : Activity
    {
        private Auth0Client _client;
        private Button _loginButton;
        private TextView _userDetailsTextView;

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            ActivityMediator.Instance.Send(intent.DataString);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            _loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            _userDetailsTextView = FindViewById<TextView>(Resource.Id.UserDetailsTextView);

            _client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = Resources.GetString(Resource.String.auth0_domain),
                ClientId = Resources.GetString(Resource.String.auth0_client_id)
            });

            _loginButton.Click += LoginButtonOnClick;
            _userDetailsTextView.Text = String.Empty;
        }

        private async void LoginButtonOnClick(object sender, EventArgs eventArgs)
        {
            _userDetailsTextView.Text = "";

            // Call the login method
            var loginResult = await _client.LoginAsync();

            var sb = new StringBuilder();
            if (loginResult.IsError)
            {
                sb.AppendLine($"An error occurred during login: {loginResult.Error}");
            }
            else
            {
                sb.AppendLine($"ID Token: {loginResult.IdentityToken}");
                sb.AppendLine($"Access Token: {loginResult.AccessToken}");
                sb.AppendLine($"Refresh Token: {loginResult.RefreshToken}");

                sb.AppendLine();

                sb.AppendLine("-- Claims --");
                foreach (var claim in loginResult.User.Claims)
                {
                    sb.AppendLine($"{claim.Type} = {claim.Value}");
                }
            }

            _userDetailsTextView.Text = sb.ToString();
        }
    }
}