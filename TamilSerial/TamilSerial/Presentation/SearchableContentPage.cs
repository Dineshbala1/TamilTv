using Xamarin.Forms;

namespace TamilSerial.Presentation
{
    public class SearchableContentPage : ContentPage
    {
        public SearchableContentPage()
        {
            var toolbarItem = new ToolbarItem {IconImageSource = ImageSource.FromFile("search_white.png")};
            toolbarItem.SetBinding(MenuItem.CommandProperty, new Binding {Path = "MenuSearchCommand" });
            ToolbarItems.Add(toolbarItem);
        }
    }
}
