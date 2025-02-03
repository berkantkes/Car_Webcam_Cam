using System.Collections.Generic;
using UnityEngine;

public class BannerManager : MonoBehaviour
{
    [SerializeField] private List<Banner> _banners;

    public void SetBannerSprite(Sprite sprite)
    {
        List<Banner> availableBanners = _banners.FindAll(banner => !banner.IsAttachedSprite);

        if (availableBanners.Count > 0)
        {
            Banner selectedBanner = availableBanners[Random.Range(0, availableBanners.Count)];

            selectedBanner.SetSprites(sprite);
        }
        else
        {
            Debug.Log("All banners already have sprites assigned");
        }
    }
}