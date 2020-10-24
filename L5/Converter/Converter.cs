using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace L5.Converter
{
    public static class Converter
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }
    }
}