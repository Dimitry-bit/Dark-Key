using System.Collections.Generic;
using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using UnityEngine;

namespace DarkKey.Ui.Pages
{
    public class PageController : MonoBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.UI};

        private Dictionary<PageType, Page> _pages;
        [SerializeField] private Page[] menuPages;

        #region Unity Functions

        private void Start()
        {
            _pages = new Dictionary<PageType, Page>();
            RegisterAllPages();
        }

        #endregion

        #region Public Functions

        public void TurnOnPage(PageType onPage)
        {
            if (onPage == PageType.None) return;
            if (!PageExists(onPage))
            {
                ServiceLocator.Instance.cutomeDebugger.LogWarning(
                    $"You are trying to turn a page on [{onPage}] that hasn't been registered",
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
                ServiceLocator.Instance.cutomeDebugger.LogWarning(
                    $"You are trying to turn a page off [{offPage}] that hasn't been registered",
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
                ServiceLocator.Instance.cutomeDebugger.LogInfo($"{page} has been successfully been registered",
                    ScriptLogLevel);
            }
        }

        private void RegisterPage(PageType pageType, Page page)
        {
            if (PageExists(pageType))
            {
                ServiceLocator.Instance.cutomeDebugger.LogWarning(
                    $"You are trying to register a page {pageType} that has already been registered {page}",
                    ScriptLogLevel);
                return;
            }

            _pages.Add(pageType, page);
        }

        private Page GetPage(PageType type)
        {
            if (PageExists(type)) return _pages[type];

            ServiceLocator.Instance.cutomeDebugger.LogWarning(
                $"You are trying to get a page [{type}] that hasn't been registered",
                ScriptLogLevel);
            return null;
        }

        private bool PageExists(PageType type) => _pages.ContainsKey(type);

        #endregion
    }
}