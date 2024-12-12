using Xamarin.Forms;
using System.Collections.Generic;
using QuickReserve.Models;

namespace QuickReserve.Views
{
    public partial class OrderPage : ContentPage
    {
        public List<Food> OrderItems { get; set; }

        public OrderPage(List<Food> orderItems)
        {
            InitializeComponent();
            OrderItems = orderItems;
            BindingContext = this;
        }
    }
}
