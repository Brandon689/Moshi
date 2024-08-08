namespace Moshi.Forums.Data
{
    public class RandomPicker<T>
    {
        private readonly List<T> _items;
        private int _currentIndex;
        private readonly Random _random;

        public RandomPicker(List<T> items)
        {
            _items = new List<T>(items);
            _random = new Random();
            Shuffle();
        }

        public T PickNext()
        {
            if (_currentIndex >= _items.Count)
            {
                Shuffle();
                _currentIndex = 0;
            }

            return _items[_currentIndex++];
        }

        private void Shuffle()
        {
            for (int i = _items.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                T temp = _items[i];
                _items[i] = _items[j];
                _items[j] = temp;
            }
        }
    }
}
