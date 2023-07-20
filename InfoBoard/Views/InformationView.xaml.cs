using InfoBoard.Models;

namespace InfoBoard.Views;

public partial class InformationView : ContentPage, IQueryAttributable
{

    //public Information InfoMessage { get; private set; }
    public InformationView()
	{
		InitializeComponent();
        //BindingContext = InfoMessage;
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> message)
    {
        var infoMessage = message["PickCategories"] as Information;
        BindingContext = infoMessage;

        //OnPropertyChanged(nameof(Information));
        //OnPropertyChanged(nameof(InfoMessage));
        //OnPropertyChanged(nameof(Information.Title));
        //OnPropertyChanged(nameof(Information.Message));
        //throw new NotImplementedException();
    }
}