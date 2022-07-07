using Microsoft.AspNetCore.Components;

using MystiickWeb.Client.Shared;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Client.Pages
{
    public partial class Photography
    {
        public ImageCategory[]? categories;
        public ImageCategory[]? subcategories;
        public ImageResult[]? images;
        public bool imagesLoading = false;
        public ImageResult? previewImage;
        public Paginator imagePager = new();

        [Parameter]
        public string? ImageGuid { get; set; }
        [Inject]
        private Services .CacheService _cache { get; set; } = new();
        [Inject]
        private NavigationManager? _navigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            categories = await GetFromApiAsync<ImageCategory[]>("api/image/categories");

            if (!string.IsNullOrEmpty(ImageGuid))
            {
                previewImage = await GetFromApiAsync<ImageResult>($"api/image/{ImageGuid}");
            }
        }

        public override async Task<T> GetFromApiAsync<T>(string path)
        {
            // Set to null first to clear out existing list, preventing images from popping in over the old ones, one at a time
            images = null;
            _cache.Delete("ImagePager");

            return await base.GetFromApiAsync<T>(path);
        }

        protected async Task LoadThumbnails(string category)
        {
            SetImages(await GetFromApiAsync<ImageResult[]>($"api/image/categories/{category}"), true);
        }

        protected async Task LoadThumbnailsBySubcategory(string subcategory)
        {
            SetImages(await GetFromApiAsync<ImageResult[]>($"api/image/subcategories/{subcategory}"), false);
        }

        protected async Task LoadThumbnailsByTag(string tag)
        {
            SetImages(await GetFromApiAsync<ImageResult[]>($"api/image/tags/{tag}"), true);
        }

        protected void SetImages(ImageResult[] input, bool populateSub)
        {
            images = input;

            if (populateSub)
            {
                subcategories = images.GroupBy(x => x.Subcategory)
                                      .Select(x => new ImageCategory() { Name = x.Key, Count = x.Count() })
                                      .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                                      .ToArray();
            }
        }

        protected void ShowPreview(ImageResult image)
        {
            previewImage = image;
            _navigationManager?.NavigateTo($"photography/{image.GUID}");
        }
    }
}
