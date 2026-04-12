namespace childspace_backend.Utility
{
    public static class CloudinaryHelper
    {
        public static string? ExtractPublicIdFromUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            try
            {
                var uploadIndex = url.LastIndexOf("/upload/");
                if (uploadIndex == -1) return null;

                var pathAfterUpload = url.Substring(uploadIndex + 8);

                if (pathAfterUpload.StartsWith("v") && pathAfterUpload.Contains("/"))
                {
                    pathAfterUpload = pathAfterUpload.Substring(pathAfterUpload.IndexOf("/") + 1);
                }

                var lastDotIndex = pathAfterUpload.LastIndexOf(".");
                if (lastDotIndex != -1)
                {
                    pathAfterUpload = pathAfterUpload.Substring(0, lastDotIndex);
                }

                return pathAfterUpload;
            }
            catch
            {
                return null;
            }
        }
    }
}
