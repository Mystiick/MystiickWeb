using Microsoft.AspNetCore.Components;

using MystiickWeb.Wasm.Shared;
using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Services;
using MystiickWeb.Wasm.Managers;

namespace MystiickWeb.Wasm.Pages.Posts
{
    public partial class Photogrid
    {
        private ImageCategory[]? categories;
        private ImageCategory[]? subcategories;
        private ImageResult[]? images;
        private ImageResult? previewImage;
        private Paginator? imagePager = new();

        [Parameter] public string? ImageGuid { get; set; }
        [Parameter] public string? Category { get; set; }
        [Parameter] public string? Subcategory { get; set; }
        [Parameter] public string? Tag { get; set; }

        [Inject] private CacheService _cache { get; set; } = new();

        // REASON: NavigationManager and ImageService is injected, so there is no need to set them to a value
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] private ImageManager _imageManager { get; set; }
#pragma warning restore CS8618

        protected override async Task OnInitializedAsync()
        {
            categories = await CallApi(_imageManager.GetCategories());
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            images = null;

            // Handle URL args
            if (!string.IsNullOrWhiteSpace(ImageGuid))
            {
                previewImage = (await _imageManager.GetImageByID(ImageGuid)).Value;
            }
            if (!string.IsNullOrWhiteSpace(Category))
            {
                SetImages(await CallApi(_imageManager.GetImagesByCategory(Category)), true);
            }
            if (!string.IsNullOrWhiteSpace(Subcategory))
            {
                SetImages(await CallApi(_imageManager.GetImagesBySubategory(Subcategory)), false);
            }
            if (!string.IsNullOrWhiteSpace(Tag))
            {
                SetImages(await CallApi(_imageManager.GetImagesByTag(Tag)), true);
            }
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

        protected void SetImages(ImageResult[]? input, bool populateSub)
        {
            images = input;

            if (populateSub && images != null)
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
