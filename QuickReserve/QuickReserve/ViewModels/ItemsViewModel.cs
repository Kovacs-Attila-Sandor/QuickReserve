using QuickReserve.Models;
using QuickReserve.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QuickReserve.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private User _selectedItem;

        public ObservableCollection<User> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<User> ItemTapped { get; }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

  
    }
}