using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyfinJav.Providers.R18Provider
{
    public class R18ImageProvider : IRemoteImageProvider
    {
        private readonly IHttpClient httpClient;

        public string Name => "R18";

        public R18ImageProvider(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            var id = item.GetProviderId("R18");
            if (string.IsNullOrEmpty(id))
            {
                return Task.FromResult<IEnumerable<RemoteImageInfo>>(new RemoteImageInfo[] { });
            }

            // probably should be downloading the full size image, and then cropping the front cover
            var primaryImage = String.Format("https://pics.r18.com/digital/video/{0}/{0}ps.jpg", id);
            var thumbImage = String.Format("https://pics.r18.com/digital/video/{0}/{0}pl.jpg", id);

            return Task.FromResult<IEnumerable<RemoteImageInfo>>(new RemoteImageInfo[]
            {
                new RemoteImageInfo
                {
                    ProviderName = Name,
                    Type = ImageType.Primary,
                    Url = primaryImage
                },
                new RemoteImageInfo
                {
                    ProviderName = Name,
                    Type = ImageType.Thumb,
                    Url = thumbImage
                }
            });
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
            return new[] { ImageType.Primary, ImageType.Thumb };
        }

        public bool Supports(BaseItem item)
        {
            return item is Movie;
        }
    }
}