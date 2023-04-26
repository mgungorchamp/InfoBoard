using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoBoard.Models;
internal class ImageLoader
{
    public ObservableCollection<string> ImageBasket { get; set; } = new ObservableCollection<string>();

    public Image FirstImage 
    {
       
        get 
        {
            Image image = new Image();
            image.Source = new UriImageSource
            {
                Uri = new Uri("http://clipart-library.com/image_gallery2/Nature-PNG-File.png"),
                CacheValidity = new TimeSpan(10, 0, 0, 0)
            };       

            return image; 
        } 
    }

    public ImageLoader() =>
        LoadImages();

    public void LoadImages()
    {
        ImageBasket.Clear();

        // Get the folder where the notes are stored.
        string appDataPath = FileSystem.AppDataDirectory;

        // Use Linq extensions to load the *.notes.txt files.
        IEnumerable<string> imageList = Directory

                                    // Select the file names from the directory
                                    .EnumerateFiles(appDataPath, "*.png")
                                    
                                    // With the final collection of notes, order them by date
                                    .OrderBy(pic => pic);

        // Add each note into the ObservableCollection
        foreach (string anImage in imageList)
            ImageBasket.Add(anImage);
    }
}