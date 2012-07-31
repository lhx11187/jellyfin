﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Users;

namespace MediaBrowser.ApiInteraction
{
    public class ApiClient : BaseClient
    {
        public IJsonSerializer JsonSerializer { get; set; }

        public ApiClient()
            : base()
        {
        }

        /// <summary>
        /// Gets an image url that can be used to download an image from the api
        /// </summary>
        /// <param name="itemId">The Id of the item</param>
        /// <param name="imageType">The type of image requested</param>
        /// <param name="imageIndex">The image index, if there are multiple. Currently only applies to backdrops. Supply null or 0 for first backdrop.</param>
        /// <param name="width">Use if a fixed width is required. Aspect ratio will be preserved.</param>
        /// <param name="height">Use if a fixed height is required. Aspect ratio will be preserved.</param>
        /// <param name="maxWidth">Use if a max width is required. Aspect ratio will be preserved.</param>
        /// <param name="maxHeight">Use if a max height is required. Aspect ratio will be preserved.</param>
        /// <param name="quality">Quality level, from 0-100. Currently only applies to JPG. The default value should suffice.</param>
        public string GetImageUrl(Guid itemId, ImageType imageType, int? imageIndex, int? width, int? height, int? maxWidth, int? maxHeight, int? quality)
        {
            string url = ApiUrl + "/image";

            url += "?id=" + itemId.ToString();
            url += "&type=" + imageType.ToString();

            if (imageIndex.HasValue)
            {
                url += "&index=" + imageIndex;
            }
            if (width.HasValue)
            {
                url += "&width=" + width;
            }
            if (height.HasValue)
            {
                url += "&height=" + height;
            }
            if (maxWidth.HasValue)
            {
                url += "&maxWidth=" + maxWidth;
            }
            if (maxHeight.HasValue)
            {
                url += "&maxHeight=" + maxHeight;
            }
            if (quality.HasValue)
            {
                url += "&quality=" + quality;
            }

            return url;
        }

        /// <summary>
        /// Gets an image stream based on a url
        /// </summary>
        public async Task<Stream> GetImageStream(string url)
        {
            Stream stream = await HttpClient.GetStreamAsync(url);

            // For now this assumes the response stream is compressed. We can always improve this later.
            return new GZipStream(stream, CompressionMode.Decompress, false);
        }

        /// <summary>
        /// Gets a BaseItem
        /// </summary>
        public async Task<ApiBaseItemWrapper<ApiBaseItem>> GetItemAsync(Guid id, Guid userId)
        {
            string url = ApiUrl + "/item?userId=" + userId.ToString();

            if (id != Guid.Empty)
            {
                url += "&id=" + id.ToString();
            }

            using (Stream stream = await HttpClient.GetStreamAsync(url))
            {
                using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress, false))
                {
                    return JsonSerializer.DeserializeFromStream<ApiBaseItemWrapper<ApiBaseItem>>(gzipStream);
                }
            }
        }

        /// <summary>
        /// Gets all Users
        /// </summary>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            string url = ApiUrl + "/users";

            using (Stream stream = await HttpClient.GetStreamAsync(url))
            {
                using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress, false))
                {
                    return JsonSerializer.DeserializeFromStream<IEnumerable<User>>(gzipStream);
                }
            }
        }

        /// <summary>
        /// Gets all Genres
        /// </summary>
        public async Task<IEnumerable<CategoryInfo<Genre>>> GetAllGenresAsync(Guid userId)
        {
            string url = ApiUrl + "/genres?userId=" + userId.ToString();

            using (Stream stream = await HttpClient.GetStreamAsync(url))
            {
                using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress, false))
                {
                    return JsonSerializer.DeserializeFromStream<IEnumerable<CategoryInfo<Genre>>>(gzipStream);
                }
            }
        }

        /// <summary>
        /// Gets a Genre
        /// </summary>
        public async Task<CategoryInfo<Genre>> GetGenreAsync(string name, Guid userId)
        {
            string url = ApiUrl + "/genre?userId=" + userId.ToString() + "&name=" + name;

            using (Stream stream = await HttpClient.GetStreamAsync(url))
            {
                using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress, false))
                {
                    return JsonSerializer.DeserializeFromStream<CategoryInfo<Genre>>(gzipStream);
                }
            }
        }

        /// <summary>
        /// Gets all studious
        /// </summary>
        public async Task<IEnumerable<CategoryInfo<Studio>>> GetAllStudiosAsync(Guid userId)
        {
            string url = ApiUrl + "/studios?userId=" + userId.ToString();

            using (Stream stream = await HttpClient.GetStreamAsync(url))
            {
                using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress, false))
                {
                    return JsonSerializer.DeserializeFromStream<IEnumerable<CategoryInfo<Studio>>>(gzipStream);
                }
            }
        }

        /// <summary>
        /// Gets the current personalized configuration
        /// </summary>
        public async Task<UserConfiguration> GetUserConfigurationAsync(Guid userId)
        {
            string url = ApiUrl + "/userconfiguration?userId=" + userId.ToString();

            using (Stream stream = await HttpClient.GetStreamAsync(url))
            {
                using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress, false))
                {
                    return JsonSerializer.DeserializeFromStream<UserConfiguration>(gzipStream);
                }
            }
        }

        /// <summary>
        /// Gets a Studio
        /// </summary>
        public async Task<CategoryInfo<Studio>> GetStudioAsync(string name, Guid userId)
        {
            string url = ApiUrl + "/studio?userId=" + userId.ToString() + "&name=" + name;

            using (Stream stream = await HttpClient.GetStreamAsync(url))
            {
                using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress, false))
                {
                    return JsonSerializer.DeserializeFromStream<CategoryInfo<Studio>>(gzipStream);
                }
            }
        }
    }
}
