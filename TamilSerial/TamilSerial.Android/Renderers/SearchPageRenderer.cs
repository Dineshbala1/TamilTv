using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.AppCompat.Widget;
using TamilSerial.Droid.Renderers;
using TamilSerial.Presentation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SearchPage), typeof(SearchPageRenderer))]
namespace TamilSerial.Droid.Renderers
{
    public class SearchPageRenderer : PageRenderer
    {
        public SearchPageRenderer(Context context) : base(context)
        {

        }

        //Add the Searchbar once Xamarin.Forms creates the Page
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement is SearchPage && e.NewElement is Page page)
            {
                page.Appearing += PageOnAppearing;
            }
        }

        private void PageOnAppearing(object sender, EventArgs e)
        {
            AddSearchToToolbar((sender as Page).Title);
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (Element is SearchPage && Element is Page page && page.Parent is NavigationPage navigationPage)
            {
                //Workaround to re-add the SearchView when navigating back to an ISearchPage, because Xamarin.Forms automatically removes it
                navigationPage.Popped += HandleNavigationPagePopped;
                navigationPage.PoppedToRoot += HandleNavigationPagePopped;

                NavigationPage.SetHasBackButton(navigationPage.CurrentPage, false);
            }
        }

        //Adding the SearchBar in OnSizeChanged ensures the SearchBar is re-added after the device is rotated, because Xamarin.Forms automatically removes it
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            if (Element is SearchPage && Element is Page page && page.Parent.Parent is NavigationPage navigationPage)
            {
                AddSearchToToolbar(page.Title);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (GetToolbar() is Toolbar toolBar)
                toolBar.Menu?.RemoveItem(Resource.Menu.mainmenu);

            base.Dispose(disposing);
        }

        static IEnumerable<Toolbar> GetToolbars(ViewGroup viewGroup)
        {
            for (int i = 0; i < viewGroup.ChildCount; i++)
            {
                if (viewGroup.GetChildAt(i) is Toolbar toolbar)
                {
                    yield return toolbar;
                }
                else if (viewGroup.GetChildAt(i) is ViewGroup childViewGroup)
                {
                    foreach (var childToolbar in GetToolbars(childViewGroup))
                        yield return childToolbar;
                }
            }
        }

        Toolbar? GetToolbar()
        {
            if (Xamarin.Essentials.Platform.CurrentActivity.Window?.DecorView.RootView is ViewGroup viewGroup)
            {
                var toolbars = GetToolbars(viewGroup);

                //Return top-most Toolbar
                return toolbars.LastOrDefault();
            }

            return null;
        }

        //Workaround to re-add the SearchView when navigating back to an ISearchPage, because Xamarin.Forms automatically removes it
        void HandleNavigationPagePopped(object sender, NavigationEventArgs e)
        {
            if (sender is NavigationPage navigationPage &&
                navigationPage.CurrentPage is SearchPage searchableContentPage)
            {
                AddSearchToToolbar(searchableContentPage.Title);
            }
        }

        void AddSearchToToolbar(string pageTitle)
        {
            if (GetToolbar() is Toolbar toolBar
                && toolBar.Menu?.FindItem(Resource.Id.ActionSearch)?.ActionView?.JavaCast<SearchView>()?.GetType() != typeof(SearchView))
            {
                toolBar.Title = pageTitle;
                toolBar.InflateMenu(Resource.Menu.mainmenu);

                if (toolBar.Menu?.FindItem(Resource.Id.ActionSearch)?.ActionView?.JavaCast<SearchView>() is SearchView
                    searchView)
                {
                    searchView.QueryTextChange += HandleQueryTextChange;
                    searchView.ImeOptions = (int) ImeAction.Search;
                    searchView.InputType = (int) InputTypes.TextVariationFilter;
                    searchView.MaxWidth =
                        int.MaxValue; //Set to full width - http://stackoverflow.com/questions/31456102/searchview-doesnt-expand-full-width

                    searchView.SetIconifiedByDefault(false);
                    searchView.QueryTextSubmit += SearchViewOnQueryTextSubmit;
                    searchView.QueryHint = (Element as SearchPage)?.SearchPlaceHolderText;
                    toolBar.Menu?.FindItem(Resource.Id.ActionSearch)?.ExpandActionView();
                }
            }
        }

        private void SearchViewOnQueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            if (Element is SearchPage searchPage)
                searchPage.SearchCommand?.Execute(e.NewText);
        }

        void HandleQueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            if (Element is SearchPage searchPage)
                searchPage.SearchText = e.NewText;
        }
    }
}