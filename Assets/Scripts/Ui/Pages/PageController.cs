using System.Collections.Generic;
using DarkKey.Core.Debugger;
using UnityEngine;

namespace DarkKey.Ui.Pages
{
    public class PageController : MonoBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.UI};

        private static PageController _instance;
        public static PageController Instance
        {
            get
            {
                if (_instance == null) CustomDebugger.LogError("Instance is null", ScriptLogLevel);
                return _instance;
            }
        }

        private Dictionary<PageType, Page> _pages;
        [SerializeField] private Page[] menuPages;

        #region Unity Functions

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _pages = new Dictionary<PageType, Page>();
            }
            else
                Destroy(gameObject);
        }

        private void Start() => RegisterAllPages();

        #endregion

        #region Public Functions

        public void TurnOnPage(PageType onPage)
        {
            if (onPage == PageType.None) return;
            if (!PageExists(onPage))
            {
                CustomDebugger.LogWarning($"You are trying to turn a page on [{onPage}] that hasn't been registered",
                    ScriptLogLevel);
                return;
            }

            var page = GetPage(onPage);
            page.gameObject.SetActive(true);
        }

        public void TurnOffPage(PageType offPage, PageType onPage = PageType.None)
        {
            if (offPage == PageType.None) return;
            if (!PageExists(offPage))
            {
                CustomDebugger.LogWarning($"You are trying to turn a page off [{offPage}] that hasn't been registered",
                    ScriptLogLevel);
                return;
            }

            var page = GetPage(offPage);
            page.gameObject.SetActive(false);

            TurnOnPage(onPage);
        }

        #endregion

        #region Private Functions

        private void RegisterAllPages()
        {
            foreach (var page in menuPages)
            {
                RegisterPage(page.PageType, page);
                CustomDebugger.LogInfo($"{page} has been successfully been registered", ScriptLogLevel);
            }
        }

        private void RegisterPage(PageType pageType, Page page)
        {
            if (PageExists(pageType))
            {
                CustomDebugger.LogWarning(
                    $"You are trying to register a page {pageType} that has already been registered {page}",
                    ScriptLogLevel);
                return;
            }

            _pages.Add(pageType, page);
        }

        private Page GetPage(PageType type)
        {
            if (PageExists(type)) return _pages[type];

            CustomDebugger.LogWarning($"You are trying to get a page [{type}] that hasn't been registered",
                ScriptLogLevel);
            return null;
        }

        private bool PageExists(PageType type) => _pages.ContainsKey(type);

        #endregion
    }
}