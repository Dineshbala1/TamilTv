using TamilSerial.ViewModels.Base;

namespace TamilTv.Models
{
    public class SelectedPlayable : ExtendedBindableObject
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }

        public string Title { get; set; }

        public string Url { get; set; }
    }
}
