using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using System;

namespace JellyfinJav.Providers.WarashiProvider
{
    public class WarashiPersonImageProvider : IRemoteImageProvider
    {
        private readonly IHttpClient httpClient;
        private readonly static Api.WarashiClient client = new Api.WarashiClient();

        public string Name => "Warashi";

        public WarashiPersonImageProvider(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            var id = item.GetProviderId("Warashi");
            if (string.IsNullOrEmpty(id))
                return new RemoteImageInfo[] { };

            var actress = await client.LoadActress(id);
            if (!actress.HasValue || actress.Value.Cover == null)
                return new RemoteImageInfo[] { };

            return new RemoteImageInfo[]
            {
                new RemoteImageInfo
                {
                    ProviderName = Name,
                    Type = ImageType.Primary,
                    Url = actress.Value.Cover
                }
            };
        }

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return httpClient.GetResponse(new HttpRequestOptions
            {
                Url = url,
                CancellationToken = cancelToken
            });
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public bool Supports(BaseItem item)
        {
            return item is Person;
        }
    }
}