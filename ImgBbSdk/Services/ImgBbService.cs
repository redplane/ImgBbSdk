using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ImgBbSdk.Constants;

namespace ImgBbSdk.Services
{
    public class ImgBbService
    {
        #region Properties

        /// <summary>
        /// Instance for making http request to ImgBb hosting.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Check whether user has been authorized before.
        /// </summary>
        public bool IsAuthorized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize service with injectors.
        /// </summary>
        public ImgBbService()
        {
            var httpClientHandler = new HttpClientHandler();
            var cookieContainer = new CookieContainer();
            httpClientHandler.CookieContainer = cookieContainer;
            _httpClient = new HttpClient(httpClientHandler);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Using imgbb credential to login into system.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="cancellationToken"></param>
        public async Task<HttpResponseMessage> LoginAsync(string username, string password, CancellationToken cancellationToken)
        {
            // Initialize parameters which will be submitted to ImgBb hosting.
            var keyValuePairs = new List<KeyValuePair<string, string>>();
            keyValuePairs.Add(new KeyValuePair<string, string>("login-subject", username));
            keyValuePairs.Add(new KeyValuePair<string, string>("password", password));

            // Initialize http content.
            var httpContent = new FormUrlEncodedContent(keyValuePairs);
            var httpResponseMessage = await _httpClient.PostAsync(new Uri(ImgBbUrlConstant.UrlLogin), httpContent, cancellationToken);

            // Response message is not successful.
            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
                return httpResponseMessage;

            // Mark user as authorized.
            IsAuthorized = true;

            return httpResponseMessage;
        }

        /// <summary>
        /// Upload a file to a server by using specific information.
        /// </summary>
        /// <returns></returns>
        /// <param name="bytes">File binary data</param>
        /// <param name="contentType">Http content type</param>
        /// <param name="fileName">Name of file to upload to server.</param>
        /// <param name="cancellationToken">Token to cancel the task.</param>
        public async Task<HttpResponseMessage> UploadBinaryAsync(byte[] bytes, string contentType, string fileName, CancellationToken cancellationToken)
        {
            // User hasn't been authorized before.
            if (!IsAuthorized)
                throw new Exception($"Please call {nameof(LoginAsync)} before uploading file to service api.");

            #region Parameters validation

            // Binary data is empty.
            if (bytes == null || bytes.Length < 1)
                throw new Exception("File binary data cannot be empty");

            // Content type is empty.
            if (string.IsNullOrWhiteSpace(contentType))
                throw new Exception("File content type cannot be empty");

            // File name is empty.
            if (string.IsNullOrWhiteSpace(fileName))
                throw new Exception("File name cannot be empty");

            #endregion

            #region Multipart/form-data initialization

            // Initialize multipart/form-data content.
            var multipartFormDataContent = new MultipartFormDataContent();

            // File content initialization.
            var byteArrayContent = new ByteArrayContent(bytes);
            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            multipartFormDataContent.Add(byteArrayContent, "source", fileName);
            multipartFormDataContent.Add(new StringContent("upload"), "action");
            multipartFormDataContent.Add(new StringContent("file"), "type");

            return await _httpClient.PostAsync(new Uri(ImgBbUrlConstant.UrlFileUpload), multipartFormDataContent, cancellationToken);
        }

        #endregion

    }

    #endregion
}