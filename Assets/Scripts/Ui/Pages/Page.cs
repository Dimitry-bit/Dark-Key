using UnityEngine;

namespace DarkKey.Ui.Pages
{
    public abstract class Page : MonoBehaviour
    {
        [SerializeField] private PageType pageType;
        public PageType PageType => pageType;
    }
}