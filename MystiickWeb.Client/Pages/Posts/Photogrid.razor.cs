using Microsoft.AspNetCore.Components;

using MystiickWeb.Client.Shared;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Services;

namespace MystiickWeb.Client.Pages.Posts
{
    public partial class Photogrid
    {
        private ImageCategory[]? categories;
        private ImageCategory[]? subcategories;
        private ImageResult[]? images;
        private ImageResult? previewImage;
        private Paginator imagePager = new();

        [Parameter] public string? ImageGuid { get; set; }
        [Parameter] public string? Category { get; set; }
        [Parameter] public string? Subcategory { get; set; }
        [Parameter] public string? Tag { get; set; }

        [Inject] private CacheService _cache { get; set; } = new();
        [Inject] private NavigationManager _navigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            categories = await GetFromApiAsync<ImageCategory[]>($"{ControllerConstants.Images}/categories");
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            // Handle URL args
            if (!string.IsNullOrWhiteSpace(ImageGuid))
            {
                previewImage = await GetFromApiAsync<ImageResult>($"{ControllerConstants.Images}/{ImageGuid}");
            }
            if (!string.IsNullOrWhiteSpace(Category))
            {
                SetImages(await GetFromApiAsync<ImageResult[]>($"{ControllerConstants.Images}/categories/{Category}"), true);
            }
            if (!string.IsNullOrWhiteSpace(Subcategory))
            {
                SetImages(await GetFromApiAsync<ImageResult[]>($"{ControllerConstants.Images}/subcategories/{Subcategory}"), false);
            }
            if (!string.IsNullOrWhiteSpace(Tag))
            {
                SetImages(await GetFromApiAsync<ImageResult[]>($"{ControllerConstants.Images}/tags/{Tag}"), true);
            }
        }

        public override async Task<T> GetFromApiAsync<T>(string path)
        {
            // Set to null first to clear out existing list, preventing images from popping in over the old ones, one at a time
            images = null;

            return await base.GetFromApiAsync<T>(path);
        }

        protected void LoadThumbnailsByCategory(string category)
        {
            _navigationManager.NavigateTo($"photogrid/category/{category}");
        }

        protected void LoadThumbnailsBySubcategory(string subcategory)
        {
            _navigationManager.NavigateTo($"photogrid/subcategory/{subcategory}");
        }

        protected void LoadThumbnailsByTag(string tag)
        {
            _navigationManager.NavigateTo($"photogrid/tag/{tag}");
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
            _navigationManager.NavigateTo($"photogrid/{image.GUID}");
        }
    }
}
