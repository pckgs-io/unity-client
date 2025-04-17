using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Pckgs
{
    public class AccessTokenPickerOption : IPickerOption
    {
        public object Data => TokenDetails;
        public string Token { get; private set; }
        public AccessTokenDetails TokenDetails { get; private set; }

        private bool isLoading = true;
        public AccessTokenPickerOption(string token)
        {
            Token = token;
            GetDetails();
        }

        async void GetDetails()
        {
            try
            {
                isLoading = true;
                TokenDetails = await PckgsApi.GetAccessTokenDetails(Token);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                isLoading = false;
            }
        }
        public async Task<Texture2D> GetImage()
        {
            while (isLoading) await Task.Delay(100);
            if (TokenDetails == null) return null;
            return await PckgsApi.GetTextureFile(TokenDetails.OrganizationLogo);
        }
        public async Task<string> GetName()
        {
            while (isLoading) await Task.Delay(100);
            if (TokenDetails == null) return "<Unknown>";
            return TokenDetails.OrganizationName;
        }
    }
}