using System;
using System.Collections;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using System.Linq;

namespace Goche.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class MasonryGrid : Grid
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(MasonryGrid), null, propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty ImageUrlProperty =
            BindableProperty.Create(nameof(ImageUrl), typeof(string), typeof(MasonryGrid), default(string), propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(MasonryGrid), default(string), propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty FirstItemOnRightProperty =
            BindableProperty.Create(nameof(FirstItemOnRight), typeof(bool), typeof(MasonryGrid), false, propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(MasonryGrid), null);

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string ImageUrl
        {
            get { return (string)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool FirstItemOnRight
        {
            get { return (bool)GetValue(FirstItemOnRightProperty); }
            set { SetValue(FirstItemOnRightProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MasonryGrid masonryGrid && newValue is IEnumerable items)
            {
                masonryGrid.PopulateMasonryLayout(items, masonryGrid.ImageUrl, masonryGrid.Text, masonryGrid.FirstItemOnRight, masonryGrid.ItemTemplate);
            }
        }

        //[Obsolete("This Method no longer used",true)]
        //private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    if (bindable is MasonryGrid masonryGrid && newValue is IEnumerable items)
        //    {
        //        masonryGrid.PopulateMasonryLayout(items, masonryGrid.ImageUrl, masonryGrid.Text, masonryGrid.FirstItemOnRight);
        //    }
        //}

        private void PopulateMasonryLayout(IEnumerable items, string imageUrlPropertyName, string textPropertyName, bool firstItemOnRight, DataTemplate itemTemplate)
        {
            try
            {
                // Clear existing children
                Children.Clear();

                VerticalOptions = LayoutOptions.FillAndExpand;
                HorizontalOptions = LayoutOptions.FillAndExpand;
                ColumnSpacing = 5.0;
                RowSpacing = 7.0;
                Padding = new Thickness(15);

                int stackCount = 3; // Number of stack layouts
                int currentIndex = firstItemOnRight ? stackCount - 1 : 0; // Track the current stack layout index
                
                // Create stack layouts
                var stackLayouts = new StackLayout[stackCount];
                for (int i = 0; i < stackCount; i++)
                {
                    stackLayouts[i] = new StackLayout();
                    Children.Add(stackLayouts[i]);
                }

                var itemHeights = new double[] { 220, 180, 200, 170, 190, 210, 160, 230, 250, 190 };

                foreach (var item in items)
                {
                    var view = (View)itemTemplate.CreateContent();

                    view.BindingContext = item;

                    view.HeightRequest = itemHeights[new Random().Next(0, itemHeights.Length)];

                    stackLayouts[currentIndex].Children.Add(view);

                    if (firstItemOnRight)
                        currentIndex = (currentIndex - 1 + stackCount) % stackCount;
                    else
                        currentIndex = (currentIndex + 1) % stackCount;

                    if (currentIndex == 0)
                    {
                        // Shuffle the itemHeights array to distribute item heights among stack layouts
                        itemHeights = ShuffleArray(itemHeights, view.HeightRequest);
                    }
                }

                for (int i = 0; i < stackCount; i++)
                {
                    var columnDefinition = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                    ColumnDefinitions.Add(columnDefinition);
                    Grid.SetColumn(stackLayouts[i], i);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Obsolete("This Method No Longer User, Please Pass ItemTemplate Instead",true)]
        private void PopulateMasonryLayout(IEnumerable items, string imageUrlPropertyName, string textPropertyName, bool firstItemOnRight)
        {
            try
            {
                // Clear existing children
                Children.Clear();

                VerticalOptions = LayoutOptions.FillAndExpand;
                HorizontalOptions = LayoutOptions.FillAndExpand;
                ColumnSpacing = 5.0;
                RowSpacing = 7.0;
                Padding = new Thickness(15);

                int stackCount = 3; // Number of stack layouts
                int currentIndex = firstItemOnRight ? stackCount - 1 : 0; // Track the current stack layout index

                // Create stack layouts
                var stackLayouts = new StackLayout[stackCount];
                for (int i = 0; i < stackCount; i++)
                {
                    stackLayouts[i] = new StackLayout();
                    Children.Add(stackLayouts[i]);
                }

                var itemHeights = new double[] { 220, 180, 200, 170, 190, 210, 160, 230, 250, 190 };

                foreach (var item in items)
                {
                    var imageValue = GetPropertyValue(item, imageUrlPropertyName);
                    var titleValue = GetPropertyValue(item, textPropertyName);

                    var heightRequest = itemHeights[(currentIndex + 1) % itemHeights.Length];

                    var view = new PancakeView
                    {
                        BackgroundColor = Color.Transparent,
                        CornerRadius = new CornerRadius(20),
                        Padding = new Thickness(0),
                        HeightRequest = heightRequest,
                        Content = new Grid
                        {
                            Children =
                            {
                                new Image
                                {
                                    Source = imageValue?.ToString(),
                                    Aspect = Aspect.AspectFill,
                                    HeightRequest = heightRequest
                                },
                                new BoxView
                                {
                                    Color = Color.Black,
                                    Opacity = 0.4
                                },
                                new Label
                                {
                                    Text = titleValue?.ToString(),
                                    FontSize = 16,
                                    FontAttributes = FontAttributes.Bold,
                                    TextColor = Color.White,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    VerticalTextAlignment = TextAlignment.Center
                                }
                            }
                        }
                    };

                    stackLayouts[currentIndex].Children.Add(view);

                    if (firstItemOnRight)
                        currentIndex = (currentIndex - 1 + stackCount) % stackCount;
                    else
                        currentIndex = (currentIndex + 1) % stackCount;

                    if (currentIndex == 0)
                    {
                        // Shuffle the itemHeights array to distribute item heights among stack layouts
                        itemHeights = ShuffleArray(itemHeights, heightRequest);
                    }
                }

                for (int i = 0; i < stackCount; i++)
                {
                    var columnDefinition = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                    ColumnDefinitions.Add(columnDefinition);
                    Grid.SetColumn(stackLayouts[i], i);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private object GetPropertyValue(object item, string propertyName)
        {
            if (item == null || string.IsNullOrEmpty(propertyName))
                return null;

            var propertyInfo = item.GetType().GetProperty(propertyName);
            return propertyInfo?.GetValue(item);
        }

        double[] ShuffleArray(double[] array, double lastItemHeightRequest)
        {
            Random random = new Random();
            int n = array.Length;
            while (n > 1)
            {
                int k = random.Next(n--);
                var temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
            array[array.Length - 1] = lastItemHeightRequest; // Set the last item's height request
            return array;
        }
    }
}
