using Sharpnado.Shades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CinemaPage : ContentPage
    {
        private const int RowHeight = 33;
        private const int ColumnWidth = 33;
        private const int Rows = 6;
        private const int Columns = 7;
        public CinemaPage()
        {
            InitializeComponent();
            SitsGrid.Children.Add(BuildSits(Rows, Columns));


            HideButtons(new Point(0, 0), new Point(0, 1), new Point(0, 5), new Point(0, 6));
            HideButtons(new Point(Rows - 1, 0), new Point(Rows - 1, Columns - 1));
            OccupySit(new Point(4, 3), new Point(4, 4), new Point(3, 1), new Point(1,4), new Point(1,5), new Point(2,3), new Point(2,4), new Point(2,5));
            ZoomBackground();
            BindingContext = this;
        }

        private async Task ZoomBackground()
        {
            uint animationLength = 20000;
            while(true)
            {
                await Task.WhenAll( Background.ScaleTo(1.5, animationLength));
                await Task.WhenAll(Background.ScaleTo(1, animationLength));
            }
        }

        private View[,] _buttons = new View[Rows, Columns];        

        private int _selectedSits;
        public int SelectedSits
        {
            get => _selectedSits;
            set
            {
                _selectedSits = value;
                OnPropertyChanged(nameof(SelectedSits));
            }
        }

        private Grid BuildSits(int rows, int columns)
        {
            var rowDefinitionCollection = new RowDefinitionCollection();
            for (var i = 0; i < rows; i++)
                rowDefinitionCollection.Add(new RowDefinition { Height = RowHeight });

            var columnDefinitionCollection = new ColumnDefinitionCollection();
            for (var i = 0; i < columns; i++)
                columnDefinitionCollection.Add(new ColumnDefinition { Width = ColumnWidth });

            var grid = new Grid
            {
                RowDefinitions = rowDefinitionCollection,
                ColumnDefinitions = columnDefinitionCollection,
                HorizontalOptions = LayoutOptions.Center,
                RowSpacing = 20, ColumnSpacing = 10
            };

            for(var i=0; i < rows; i++)
                for(var j=0; j < columns; j++)
                {
                    bool enabled = false;
                    var button = new ImageButton()
                    {
                        BackgroundColor = Color.Transparent,
                        CornerRadius = 8,
                        Opacity = 0.15,
                        Source = "chair"                        
                    };
                    _buttons[i, j] = button;
                    var shade = new Shade
                    {
                        BlurRadius = 10,
                        Opacity = 0,
                        Offset = new Xamarin.Forms.Point(0,0),
                        Color = Color.FromHex("fbc02d")
                    };
                    var shadows = new Shadows
                    {
                        Shades = new[] { shade }
                    };
                    shadows.Content = button;
                    button.Clicked += (_, __) =>
                    {
                        if (!enabled)
                        {
                            enabled = true;                            
                            button.Opacity = 1;
                            button.BorderWidth = 0;
                            shade.BlurRadius = 10;
                            shade.Opacity = 0.7;
                            SelectedSits++;
                            button.SetValue(IconTintColorEffect.TintColorProperty, Color.FromHex("fbc02d"));
                        }
                        else
                        {
                            button.SetValue(IconTintColorEffect.TintColorProperty, Color.White);
                            enabled = false;                            
                            button.BorderWidth = 1;
                            button.Opacity = 0.15;
                            shade.BlurRadius = 0;
                            shade.Opacity = 0;
                            SelectedSits--;
                        }
                    };
                    button.SetValue(IconTintColorEffect.TintColorProperty, Color.White);
                    var child = shadows;
                    child.SetValue(Grid.RowProperty, i);
                    child.SetValue(Grid.ColumnProperty, j);

                    grid.Children.Add(child);
                }

            return grid;
        }


        private void HideButtons(params Point[] points)
        {
            foreach (var point in points)
                _buttons[point.X, point.Y].IsVisible = false;
        }

        private void HideColumn(int column)
        {
            for (var i = 0; i < Rows; i++)
                _buttons[i, column].IsVisible = false;
        }


        private void HideRow(int row)
        {
            for (var i = 0; i < Columns; i++)
                _buttons[row,i].IsVisible = false;
        }

        private void OccupySit(params Point[] points)
        {
            foreach (var point in points)
            {
                var button = _buttons[point.X, point.Y];
                button.SetValue(IconTintColorEffect.TintColorProperty, Color.White);                
                button.Opacity = 1;
            }

        }

        private class Point
        {
            public Point(int x, int y)
            {
                (X, Y) = (x, y);
            }
            public int X { get; set; }
            public int Y { get; set; }
        }        
    }
}