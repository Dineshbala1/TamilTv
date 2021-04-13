using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace TamilTv.Presentation.Behaviors
{
    public class EndOfListBehavior : Behavior<Page>
    {
        public static readonly BindableProperty EndOfItemsProperty =
            BindableProperty.CreateAttached(
                "EndOfItems",
                typeof(bool),
                typeof(EndOfListBehavior), false, propertyChanged:PropertyChanged);

        public static bool GetEndOfItems(BindableObject view)
        {
            return (bool)view.GetValue(EndOfItemsProperty);
        }

        public static void SetEndOfItems(BindableObject view, bool value)
        {
            view.SetValue(EndOfItemsProperty, value);
        }

        private static async void PropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (newvalue is bool hasEnded && hasEnded)
            {
                await (bindable as Page).DisplaySnackBarAsync(message: "No more items to display",
                    actionButtonText: "Close",
                    () => Task.CompletedTask, TimeSpan.FromSeconds(60).Seconds);
                SetEndOfItems(bindable, false);
            }
        }
    }
}
