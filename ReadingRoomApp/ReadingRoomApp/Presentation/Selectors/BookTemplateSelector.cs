using System.Windows;
using System.Windows.Controls;
using ReadingRoomApp.Core.Domain.Entities;

namespace ReadingRoomApp.Presentation.Selectors
{
    public class BookTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AvailableBookTemplate { get; set; }
        public DataTemplate UnavailableBookTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Book book)
            {
                return book.IsAvailable ? AvailableBookTemplate : UnavailableBookTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}