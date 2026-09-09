using UnityEngine;
using UnityEngine.UI;

public class ShopCoinsBtn : MonoBehaviour
{
    public Button buyBtn;
    public string productId; // Set this in the Unity Inspector (e.g., "com.yourcompany.100coins")
    public int value;

    void Start()
    {
        buyBtn.onClick.AddListener(() =>
        {
            IAPManager.Instance.BuyCoins(productId, value);
        });
    }
}
